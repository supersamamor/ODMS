using System.Data;
using CsvHelper;
using CsvHelper.Configuration;
using FBSC.ODMS.Core.Constants;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using System.Globalization;
using System.IO.Compression;

namespace FBSC.ODMS.Infrastructure.Services.Dashboard;

public record ParsedColumn(string OriginalHeader, string SanitizedName, string InferredSqlType, string? SampleValue);

public record ParsedSheet(string SheetName, IReadOnlyList<ParsedColumn> Columns, IReadOnlyList<object?[]> Rows, bool IsTruncated);

public record ParsedFile(IReadOnlyList<ParsedSheet> Sheets, string Format);

public record IngestedSheetResult(string SheetName, bool Success, int RowCount, int ColumnCount, string? ErrorRemarks, string DataUploadBatchId);

/// <summary>
/// Parses an uploaded CSV/XLSX/XLS file into one or more sheets, sanitizes headers, infers
/// column types, and stages each sheet as its own SQL Server table under the `uploads` schema
/// - registering it in DataSourceSchemaCacheState exactly like live schema discovery does, so
/// it's immediately query-able through the existing query builder/field-mapping UI with zero
/// special-casing downstream. Every identifier that reaches generated DDL/DML here is either a
/// sanitized header (UploadedFileHeaderSanitizer) or a name this code itself constructs -
/// never raw user input.
/// </summary>
public class UploadedFileIngestionService(ApplicationContext context, IConfiguration configuration)
{
    private const string StagingSchema = "uploads";

    private long MaxFileSizeBytes => configuration.GetValue<long?>("DashboardEngine:UploadMaxFileSizeBytes") ?? 25 * 1024 * 1024;
    private int MaxRowCount => configuration.GetValue<int?>("DashboardEngine:UploadMaxRowCount") ?? 200_000;
    private string ApplicationConnectionString => configuration.GetConnectionString("ApplicationContext")!;

