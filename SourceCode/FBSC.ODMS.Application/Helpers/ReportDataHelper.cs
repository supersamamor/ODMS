using FBSC.ODMS.Application.DTOs;
using FBSC.ODMS.Core.Constants;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Data;
using FBSC.Common.Identity.Abstractions;
using FBSC.Common.Utility.Helpers;


namespace FBSC.ODMS.Application.Helpers
{
    public static class ReportDataHelper
    {
        private const string DefaultConnectionStringKey = "ReportContext";

        /// <summary>
        /// Resolves the connection string a single report should execute against: the
        /// <see cref="DataSourceState"/> referenced by <paramref name="dataSourceId"/> when set,
        /// otherwise the default "ReportContext" connection string.
        /// </summary>
        public static async Task<string> ResolveConnectionStringAsync(
            ApplicationContext context,
            IConfiguration configuration,
            string? dataSourceId,
            CancellationToken cancellationToken = default)
        {
            var defaultConnectionString = configuration.GetConnectionString(DefaultConnectionStringKey)!;
            if (string.IsNullOrWhiteSpace(dataSourceId))
            {
                return defaultConnectionString;
            }

            var dataSource = await context.DataSource
                .AsNoTracking()
                .Where(d => d.Id == dataSourceId && d.IsActive)
                .Select(d => new DataSourceConnectionInfo(
                    d.Id, d.DataSourceType, d.ServerAddress, d.DatabaseName, d.AuthenticationType,
                    d.Username, d.PasswordEncrypted, d.ConnectionStringEncrypted))
                .FirstOrDefaultAsync(cancellationToken);

            return BuildConnectionString(dataSource, configuration, defaultConnectionString);
        }

        /// <summary>
        /// Batch-resolves connection strings for many reports (e.g. a dashboard render) in a single
        /// round-trip against the DataSource table, avoiding one query per report when scaling to
        /// many concurrently-rendered widgets.
        /// </summary>
        public static async Task<IReadOnlyDictionary<string, string>> ResolveConnectionStringsAsync(
            ApplicationContext context,
            IConfiguration configuration,
            IEnumerable<string?> dataSourceIds,
            CancellationToken cancellationToken = default)
        {
            var defaultConnectionString = configuration.GetConnectionString(DefaultConnectionStringKey)!;
            var distinctIds = dataSourceIds
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Select(id => id!)
                .Distinct(StringComparer.Ordinal)
                .ToArray();

            var map = new Dictionary<string, string>(distinctIds.Length, StringComparer.Ordinal);
            if (distinctIds.Length == 0)
            {
                return map;
            }

            var dataSources = await context.DataSource
                .AsNoTracking()
                .Where(d => distinctIds.Contains(d.Id) && d.IsActive)
                .Select(d => new DataSourceConnectionInfo(
                    d.Id, d.DataSourceType, d.ServerAddress, d.DatabaseName, d.AuthenticationType,
                    d.Username, d.PasswordEncrypted, d.ConnectionStringEncrypted))
                .ToDictionaryAsync(d => d.Id, StringComparer.Ordinal, cancellationToken);

            foreach (var id in distinctIds)
            {
                dataSources.TryGetValue(id, out var dataSource);
                map[id] = BuildConnectionString(dataSource, configuration, defaultConnectionString);
            }

            return map;
        }

