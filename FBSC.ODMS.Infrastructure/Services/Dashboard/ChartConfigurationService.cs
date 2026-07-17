using FBSC.ODMS.Core.Constants;
using FBSC.ODMS.Core.ODMS;
using System.Text.Json;

namespace FBSC.ODMS.Infrastructure.Services.Dashboard;

public record ResolvedFieldMapping(string? XAxisColumnName, IReadOnlyList<string> YAxisColumnNames, string? SeriesColumnName);

public record ChartConfigurationResult(bool Success, string? ErrorMessage, object? Config);

/// <summary>
/// Turns "a query's result columns + a widget's field mapping" into a chart-library-ready
/// configuration object, without any per-schema/per-source code. When a widget has no
/// explicit mapping yet (XAxisColumnName/YAxisColumnsJson/SeriesColumnName all unset), the
/// mapping is auto-suggested from DashboardQueryResultColumnState.InferredRole (falling back
/// to name/type heuristics for columns that were never catalogued).
/// </summary>
public class ChartConfigurationService
{
    public ChartConfigurationResult BuildConfig(
        DashboardWidgetState widget,
        ReportTypeState reportType,
        IReadOnlyList<DashboardQueryResultColumnInfo> resultColumns,
        IReadOnlyList<Dictionary<string, object?>> rows,
        IReadOnlyList<DashboardQueryResultColumnState> catalogColumns)
    {
        var mapping = ResolveMapping(widget, reportType, resultColumns, catalogColumns);

        if (reportType.RequiresXAxis && string.IsNullOrEmpty(mapping.XAxisColumnName))
        {
            return new ChartConfigurationResult(false, $"'{reportType.Name}' requires a label/X-axis column but none could be mapped or inferred.", null);
        }
        if (reportType.RequiresYAxis && mapping.YAxisColumnNames.Count == 0)
        {
            return new ChartConfigurationResult(false, $"'{reportType.Name}' requires at least one value/Y-axis column but none could be mapped or inferred.", null);
        }
        if (reportType.RequiresSeriesGrouping && string.IsNullOrEmpty(mapping.SeriesColumnName))
        {
            return new ChartConfigurationResult(false, $"'{reportType.Name}' requires a series-grouping column but none could be mapped or inferred.", null);
        }
        if (reportType.MinColumnsRequired is { } min && resultColumns.Count < min)
        {
            return new ChartConfigurationResult(false, $"'{reportType.Name}' requires at least {min} result column(s), the query returned {resultColumns.Count}.", null);
        }

        if (reportType.ChartRenderer.Equals("table", StringComparison.OrdinalIgnoreCase))
        {
            return new ChartConfigurationResult(true, null, new
            {
                type = "table",
                columns = resultColumns.Select(c => c.ColumnName),
                rows,
            });
        }

        if (reportType.ChartRenderer.Equals("kpi", StringComparison.OrdinalIgnoreCase))
        {
            var valueColumn = mapping.YAxisColumnNames.FirstOrDefault() ?? resultColumns.FirstOrDefault()?.ColumnName;
            var value = valueColumn is not null && rows.Count > 0 ? rows[0].GetValueOrDefault(valueColumn) : null;
            return new ChartConfigurationResult(true, null, new { type = "kpi", label = widget.Title, value });
        }

        return new ChartConfigurationResult(true, null, BuildChartJsConfig(widget, reportType, mapping, rows));
    }

    private static object BuildChartJsConfig(DashboardWidgetState widget, ReportTypeState reportType, ResolvedFieldMapping mapping, IReadOnlyList<Dictionary<string, object?>> rows)
    {
        if (!string.IsNullOrEmpty(mapping.SeriesColumnName))
        {
            var seriesGroups = rows.GroupBy(r => Convert.ToString(r.GetValueOrDefault(mapping.SeriesColumnName)) ?? "").ToList();
            var labels = rows.Select(r => Convert.ToString(r.GetValueOrDefault(mapping.XAxisColumnName!)) ?? "")
                .Distinct().ToList();
            var datasets = seriesGroups.Select(g => new
            {
                label = g.Key,
                data = labels.Select(label => AggregateFor(g.Where(r => Convert.ToString(r.GetValueOrDefault(mapping.XAxisColumnName!)) == label), mapping.YAxisColumnNames.First(), widget.AggregationOverride)),
            });
            return new { type = ChartRendererToChartJsType(reportType.ChartRenderer), data = new { labels, datasets } };
        }

