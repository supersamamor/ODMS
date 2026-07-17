using FBSC.ODMS.Application.DTOs;
using FBSC.ODMS.Core.Constants;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Services.Dashboard;
using Newtonsoft.Json;

namespace FBSC.ODMS.Application.Helpers
{
    /// <summary>
    /// Executes a DashboardWidget through the Phase-2 query execution engine
    /// (DashboardQueryExecutionService) and shapes the result into exactly the same
    /// ReportResultModel/JSON contract ReportDataHelper produces for a legacy Report -
    /// Results/ColumnHeaders as JSON strings, ReportOrChartType as a ReportChartType constant -
    /// so DashboardRenderer/_Chart.cshtml/_ChartBuilderScripts render a DashboardWidget with
    /// zero changes to that stack. ReportTypeState.ChartRenderer values are expected to be
    /// ReportChartType constants (Table/Bar/HorizontalBar/Pie/Doughnut/PolarArea) for exactly
    /// this reason - not a separate chart.js-native vocabulary.
    /// A widget's DataSourceConnection may be a live ExternalDatabase or a locally-staged
    /// UploadedFile - DashboardQueryExecutionService/DataSourceConnectionFactory already
    /// absorb that distinction, so nothing here branches on ConnectionKind.
    /// </summary>
    public static class DashboardWidgetRenderHelper
    {
        public static async Task<ReportResultModel> RenderAsync(
            DashboardWidgetState widget,
            DashboardQueryExecutionService executionService,
            ChartConfigurationService chartConfigurationService,
            IReadOnlyDictionary<string, string?> parameterValues,
            string? userId,
            string? traceId,
            CancellationToken cancellationToken)
        {
            var query = widget.DashboardQuery!;
            var reportType = widget.ReportType!;
            var spanWidth = GridWidthToSpanWidth(widget.GridWidth);

            var executionResult = await executionService.ExecuteAsync(query, parameterValues, userId, traceId, cancellationToken);
            if (!executionResult.Success)
            {
                return new ReportResultModel
                {
                    ReportId = widget.Id,
                    ReportName = widget.Title,
                    ReportOrChartType = reportType.ChartRenderer,
                    SpanWidth = spanWidth,
                };
            }

            if (reportType.ChartRenderer == ReportChartType.Table)
            {
                return BuildTableResult(widget, executionResult, spanWidth);
            }

            var catalogColumns = (query.DashboardQueryResultColumnList ?? []).ToList();
            var mapping = chartConfigurationService.ResolveMapping(widget, reportType, executionResult.Columns, catalogColumns);
            if (string.IsNullOrEmpty(mapping.XAxisColumnName) || mapping.YAxisColumnNames.Count == 0)
            {
                return new ReportResultModel
                {
                    ReportId = widget.Id,
                    ReportName = widget.Title,
                    ReportOrChartType = reportType.ChartRenderer,
                    SpanWidth = spanWidth,
                };
            }

            return BuildChartResult(widget, reportType, executionResult, mapping, spanWidth);
        }

        private static int GridWidthToSpanWidth(int gridWidth) => gridWidth switch
        {
            <= 0 => 50,
            <= 3 => 25,
            <= 6 => 50,
            _ => 100,
        };

        private static ReportResultModel BuildTableResult(DashboardWidgetState widget, DashboardQueryExecutionResult executionResult, int spanWidth)
        {
            var sanitizedNames = executionResult.Columns.Select(c => StringHelper.Sanitize(c.ColumnName)).ToList();
            var columnHeaders = executionResult.Columns
                .Select((c, i) => new Dictionary<string, string> { ["title"] = StringHelper.ToProperCase(c.ColumnName), ["data"] = sanitizedNames[i] })
                .ToList();
            var rows = executionResult.Rows.Select(row =>
            {
                var dict = new Dictionary<string, object?>(executionResult.Columns.Count);
                for (var i = 0; i < executionResult.Columns.Count; i++)
                {
                    dict[sanitizedNames[i]] = row.GetValueOrDefault(executionResult.Columns[i].ColumnName);
                }
                return dict;
            }).ToList();

            return new ReportResultModel
            {
                ReportId = widget.Id,
                ReportName = widget.Title,
                ReportOrChartType = ReportChartType.Table,
                Results = JsonConvert.SerializeObject(rows, Formatting.Indented),
                ColumnHeaders = JsonConvert.SerializeObject(columnHeaders, Formatting.Indented),
                SpanWidth = spanWidth,
            };
        }