        private static string BuildConnectionString(
            DataSourceConnectionInfo? dataSource,
            IConfiguration configuration,
            string defaultConnectionString)
        {
            if (dataSource is not { } ds)
            {
                return defaultConnectionString;
            }

            if (string.Equals(ds.DataSourceType, DataSourceTypes.FileUpload, StringComparison.OrdinalIgnoreCase))
            {
                // A file-upload data source has no external server to connect to: its rows live in a
                // dynamically-created table inside the same physical database ReportContext points at.
                return defaultConnectionString;
            }

            var decryptionKey = $"{configuration["EncryptionDecryptionKeyPrefix"]}{ds.Id}";

            if (!string.IsNullOrWhiteSpace(ds.ConnectionStringEncrypted))
            {
                var decrypted = EncryptionHelper.DecryptPassword(ds.ConnectionStringEncrypted, decryptionKey);
                return string.IsNullOrWhiteSpace(decrypted) ? defaultConnectionString : decrypted;
            }

            if (string.IsNullOrWhiteSpace(ds.ServerAddress) || string.IsNullOrWhiteSpace(ds.DatabaseName))
            {
                return defaultConnectionString;
            }

            SqlConnectionStringBuilder builder = new()
            {
                DataSource = ds.ServerAddress,
                InitialCatalog = ds.DatabaseName,
                TrustServerCertificate = true,
                ConnectTimeout = 15
            };

            if (string.Equals(ds.AuthenticationType, AuthenticationTypes.WindowsIntegrated, StringComparison.OrdinalIgnoreCase))
            {
                builder.IntegratedSecurity = true;
            }
            else if (string.Equals(ds.AuthenticationType, AuthenticationTypes.AzureAD, StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrWhiteSpace(ds.Username))
                {
                    builder.UserID = ds.Username;
                    builder.Password = string.IsNullOrWhiteSpace(ds.PasswordEncrypted)
                        ? ""
                        : EncryptionHelper.DecryptPassword(ds.PasswordEncrypted, decryptionKey);
                    builder.Authentication = SqlAuthenticationMethod.ActiveDirectoryPassword;
                }
                else
                {
                    builder.Authentication = SqlAuthenticationMethod.ActiveDirectoryDefault;
                }
            }
            else
            {
                builder.UserID = ds.Username ?? "";
                builder.Password = string.IsNullOrWhiteSpace(ds.PasswordEncrypted)
                    ? ""
                    : EncryptionHelper.DecryptPassword(ds.PasswordEncrypted, decryptionKey);
            }

            return builder.ConnectionString;
        }

        private readonly record struct DataSourceConnectionInfo(
            string Id,
            string DataSourceType,
            string? ServerAddress,
            string? DatabaseName,
            string? AuthenticationType,
            string? Username,
            string? PasswordEncrypted,
            string? ConnectionStringEncrypted);

        /// <summary>
        /// Appends the dashboard's global filter values (free-text project search,
        /// ISO work-week of the current year, business unit id) to a report's filter
        /// list under the <see cref="GlobalReportFilterFields"/> parameter names.
        /// The selected week number is precomputed into its Monday-Sunday date range
        /// (@GlobalWeekStartDate/@GlobalWeekEndDate) so report SQL never has to do
        /// week math. Idempotent per field name: entries already present (e.g. added
        /// earlier in the same request with real values) are left untouched, so query
        /// handlers can call this defensively with no values to guarantee the
        /// parameters always exist. Reports that don't reference these parameters are
        /// unaffected - sp_executesql permits declared-but-unused parameters.
        /// </summary>
        public static void AppendGlobalFilters(
            IList<ReportQueryFilterModel> filters,
            string? projectSearch = null,
            int? weekNumber = null,
            string? businessUnitId = null)
        {
            string weekStart = "", weekEnd = "";
            if (weekNumber is > 0)
            {
                var isoYear = System.Globalization.ISOWeek.GetYear(DateTime.Today);
                var week = Math.Min(weekNumber.Value, System.Globalization.ISOWeek.GetWeeksInYear(isoYear));
                var start = System.Globalization.ISOWeek.ToDateTime(isoYear, week, DayOfWeek.Monday);
                weekStart = start.ToString("yyyy-MM-dd");
                weekEnd = start.AddDays(6).ToString("yyyy-MM-dd");
            }

            AppendIfMissing(filters, GlobalReportFilterFields.Project, "", projectSearch ?? "");
            AppendIfMissing(filters, GlobalReportFilterFields.WeekNumber, DataTypes.WholeNumber, weekNumber is > 0 ? weekNumber.Value.ToString() : "");
            AppendIfMissing(filters, GlobalReportFilterFields.WeekStartDate, DataTypes.Date, weekStart);
            AppendIfMissing(filters, GlobalReportFilterFields.WeekEndDate, DataTypes.Date, weekEnd);
            AppendIfMissing(filters, GlobalReportFilterFields.BusinessUnit, "", businessUnitId ?? "");

            static void AppendIfMissing(IList<ReportQueryFilterModel> filters, string fieldName, string dataType, string value)
            {
                if (filters.Any(f => string.Equals(f.FieldName, fieldName, StringComparison.OrdinalIgnoreCase)))
                {
                    return;
                }
                filters.Add(new ReportQueryFilterModel
                {
                    FieldName = fieldName,
                    FieldDescription = fieldName,
                    DataType = dataType,
                    FieldValue = value,
                });
            }
        }

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
                        case DataTypes.WholeNumber:
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
            // 2️⃣b BUBBLE / SCATTER OUTPUT (X/Y/R POINT DATA)
            // -------------------------------
            // These chart types don't plot a numeric value per category label the
            // way Bar/Pie/Line do - each row is an (x, y[, r]) coordinate, grouped
            // into series by an (optional) `Label` column. Column matching is by
            // name (case-insensitive) rather than position, so query authors can
            // list fields in any order.
            if (report.ReportOrChartType == ReportChartType.Bubble || report.ReportOrChartType == ReportChartType.Scatter)
            {
                bool isBubble = report.ReportOrChartType == ReportChartType.Bubble;

                int labelOrdinal = -1, xOrdinal = -1, yOrdinal = -1, rOrdinal = -1;
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string name = reader.GetName(i);
                    if (name.Equals("Label", StringComparison.OrdinalIgnoreCase)) labelOrdinal = i;
                    else if (name.Equals("X", StringComparison.OrdinalIgnoreCase)) xOrdinal = i;
                    else if (name.Equals("Y", StringComparison.OrdinalIgnoreCase)) yOrdinal = i;
                    else if (name.Equals("R", StringComparison.OrdinalIgnoreCase)) rOrdinal = i;
                }

