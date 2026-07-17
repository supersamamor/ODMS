using FBSC.ODMS.Application.DTOs;
using FBSC.ODMS.Core.Constants;
using FBSC.ODMS.Core.ODMS;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Data;
using FBSC.Common.Identity.Abstractions;


namespace FBSC.ODMS.Application.Helpers
{
    public static class ReportDataHelper
    {
        public static async Task<LabelResultAndStyle> ConvertSQLQueryToJsonAsync(
            IAuthenticatedUser authenticatedUser,
            string connectionString,
            ReportState report,
            IList<ReportQueryFilterModel>? filters = null)
        {
            // Preprocess query
            string sql = report.QueryString!;
            sql = StringHelper.ReplaceCaseInsensitive(sql, ShortCodes.CurrentUserId, $"'{authenticatedUser.UserId}'");
            sql = StringHelper.ReplaceCaseInsensitive(sql, ShortCodes.CurrentDateTime, "GETDATE()");

            var validationResult = SQLValidatorHelper.Validate(sql);
            if (!validationResult.IsValid)
            {
                // Combine errors into a single readable message
                var errorMessage = string.Join(", ", validationResult.Errors);
                // Throw the specific domain exception
                throw new Exception(errorMessage);
            }

            using SqlConnection conn = new(connectionString);
            await conn.OpenAsync();

            using SqlCommand cmd = new(sql, conn);

            // Fast parameter binding
            if (filters?.Count > 0)
            {
                foreach (var f in filters)
                {
                    SqlDbType type = SqlDbType.NVarChar;
                    object value = f.FieldValue ?? "";

                    switch (f.DataType)
                    {
                        case DataTypes.Years:
                        case DataTypes.Months:
                            type = SqlDbType.Int;
                            value = string.IsNullOrWhiteSpace(f.FieldValue) ? 0 : int.Parse(f.FieldValue);
                            break;

                        case DataTypes.Date:
                            type = SqlDbType.DateTime;
                            value = string.IsNullOrWhiteSpace(f.FieldValue)
                                ? new DateTime(1900, 1, 1)
                                : DateTime.Parse(f.FieldValue);
                            break;
                    }

                    var p = cmd.Parameters.Add($"@{f.FieldName}", type);
                    p.Value = value;
                }
            }

            using SqlDataReader reader = await cmd.ExecuteReaderAsync();

            // -------------------------------
            // 1️ TABLE OUTPUT (fast dictionary mapping)
            // -------------------------------
            if (report.ReportOrChartType == ReportChartType.Table)
            {
                var rows = new List<Dictionary<string, object?>>();
                var columns = new List<Dictionary<string, string>>();

                int fieldCount = reader.FieldCount;
                string[] colNames = new string[fieldCount];
                string[] colNamesSanitized = new string[fieldCount];

                // Cache column names once
                for (int i = 0; i < fieldCount; i++)
                {
                    colNames[i] = reader.GetName(i);
                    colNamesSanitized[i] = StringHelper.Sanitize(colNames[i]);

                    columns.Add(new Dictionary<string, string>
                    {
                        ["title"] = StringHelper.ToProperCase(colNames[i]),
                        ["data"] = colNamesSanitized[i]
                    });
                }

                while (reader.Read())
                {
                    var row = new Dictionary<string, object?>(fieldCount);
                    for (int i = 0; i < fieldCount; i++)
                        row[colNamesSanitized[i]] = reader.GetValue(i);

                    rows.Add(row);
                }

                return new LabelResultAndStyle
                {
                    Results = JsonConvert.SerializeObject(rows, Formatting.Indented),
                    ColumnHeaders = JsonConvert.SerializeObject(columns, Formatting.Indented)
                };
            }

            // -------------------------------
            // 2️ PDF and Customeo Html OUTPUT (HIGHEST PERFORMANCE)
            // -------------------------------
            if (report.ReportOrChartType == ReportChartType.PDF
                || report.ReportOrChartType == ReportChartType.CustomHtml)
            {
                var dt = new DataTable();
                dt.Load(reader);
                var model = new
                {
                    Table = dt,
                    report.ReportName,
                    ReportGenerated = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };
                return new LabelResultAndStyle
                {
                    PdfReportData = model,
                    HTMLTemplate = report.HtmlTemplate,
                    HTMLFooterTemplate = report.HTMLFooterTemplate,
                };
            }

            // -------------------------------
            // 3️ CHART OUTPUT (BAR / PIE)
            // -------------------------------
            // Note: dataset colors are intentionally left unset here. They are
            // generated client-side from the active theme's --custom-primary
            // CSS variable (see _ChartBuilderScripts.cshtml), so charts stay
            // in sync with whichever theme is applied without the backend
            // needing to know which theme is active.

            bool isMultiSet = reader.FieldCount > 2;

            var columnLabels = new HashSet<string?>();
            var multipleData = new Dictionary<string, MultipleDataset>();
            var datasetGroup = new Dictionary<string, Dataset>();
            var singleData = new Dataset();

            string[] names = new string[reader.FieldCount];
            Type[] types = new Type[reader.FieldCount];

            for (int i = 0; i < reader.FieldCount; i++)
            {
                names[i] = reader.GetName(i);
                types[i] = reader.GetFieldType(i);
            }

            while (reader.Read())
            {
                for (int i = 0; i < names.Length; i++)
                {
                    string name = names[i];

                    if (name.Equals("Label", StringComparison.OrdinalIgnoreCase))
                    {
                        columnLabels.Add(reader[i]?.ToString());
                        continue;
                    }

                    // Numeric check (NO STRING ALLOCATION)
                    var t = types[i];
                    bool isNumeric =
                        t == typeof(int) || t == typeof(long) ||
                        t == typeof(decimal) || t == typeof(double) ||
                        t == typeof(float);

                    if (!isNumeric)
                        continue;

                    decimal value = Convert.ToDecimal(reader.GetValue(i));

                    // MULTIPLE RESULTSET: multi-series charts (one dataset per
                    // column, single color per dataset) vs. distribution charts
                    // (dataset(s) with one color per data point).
                    if (isMultiSet)
                    {
                        switch (report.ReportOrChartType)
                        {
                            case ReportChartType.HorizontalBar:
                            case ReportChartType.Bar:    
                                if (!multipleData.TryGetValue(name, out var multi))
                                {
                                    multi = new MultipleDataset
                                    {
                                        Label = name
                                    };
                                    multipleData[name] = multi;
                                }

                                multi.Data.Add(value);

                                break;

                            case ReportChartType.Pie:
                            case ReportChartType.Doughnut:
                            case ReportChartType.PolarArea:

                                if (!datasetGroup.TryGetValue(name, out var pie))
                                {
                                    pie = new Dataset
                                    {
                                        Label = name
                                    };
                                    datasetGroup[name] = pie;
                                }

                                pie.Data.Add(value);

                                break;
                        }
                    }
                    else
                    {
                        // SINGLE DATASET
                        if (singleData.Label == null)
                            singleData.Label = name;

                        singleData.Data.Add(value);
                    }
                }
            }

            // -------------------------------
            // RETURN CHART RESULTS
            // -------------------------------

            if (isMultiSet)
            {
                return report.ReportOrChartType switch
                {
                    ReportChartType.HorizontalBar or ReportChartType.Bar => new LabelResultAndStyle
                    {
                        Results = JsonConvert.SerializeObject(multipleData.Values, Formatting.Indented),
                        ColumnHeaders = JsonConvert.SerializeObject(columnLabels, Formatting.Indented),
                        DisplayLegend = true
                    },

                    ReportChartType.Pie or ReportChartType.Doughnut or ReportChartType.PolarArea => new LabelResultAndStyle
                    {
                        Results = JsonConvert.SerializeObject(datasetGroup.Values, Formatting.Indented),
                        ColumnHeaders = JsonConvert.SerializeObject(columnLabels, Formatting.Indented),
                        DisplayLegend = true
                    },

                    _ => new LabelResultAndStyle()
                };
            }

            // SINGLE DATASET RESULT
            return new LabelResultAndStyle
            {
                Results = JsonConvert.SerializeObject(new[] { singleData }, Formatting.Indented),
                ColumnHeaders = JsonConvert.SerializeObject(columnLabels, Formatting.Indented),
                DisplayLegend = false
            };
        }