        private static ReportResultModel BuildChartResult(
            DashboardWidgetState widget,
            ReportTypeState reportType,
            DashboardQueryExecutionResult executionResult,
            ResolvedFieldMapping mapping,
            int spanWidth)
        {
            string resultsJson;
            List<string> columnHeaders;
            bool displayLegend;

            if (!string.IsNullOrEmpty(mapping.SeriesColumnName))
            {
                // Long-format pivot: one dataset per distinct series value, aligned against
                // the overall distinct set of X-axis labels (0 where a series has no row for
                // a given label). This is the one shape ReportDataHelper has no equivalent
                // for (it only ever reads a fixed wide-format resultset), since a
                // WidgetFieldMapping series column is a genuinely more flexible concept.
                var labels = executionResult.Rows
                    .Select(r => Convert.ToString(r.GetValueOrDefault(mapping.XAxisColumnName!)) ?? "")
                    .Distinct()
                    .ToList();
                var yColumn = mapping.YAxisColumnNames[0];
                var seriesGroups = executionResult.Rows
                    .GroupBy(r => Convert.ToString(r.GetValueOrDefault(mapping.SeriesColumnName)) ?? "");

                var datasets = seriesGroups.Select(group => new ReportDataHelper.MultipleDataset
                {
                    Label = group.Key,
                    Data = [.. labels.Select(label => (decimal)ChartConfigurationService.AggregateFor(
                        group.Where(r => Convert.ToString(r.GetValueOrDefault(mapping.XAxisColumnName!)) == label),
                        yColumn,
                        widget.AggregationOverride))],
                }).ToList();

                columnHeaders = labels;
                resultsJson = JsonConvert.SerializeObject(datasets, Formatting.Indented);
                displayLegend = true;
            }
            else
            {
                // Wide format: rows map 1:1 to data points in row order, exactly like
                // ReportDataHelper - the widget's own SQL query is expected to already be
                // shaped/grouped as needed (no implicit aggregation across duplicate labels).
                columnHeaders = [.. executionResult.Rows.Select(r => Convert.ToString(r.GetValueOrDefault(mapping.XAxisColumnName!)) ?? "")];

                if (mapping.YAxisColumnNames.Count > 1)
                {
                    var datasets = mapping.YAxisColumnNames.Select(yColumn => new ReportDataHelper.MultipleDataset
                    {
                        Label = yColumn,
                        Data = [.. executionResult.Rows.Select(r => ToDecimal(r.GetValueOrDefault(yColumn)))],
                    }).ToList();
                    resultsJson = JsonConvert.SerializeObject(datasets, Formatting.Indented);
                    displayLegend = true;
                }
                else
                {
                    var yColumn = mapping.YAxisColumnNames[0];
                    var dataset = new ReportDataHelper.Dataset
                    {
                        Label = yColumn,
                        Data = [.. executionResult.Rows.Select(r => ToDecimal(r.GetValueOrDefault(yColumn)))],
                    };
                    resultsJson = JsonConvert.SerializeObject(new[] { dataset }, Formatting.Indented);
                    displayLegend = false;
                }
            }

            return new ReportResultModel
            {
                ReportId = widget.Id,
                ReportName = widget.Title,
                ReportOrChartType = reportType.ChartRenderer,
                Results = resultsJson,
                ColumnHeaders = JsonConvert.SerializeObject(columnHeaders, Formatting.Indented),
                DisplayLegend = displayLegend,
                SpanWidth = spanWidth,
            };
        }

        private static decimal ToDecimal(object? value) => value switch
        {
            null => 0m,
            decimal d => d,
            IConvertible c => Convert.ToDecimal(c),
            _ => 0m,
        };
    }
}