        var plainLabels = rows.Select(r => mapping.XAxisColumnName is not null ? Convert.ToString(r.GetValueOrDefault(mapping.XAxisColumnName)) ?? "" : "").ToList();
        var plainDatasets = mapping.YAxisColumnNames.Select(y => new
        {
            label = y,
            data = rows.Select(r => r.GetValueOrDefault(y)),
        });
        return new { type = ChartRendererToChartJsType(reportType.ChartRenderer), data = new { labels = plainLabels, datasets = plainDatasets } };
    }

    /// <summary>
    /// Public so DashboardWidgetRenderHelper (Application layer, feeding the legacy
    /// _ChartBuilderScripts.cshtml rendering pipeline) can apply the exact same aggregation
    /// rule when pivoting a series-grouped result set - one aggregation implementation, not two.
    /// </summary>
    public static double AggregateFor(IEnumerable<Dictionary<string, object?>> rows, string column, string? aggregation)
    {
        var values = rows.Select(r => ToDouble(r.GetValueOrDefault(column))).ToList();
        return (aggregation ?? AggregationType.Sum) switch
        {
            AggregationType.Avg => values.Count > 0 ? values.Average() : 0,
            AggregationType.Count => values.Count,
            AggregationType.Min => values.Count > 0 ? values.Min() : 0,
            AggregationType.Max => values.Count > 0 ? values.Max() : 0,
            AggregationType.None => values.FirstOrDefault(),
            _ => values.Sum(),
        };
    }

    public static double ToDouble(object? value) => value switch
    {
        null => 0,
        double d => d,
        decimal m => (double)m,
        IConvertible c => Convert.ToDouble(c),
        _ => 0,
    };

    private static string ChartRendererToChartJsType(string chartRenderer) => chartRenderer.ToLowerInvariant() switch
    {
        "horizontal bar" => "bar",
        var other => other,
    };

    public ResolvedFieldMapping ResolveMapping(
        DashboardWidgetState widget,
        ReportTypeState reportType,
        IReadOnlyList<DashboardQueryResultColumnInfo> resultColumns,
        IReadOnlyList<DashboardQueryResultColumnState> catalogColumns)
    {
        var catalogByName = catalogColumns.ToDictionary(c => c.ColumnName, StringComparer.OrdinalIgnoreCase);

        string? xAxis = widget.XAxisColumnName;
        var yAxis = DeserializeYAxisColumns(widget.YAxisColumnsJson);
        string? series = widget.SeriesColumnName;

        if (string.IsNullOrEmpty(xAxis))
        {
            xAxis = resultColumns
                .FirstOrDefault(c => RoleFor(c, catalogByName) is SemanticType.Dimension or SemanticType.Date)?.ColumnName;
        }

        if (yAxis.Count == 0)
        {
            yAxis = resultColumns
                .Where(c => !c.ColumnName.Equals(xAxis, StringComparison.OrdinalIgnoreCase) && RoleFor(c, catalogByName) == SemanticType.Measure)
                .Select(c => c.ColumnName)
                .ToList();
        }

        if (string.IsNullOrEmpty(series) && reportType.RequiresSeriesGrouping)
        {
            series = resultColumns
                .FirstOrDefault(c => !c.ColumnName.Equals(xAxis, StringComparison.OrdinalIgnoreCase)
                    && !yAxis.Contains(c.ColumnName, StringComparer.OrdinalIgnoreCase)
                    && RoleFor(c, catalogByName) == SemanticType.Dimension)?.ColumnName;
        }

        return new ResolvedFieldMapping(xAxis, yAxis, series);
    }

    /// <summary>
    /// Cataloged InferredRole (from schema discovery) wins; falls back to the same
    /// name/type heuristic DataSourceSchemaDiscoveryService uses for uncatalogued columns
    /// (e.g. an ad-hoc computed column in the SELECT list) so there's one heuristic, not two.
    /// </summary>
    private static string RoleFor(DashboardQueryResultColumnInfo column, IReadOnlyDictionary<string, DashboardQueryResultColumnState> catalog) =>
        catalog.TryGetValue(column.ColumnName, out var cataloged) && !string.IsNullOrEmpty(cataloged.InferredRole)
            ? cataloged.InferredRole
            : DataSourceSchemaDiscoveryService.InferSemanticType(column.ColumnName, column.SqlDataType);

    private static List<string> DeserializeYAxisColumns(string? yAxisColumnsJson)
    {
        if (string.IsNullOrWhiteSpace(yAxisColumnsJson))
        {
            return [];
        }
        try
        {
            return JsonSerializer.Deserialize<List<string>>(yAxisColumnsJson) ?? [];
        }
        catch (JsonException)
        {
            return [];
        }
    }
}