        public static async Task<List<Dictionary<string, string?>>> ConvertTableKeyValueToDictionary(string connectionString, string tableKeyValue, string? filter)
        {
            string[] queryComponents = tableKeyValue.Split(',');
            var query = $"select Distinct {queryComponents[1]} as [Key],{queryComponents[2]} as [Value] from {queryComponents[0]}";
            if (!string.IsNullOrEmpty(filter))
            {
                query += $" where {filter}";
            }
            query += $" order by {queryComponents[2]}";
            var validationResult = SQLValidatorHelper.Validate(query);
            if (!validationResult.IsValid)
            {
                // Combine errors into a single readable message
                var errorMessage = string.Join(", ", validationResult.Errors);
                // Throw the specific domain exception
                throw new Exception(errorMessage);
            }
            using SqlConnection connection = new(connectionString);
            connection.Open();
            List<Dictionary<string, string?>> tableData = [];
            using SqlCommand command = new(query, connection);
            using SqlDataReader reader = await command.ExecuteReaderAsync();
            while (reader.Read())
            {
                Dictionary<string, string?> rowData = [];
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    rowData[reader.GetName(i)] = reader[i]?.ToString();
                }
                tableData.Add(rowData);
            }
            return tableData;
        }
        public class LabelResultAndStyle
        {
            public string? Results { get; set; }
            public string? ColumnHeaders { get; set; }
            public string? Colors { get; set; }
            public bool DisplayLegend { get; set; }
            public object? PdfReportData { get; set; }
            public string? HTMLTemplate { get; set; }
            public string? HTMLFooterTemplate { get; init; } = "";
        }
        // internal (not private) so DashboardWidgetRenderHelper can emit the exact same JSON
        // shape for DashboardWidget-sourced charts as this class emits for Report-sourced
        // ones - _ChartBuilderScripts.cshtml's BuildChart() JS is shared by both and expects
        // one contract, not two near-identical ones.
        internal class Dataset
        {
            [JsonProperty("label")]
            public string? Label { get; set; }
            [JsonProperty("data")]
            public List<decimal> Data { get; set; } = [];
            [JsonProperty("backgroundColor")]
            public List<string> BackgroundColor { get; set; } = [];
            [JsonProperty("borderWidth")]
            public int BorderWidth { get; set; } = 1;
        }
        internal class MultipleDataset
        {
            [JsonProperty("label")]
            public string? Label { get; set; }
            [JsonProperty("data")]
            public List<decimal> Data { get; set; } = [];
            [JsonProperty("backgroundColor")]
            public string BackgroundColor { get; set; } = "";
            [JsonProperty("borderWidth")]
            public int BorderWidth { get; set; } = 1;
        }
    }
}