    public static string DetectFormat(string fileName)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        return ext switch
        {
            ".csv" => UploadedFileFormat.Csv,
            ".xlsx" => UploadedFileFormat.Xlsx,
            ".xls" => UploadedFileFormat.Xls,
            _ => throw new NotSupportedException($"Unsupported file extension '{ext}'. Only .csv, .xlsx, and .xls are accepted."),
        };
    }

    /// <summary>
    /// Parses the file only - no database writes. Used to build the upload preview (detected
    /// sheets, inferred columns/types, sample rows) before the user confirms.
    /// </summary>
    public ParsedFile Parse(byte[] fileBytes, string fileName, string? csvDelimiterOverride = null)
    {
        if (fileBytes.LongLength > MaxFileSizeBytes)
        {
            throw new InvalidOperationException($"File exceeds the maximum allowed size of {MaxFileSizeBytes / (1024 * 1024)} MB.");
        }

        var format = DetectFormat(fileName);
        return format switch
        {
            UploadedFileFormat.Csv => new ParsedFile([ParseCsv(fileBytes, Path.GetFileNameWithoutExtension(fileName), csvDelimiterOverride)], format),
            UploadedFileFormat.Xlsx => new ParsedFile(ParseXlsx(fileBytes, includedSheetNames: null), format),
            UploadedFileFormat.Xls => throw new NotSupportedException("Legacy .xls files aren't supported yet - please re-save as .xlsx or .csv and upload again."),
            _ => throw new NotSupportedException($"Unsupported format '{format}'."),
        };
    }

    /// <summary>
    /// Confirms an upload: stages each included sheet as its own table and registers it in
    /// DataSourceSchemaCacheState. Re-running for an existing DataSourceId+SheetName replaces
    /// that sheet's staged data atomically (build-new -> swap -> drop-old) so a failed
    /// re-upload never clobbers previously-working data.
    /// </summary>
    public async Task<IReadOnlyList<IngestedSheetResult>> IngestAsync(
        string dataSourceId,
        byte[] fileBytes,
        string fileName,
        string? uploadedByUserId,
        IReadOnlySet<string>? includedSheetNames,
        string? csvDelimiterOverride,
        CancellationToken cancellationToken = default)
    {
        var parsed = Parse(fileBytes, fileName, csvDelimiterOverride);
        var results = new List<IngestedSheetResult>();

        foreach (var sheet in parsed.Sheets)
        {
            if (includedSheetNames is not null && !includedSheetNames.Contains(sheet.SheetName))
            {
                continue;
            }
            results.Add(await IngestSheetAsync(dataSourceId, fileName, parsed.Format, sheet, uploadedByUserId, cancellationToken));
        }

        return results;
    }

    private async Task<IngestedSheetResult> IngestSheetAsync(
        string dataSourceId,
        string fileName,
        string format,
        ParsedSheet sheet,
        string? uploadedByUserId,
        CancellationToken cancellationToken)
    {
        var batch = await context.DataUploadBatch
            .SingleOrDefaultAsync(b => b.DataSourceId == dataSourceId && b.SheetName == sheet.SheetName, cancellationToken);

        if (batch is null)
        {
            batch = new DataUploadBatchState
            {
                DataSourceId = dataSourceId,
                FileName = fileName,
                SheetName = sheet.SheetName,
                FileType = format,
                UploadedBy = uploadedByUserId,
                ImportStatus = UploadedFileImportStatus.Parsing,
            };
            context.DataUploadBatch.Add(batch);
        }
        else
        {
            var updated = batch with
            {
                FileName = fileName,
                FileType = format,
                UploadedBy = uploadedByUserId,
                ImportStatus = UploadedFileImportStatus.Parsing,
                ErrorRemarks = null,
            };
            context.Entry(batch).CurrentValues.SetValues(updated);
        }
        await context.SaveChangesAsync(cancellationToken);

        var finalTableName = $"stg_{batch.Id.Replace("-", "")}";
        var stagingTableName = $"{StagingSchema}.{finalTableName}";

        try
        {
            await StageSheetAtomicallyAsync(finalTableName, sheet, cancellationToken);
            await UpsertSchemaCacheAsync(dataSourceId, sheet, stagingTableName, cancellationToken);
            await UpsertUploadColumnsAsync(batch.Id, sheet, cancellationToken);

            var readyValues = batch with
            {
                StagingTableName = stagingTableName,
                RowCount = sheet.Rows.Count,
                ColumnCount = sheet.Columns.Count,
                ImportStatus = UploadedFileImportStatus.Ready,
                ImportedAt = DateTime.UtcNow,
                ErrorRemarks = sheet.IsTruncated ? $"Row cap of {MaxRowCount:N0} reached - file had more rows than were imported." : null,
            };
            context.Entry(batch).CurrentValues.SetValues(readyValues);
            await context.SaveChangesAsync(cancellationToken);

            return new IngestedSheetResult(sheet.SheetName, true, sheet.Rows.Count, sheet.Columns.Count, readyValues.ErrorRemarks, batch.Id);
        }
        catch (Exception ex)
        {
            var failedValues = batch with
            {
                ImportStatus = UploadedFileImportStatus.Failed,
                ErrorRemarks = ex.Message,
            };
            context.Entry(batch).CurrentValues.SetValues(failedValues);
            await context.SaveChangesAsync(cancellationToken);
            return new IngestedSheetResult(sheet.SheetName, false, 0, 0, ex.Message, batch.Id);
        }
    }

    /// <summary>
    /// Builds the new table under a temporary name, bulk-loads it, and only swaps it in for
    /// the real name once fully loaded - so a failure partway through leaves any previously
    /// staged (working) table for this sheet completely untouched.
    /// </summary>
    private async Task StageSheetAtomicallyAsync(string finalTableName, ParsedSheet sheet, CancellationToken cancellationToken)
    {
        var tempTableName = $"{finalTableName}_new";

        await using var connection = new SqlConnection(ApplicationConnectionString);
        await connection.OpenAsync(cancellationToken);

        await EnsureSchemaExistsAsync(connection, cancellationToken);
        await DropTableIfExistsAsync(connection, tempTableName, cancellationToken);
        await CreateStagingTableAsync(connection, tempTableName, sheet.Columns, cancellationToken);
        await BulkLoadAsync(connection, tempTableName, sheet, cancellationToken);

        await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync(cancellationToken);
        try
        {
            await DropTableIfExistsAsync(connection, finalTableName, cancellationToken, transaction);
            await using (var rename = new SqlCommand(
                "EXEC sp_rename @OldName, @NewName",
                connection, transaction))
            {
                rename.Parameters.AddWithValue("@OldName", $"{StagingSchema}.{tempTableName}");
                rename.Parameters.AddWithValue("@NewName", finalTableName);
                await rename.ExecuteNonQueryAsync(cancellationToken);
            }
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            await DropTableIfExistsAsync(connection, tempTableName, cancellationToken);
            throw;
        }
    }

    private static async Task EnsureSchemaExistsAsync(SqlConnection connection, CancellationToken cancellationToken)
    {
        const string sql = $"""
            IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = '{StagingSchema}')
                EXEC('CREATE SCHEMA {StagingSchema}');
            """;
        await using var command = new SqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static async Task DropTableIfExistsAsync(SqlConnection connection, string tableName, CancellationToken cancellationToken, SqlTransaction? transaction = null)
    {
        // tableName is either sanitizer-derived or constructed by this class from a batch's
        // own Id - never raw user input - so bracket-quoting it directly here is safe.
        var sql = $"IF OBJECT_ID('{StagingSchema}.[{tableName}]', 'U') IS NOT NULL DROP TABLE {StagingSchema}.[{tableName}];";
        await using var command = new SqlCommand(sql, connection, transaction);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static async Task CreateStagingTableAsync(SqlConnection connection, string tableName, IReadOnlyList<ParsedColumn> columns, CancellationToken cancellationToken)
    {
        var columnDefinitions = string.Join(", ", columns.Select(c => $"[{c.SanitizedName}] {c.InferredSqlType} NULL"));
        var sql = $"CREATE TABLE {StagingSchema}.[{tableName}] ({columnDefinitions});";
        await using var command = new SqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static async Task BulkLoadAsync(SqlConnection connection, string tableName, ParsedSheet sheet, CancellationToken cancellationToken)
    {
        var dataTable = new DataTable();
        foreach (var column in sheet.Columns)
        {
            dataTable.Columns.Add(column.SanitizedName, UploadedColumnTypeInferrer.ToClrType(column.InferredSqlType));
        }
        foreach (var row in sheet.Rows)
        {
            dataTable.Rows.Add(row);
        }

        using var bulkCopy = new SqlBulkCopy(connection)
        {
            DestinationTableName = $"{StagingSchema}.[{tableName}]",
            BulkCopyTimeout = 120,
        };
        foreach (DataColumn column in dataTable.Columns)
        {
            bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
        }
        await bulkCopy.WriteToServerAsync(dataTable, cancellationToken);
    }

    private async Task UpsertSchemaCacheAsync(string dataSourceId, ParsedSheet sheet, string stagingTableName, CancellationToken cancellationToken)
    {
        var schemaName = StagingSchema;
        var tableName = stagingTableName[(StagingSchema.Length + 1)..];

        var existing = await context.DataSourceSchemaCache
            .Where(s => s.DataSourceId == dataSourceId && s.SchemaName == schemaName && s.TableName == tableName)
            .ToListAsync(cancellationToken);
        context.DataSourceSchemaCache.RemoveRange(existing);

        var now = DateTime.UtcNow;
        for (var i = 0; i < sheet.Columns.Count; i++)
        {
            var column = sheet.Columns[i];
            context.DataSourceSchemaCache.Add(new DataSourceSchemaCacheState
            {
                DataSourceId = dataSourceId,
                SchemaName = schemaName,
                TableName = tableName,
                ColumnName = column.SanitizedName,
                SqlDataType = column.InferredSqlType,
                OrdinalPosition = i + 1,
                IsNullable = true,
                InferredSemanticType = DataSourceSchemaDiscoveryService.InferSemanticType(column.SanitizedName, column.InferredSqlType),
                RefreshedAt = now,
            });
        }
        await context.SaveChangesAsync(cancellationToken);
    }

    private async Task UpsertUploadColumnsAsync(string dataUploadBatchId, ParsedSheet sheet, CancellationToken cancellationToken)
    {
        var existing = await context.DataUploadColumn
            .Where(c => c.DataUploadBatchId == dataUploadBatchId)
            .ToListAsync(cancellationToken);
        context.DataUploadColumn.RemoveRange(existing);

        for (var i = 0; i < sheet.Columns.Count; i++)
        {
            var column = sheet.Columns[i];
            context.DataUploadColumn.Add(new DataUploadColumnState
            {
                DataUploadBatchId = dataUploadBatchId,
                ColumnName = column.SanitizedName,
                DetectedDataType = column.InferredSqlType,
                MappedSqlDataType = column.InferredSqlType,
                OrdinalPosition = i + 1,
                SampleValue = column.SampleValue,
            });
        }
        await context.SaveChangesAsync(cancellationToken);
    }

    // -------------------- Parsers --------------------

    private ParsedSheet ParseCsv(byte[] fileBytes, string sheetName, string? delimiterOverride)
    {
        using var stream = new MemoryStream(fileBytes);
        using var reader = new StreamReader(stream, detectEncodingFromByteOrderMarks: true);

        var delimiter = delimiterOverride ?? DetectCsvDelimiter(fileBytes);
        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = delimiter,
            HasHeaderRecord = true,
            BadDataFound = null,
            MissingFieldFound = null,
        };
        using var csv = new CsvReader(reader, csvConfig);

        if (!csv.Read() || !csv.ReadHeader() || csv.HeaderRecord is null)
        {
            return new ParsedSheet(sheetName, [], [], false);
        }

        var sanitizedNames = UploadedFileHeaderSanitizer.Sanitize(csv.HeaderRecord);
        var fieldCount = csv.HeaderRecord.Length;
        var rawColumnValues = new List<string?>[fieldCount];
        for (var i = 0; i < fieldCount; i++)
        {
            rawColumnValues[i] = [];
        }

        var rawRows = new List<string?[]>();
        var isTruncated = false;
        while (csv.Read())
        {
            if (rawRows.Count >= MaxRowCount)
            {
                isTruncated = true;
                break;
            }
            var row = new string?[fieldCount];
            for (var i = 0; i < fieldCount; i++)
            {
                var value = csv.TryGetField<string>(i, out var field) ? field : null;
                row[i] = value;
                rawColumnValues[i].Add(value);
            }
            rawRows.Add(row);
        }

        var columns = BuildParsedColumns(csv.HeaderRecord, sanitizedNames, rawColumnValues);
        var typedRows = ConvertRows(rawRows, columns);
        return new ParsedSheet(sheetName, columns, typedRows, isTruncated);
    }

    private static string DetectCsvDelimiter(byte[] fileBytes)
    {
        using var stream = new MemoryStream(fileBytes);
        using var reader = new StreamReader(stream, detectEncodingFromByteOrderMarks: true);
        var firstLine = reader.ReadLine() ?? "";
        var candidates = new[] { ',', ';', '\t' };
        var best = candidates
            .Select(c => (Delimiter: c, Count: firstLine.Count(ch => ch == c)))
            .OrderByDescending(x => x.Count)
            .First();
        return best.Count > 0 ? best.Delimiter.ToString() : ",";
    }

    private List<ParsedSheet> ParseXlsx(byte[] fileBytes, IReadOnlySet<string>? includedSheetNames)
    {
        using var stream = new MemoryStream(fileBytes);
        if (ContainsVbaProject(stream))
        {
            throw new NotSupportedException("Macro-enabled workbooks aren't accepted - please save as a macro-free .xlsx and upload again.");
        }
        stream.Position = 0;

        using var package = new ExcelPackage(stream);
        var sheets = new List<ParsedSheet>();

        foreach (var worksheet in package.Workbook.Worksheets)
        {
            if (worksheet.Hidden != OfficeOpenXml.eWorkSheetHidden.Visible)
            {
                continue;
            }
            if (includedSheetNames is not null && !includedSheetNames.Contains(worksheet.Name))
            {
                continue;
            }
            if (worksheet.Dimension is null)
            {
                continue;
            }

            var columnCount = worksheet.Dimension.Columns;
            var totalRows = worksheet.Dimension.Rows;

            var rawHeaders = new string?[columnCount];
            for (var c = 1; c <= columnCount; c++)
            {
                // .Text reads the cached/displayed value only - EPPlus does not execute
                // formulas or macros, so this can never trigger formula evaluation.
                rawHeaders[c - 1] = worksheet.Cells[1, c]?.Text;
            }
            var sanitizedNames = UploadedFileHeaderSanitizer.Sanitize(rawHeaders);

            var rawColumnValues = new List<string?>[columnCount];
            for (var i = 0; i < columnCount; i++)
            {
                rawColumnValues[i] = [];
            }

            var rawRows = new List<string?[]>();
            var isTruncated = false;
            for (var r = 2; r <= totalRows; r++)
            {
                if (rawRows.Count >= MaxRowCount)
                {
                    isTruncated = true;
                    break;
                }
                var row = new string?[columnCount];
                for (var c = 1; c <= columnCount; c++)
                {
                    var value = worksheet.Cells[r, c]?.Text;
                    row[c - 1] = string.IsNullOrEmpty(value) ? null : value;
                    rawColumnValues[c - 1].Add(row[c - 1]);
                }
                rawRows.Add(row);
            }

            var columns = BuildParsedColumns(rawHeaders, sanitizedNames, rawColumnValues);
            var typedRows = ConvertRows(rawRows, columns);
            sheets.Add(new ParsedSheet(worksheet.Name, columns, typedRows, isTruncated));
        }

        return sheets;
    }

    private static bool ContainsVbaProject(Stream data)
    {
        try
        {
            data.Position = 0;
            using var zip = new ZipArchive(data, ZipArchiveMode.Read, leaveOpen: true);
            return zip.Entries.Any(e => e.FullName.EndsWith("vbaProject.bin", StringComparison.OrdinalIgnoreCase));
        }
        catch
        {
            return false;
        }
    }

    private static IReadOnlyList<ParsedColumn> BuildParsedColumns(IReadOnlyList<string?> rawHeaders, IReadOnlyList<string> sanitizedNames, IReadOnlyList<List<string?>> columnValues)
    {
        var columns = new List<ParsedColumn>(sanitizedNames.Count);
        for (var i = 0; i < sanitizedNames.Count; i++)
        {
            var inferredType = UploadedColumnTypeInferrer.InferSqlType(columnValues[i]);
            var sample = columnValues[i].FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));
            columns.Add(new ParsedColumn(rawHeaders[i] ?? "", sanitizedNames[i], inferredType, sample));
        }
        return columns;
    }

    private static List<object?[]> ConvertRows(List<string?[]> rawRows, IReadOnlyList<ParsedColumn> columns)
    {
        var result = new List<object?[]>(rawRows.Count);
        foreach (var rawRow in rawRows)
        {
            var typedRow = new object?[columns.Count];
            for (var i = 0; i < columns.Count; i++)
            {
                typedRow[i] = ConvertValue(rawRow[i], columns[i].InferredSqlType);
            }
            result.Add(typedRow);
        }
        return result;
    }

    private static object? ConvertValue(string? rawValue, string sqlType)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            return DBNull.Value;
        }
        return sqlType switch
        {
            "int" => int.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i) ? i : DBNull.Value,
            "decimal(18,4)" => decimal.TryParse(rawValue, NumberStyles.Number, CultureInfo.InvariantCulture, out var d) ? d : DBNull.Value,
            "datetime2" => DateTime.TryParse(rawValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt) ? dt : DBNull.Value,
            _ => rawValue,
        };
    }
}
