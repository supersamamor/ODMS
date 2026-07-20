using Microsoft.Data.SqlClient;
using OfficeOpenXml;
using System.Data;
using System.Text.RegularExpressions;

namespace FBSC.ODMS.ExcelProcessor.Services
{
    /// <summary>
    /// Parses an uploaded Excel workbook with unknown/arbitrary columns and materializes it as a
    /// real, dynamically-created SQL table (DROP + CREATE + bulk copy) so it can be queried with
    /// plain T-SQL by the reporting engine, the same way any other SQL-backed data source is.
    /// </summary>
    public class DynamicTableImportService
    {
        private const int MaxSampleRows = 1000;
        private const int TextColumnLength = 450;

        public async Task<DynamicTableImportResult> ImportAsDynamicTableAsync(
            string filePath,
            string tableName,
            string applicationConnectionString,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var (columns, table) = ReadWorkbook(filePath);
                if (columns.Count == 0)
                {
                    return DynamicTableImportResult.Failure("The uploaded file has no header row or no data columns to import.");
                }

                using SqlConnection connection = new(applicationConnectionString);
                await connection.OpenAsync(cancellationToken);

                await ExecuteNonQueryAsync(connection, $"DROP TABLE IF EXISTS [dbo].[{tableName}]", cancellationToken);
                await ExecuteNonQueryAsync(connection, BuildCreateTableSql(tableName, columns), cancellationToken);

                using SqlBulkCopy bulkCopy = new(connection)
                {
                    DestinationTableName = $"[dbo].[{tableName}]",
                    BulkCopyTimeout = 0,
                    BatchSize = 5000
                };
                foreach (var column in columns)
                {
                    bulkCopy.ColumnMappings.Add(column.Name, column.Name);
                }
                await bulkCopy.WriteToServerAsync(table, cancellationToken);

                return DynamicTableImportResult.Ok(tableName);
            }
            catch (Exception ex)
            {
                return DynamicTableImportResult.Failure(ex.Message);
            }
        }

        private static (List<InferredColumn> Columns, DataTable Table) ReadWorkbook(string filePath)
        {
            using var stream = new MemoryStream(File.ReadAllBytes(filePath));
            using var package = new ExcelPackage(stream);
            ExcelWorksheet workSheet = package.Workbook.Worksheets[0];
            var columnCount = workSheet.Dimension?.Columns ?? 0;
            var rowCount = workSheet.Dimension?.Rows ?? 0;
            if (columnCount == 0 || rowCount < 2)
            {
                return ([], new DataTable());
            }

            var rawHeaders = new List<string>(columnCount);
            for (int c = 1; c <= columnCount; c++)
            {
                rawHeaders.Add(workSheet.Cells[1, c]?.Value?.ToString() ?? "");
            }
            var columnNames = SanitizeHeaders(rawHeaders);

            // Read every data row once, up front, so type inference and the bulk-copy DataTable
            // share a single pass over the file instead of re-parsing it twice.
            var rawRows = new List<string?[]>(rowCount - 1);
            for (int r = 2; r <= rowCount; r++)
            {
                var row = new string?[columnCount];
                for (int c = 1; c <= columnCount; c++)
                {
                    var value = workSheet.Cells[r, c]?.Value?.ToString();
                    row[c - 1] = string.IsNullOrEmpty(value) ? null : value;
                }
                rawRows.Add(row);
            }

            var columns = InferColumns(columnNames, rawRows);

            var table = new DataTable();
            foreach (var column in columns)
            {
                table.Columns.Add(column.Name, ClrTypeFor(column.Type));
            }
            foreach (var row in rawRows)
            {
                var dataRow = table.NewRow();
                for (int c = 0; c < columns.Count; c++)
                {
                    dataRow[c] = ConvertCell(row[c], columns[c].Type);
                }
                table.Rows.Add(dataRow);
            }

            return (columns, table);
        }

        private static List<InferredColumn> InferColumns(List<string> columnNames, List<string?[]> rawRows)
        {
            var columns = new List<InferredColumn>(columnNames.Count);
            for (int c = 0; c < columnNames.Count; c++)
            {
                bool canBeBigInt = true, canBeDecimal = true, canBeDateTime = true, canBeBit = true, sawAnyValue = false;
                int maxTextLength = 0, sampled = 0;
                foreach (var row in rawRows)
                {
                    var value = row[c];
                    if (value == null) continue;
                    sawAnyValue = true;
                    maxTextLength = Math.Max(maxTextLength, value.Length);
                    if (canBeBigInt && !long.TryParse(value, out _)) canBeBigInt = false;
                    if (canBeDecimal && !decimal.TryParse(value, out _)) canBeDecimal = false;
                    if (canBeDateTime && !DateTime.TryParse(value, out _)) canBeDateTime = false;
                    if (canBeBit && !IsBitLike(value)) canBeBit = false;
                    if (++sampled >= MaxSampleRows) break;
                }
                var type = !sawAnyValue ? InferredType.Text
                    : canBeBigInt ? InferredType.BigInt
                    : canBeDecimal ? InferredType.Decimal
                    : canBeDateTime ? InferredType.DateTime
                    : canBeBit ? InferredType.Bit
                    : InferredType.Text;
                columns.Add(new InferredColumn(columnNames[c], type, maxTextLength));
            }
            return columns;
        }