                if (xOrdinal == -1 || yOrdinal == -1)
                {
                    throw new Exception($"{report.ReportOrChartType} charts require X and Y columns in the query result.");
                }

                var pointDatasets = new Dictionary<string, PointDataset>();

                while (reader.Read())
                {
                    string seriesLabel = labelOrdinal >= 0 ? (reader[labelOrdinal]?.ToString() ?? report.ReportName ?? "") : (report.ReportName ?? "Series");

                    if (!pointDatasets.TryGetValue(seriesLabel, out var series))
                    {
                        series = new PointDataset { Label = seriesLabel };
                        pointDatasets[seriesLabel] = series;
                    }

                    series.Data.Add(new ChartPoint
                    {
                        X = Convert.ToDecimal(reader.GetValue(xOrdinal)),
                        Y = Convert.ToDecimal(reader.GetValue(yOrdinal)),
                        R = (isBubble && rOrdinal >= 0) ? Convert.ToDecimal(reader.GetValue(rOrdinal)) : null
                    });
                }

                return new LabelResultAndStyle
                {
                    Results = JsonConvert.SerializeObject(pointDatasets.Values, Formatting.Indented),
                    ColumnHeaders = JsonConvert.SerializeObject(Array.Empty<string>(), Formatting.Indented),
                    DisplayLegend = pointDatasets.Count > 1
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
                            case ReportChartType.Line:
                            case ReportChartType.Radar:
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
                    ReportChartType.HorizontalBar or ReportChartType.Bar or ReportChartType.Line or ReportChartType.Radar => new LabelResultAndStyle
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
        private class Dataset
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
        private class MultipleDataset
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
        private class ChartPoint
        {
            [JsonProperty("x")]
            public decimal X { get; set; }
            [JsonProperty("y")]
            public decimal Y { get; set; }
            [JsonProperty("r", NullValueHandling = NullValueHandling.Ignore)]
            public decimal? R { get; set; }
        }
        private class PointDataset
        {
            [JsonProperty("label")]
            public string? Label { get; set; }
            [JsonProperty("data")]
            public List<ChartPoint> Data { get; set; } = [];
            [JsonProperty("backgroundColor")]
            public string BackgroundColor { get; set; } = "";
            [JsonProperty("borderWidth")]
            public int BorderWidth { get; set; } = 1;
        }
    }
}