        private static bool IsBitLike(string value) =>
            value.Equals("true", StringComparison.OrdinalIgnoreCase) ||
            value.Equals("false", StringComparison.OrdinalIgnoreCase) ||
            value.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
            value.Equals("no", StringComparison.OrdinalIgnoreCase);

        private static object ConvertCell(string? value, InferredType type)
        {
            if (value == null)
            {
                return DBNull.Value;
            }
            try
            {
                return type switch
                {
                    InferredType.BigInt => long.Parse(value),
                    InferredType.Decimal => decimal.Parse(value),
                    InferredType.DateTime => DateTime.Parse(value),
                    InferredType.Bit => value.Equals("true", StringComparison.OrdinalIgnoreCase) || value.Equals("yes", StringComparison.OrdinalIgnoreCase),
                    _ => value
                };
            }
            catch
            {
                // A single malformed cell (beyond the inference sample, or an inconsistent
                // outlier within it) must not fail the whole import — store it as null.
                return DBNull.Value;
            }
        }

        private static Type ClrTypeFor(InferredType type) => type switch
        {
            InferredType.BigInt => typeof(long),
            InferredType.Decimal => typeof(decimal),
            InferredType.DateTime => typeof(DateTime),
            InferredType.Bit => typeof(bool),
            _ => typeof(string)
        };

        private static string SqlTypeFor(InferredColumn column) => column.Type switch
        {
            InferredType.BigInt => "bigint",
            InferredType.Decimal => "decimal(18,4)",
            InferredType.DateTime => "datetime2",
            InferredType.Bit => "bit",
            _ => column.MaxTextLength > TextColumnLength ? "nvarchar(max)" : $"nvarchar({TextColumnLength})"
        };

        private static string BuildCreateTableSql(string tableName, List<InferredColumn> columns)
        {
            var columnDefinitions = string.Join(",\n    ", columns.Select(c => $"[{c.Name}] {SqlTypeFor(c)} NULL"));
            return $"CREATE TABLE [dbo].[{tableName}] (\n    {columnDefinitions}\n)";
        }

        private static List<string> SanitizeHeaders(List<string> rawHeaders)
        {
            var used = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var result = new List<string>(rawHeaders.Count);
            for (int i = 0; i < rawHeaders.Count; i++)
            {
                var sanitized = Regex.Replace(rawHeaders[i] ?? "", @"[^A-Za-z0-9_]+", "_").Trim('_');
                if (string.IsNullOrEmpty(sanitized) || char.IsDigit(sanitized[0]))
                {
                    sanitized = "Col_" + (string.IsNullOrEmpty(sanitized) ? (i + 1).ToString() : sanitized);
                }
                if (sanitized.Length > 128)
                {
                    sanitized = sanitized[..128];
                }
                var candidate = sanitized;
                int suffix = 2;
                while (!used.Add(candidate))
                {
                    candidate = $"{sanitized}_{suffix}";
                    if (candidate.Length > 128)
                    {
                        candidate = candidate[..(128 - suffix.ToString().Length - 1)] + $"_{suffix}";
                    }
                    suffix++;
                }
                result.Add(candidate);
            }
            return result;
        }

        private static async Task ExecuteNonQueryAsync(SqlConnection connection, string sql, CancellationToken cancellationToken)
        {
            using SqlCommand command = new(sql, connection) { CommandTimeout = 120 };
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        private enum InferredType { BigInt, Decimal, DateTime, Bit, Text }

        private sealed record InferredColumn(string Name, InferredType Type, int MaxTextLength);
    }

    public sealed class DynamicTableImportResult
    {
        public bool Success { get; private init; }
        public string? TableName { get; private init; }
        public string? ErrorMessage { get; private init; }
        public static DynamicTableImportResult Ok(string tableName) => new() { Success = true, TableName = tableName };
        public static DynamicTableImportResult Failure(string errorMessage) => new() { Success = false, ErrorMessage = errorMessage };
    }
}
