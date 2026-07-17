using FBSC.ODMS.Core.Constants;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FBSC.ODMS.Infrastructure.Services.Dashboard;

/// <summary>
/// Connects to a DataSourceState, reads INFORMATION_SCHEMA, and caches the result as
/// DataSourceSchemaCacheState rows so the query builder/AI prompt can suggest valid
/// tables/columns without hitting the source live on every request.
/// </summary>
public class DataSourceSchemaDiscoveryService(ApplicationContext context, DataSourceConnectionFactory connectionFactory)
{
    private const string InformationSchemaQuery = """
        SELECT
            c.TABLE_SCHEMA, c.TABLE_NAME, c.COLUMN_NAME, c.DATA_TYPE, c.ORDINAL_POSITION,
            CASE WHEN c.IS_NULLABLE = 'YES' THEN 1 ELSE 0 END AS IS_NULLABLE
        FROM INFORMATION_SCHEMA.COLUMNS c
        INNER JOIN INFORMATION_SCHEMA.TABLES t
            ON t.TABLE_SCHEMA = c.TABLE_SCHEMA AND t.TABLE_NAME = c.TABLE_NAME
        WHERE t.TABLE_TYPE IN ('BASE TABLE', 'VIEW')
        ORDER BY c.TABLE_SCHEMA, c.TABLE_NAME, c.ORDINAL_POSITION
        """;

    public async Task<int> DiscoverAsync(string dataSourceId, CancellationToken cancellationToken = default)
    {
        var dataSource = await context.DataSource.SingleAsync(d => d.Id == dataSourceId, cancellationToken);

        // An UploadedFile DataSource's schema is managed entirely by UploadedFileIngestionService
        // (one DataSourceSchemaCacheState row set per uploaded sheet). Running the generic
        // INFORMATION_SCHEMA scan against it would connect to this app's own database (where
        // staging tables live - see DataSourceConnectionFactory) and pick up every other table
        // in the app, not just this DataSource's staging tables.
        if (dataSource.ConnectionKind == Core.Constants.DataSourceConnectionKind.UploadedFile)
        {
            throw new InvalidOperationException("Schema discovery doesn't apply to an UploadedFile data source - upload or re-upload a file instead.");
        }

        var discovered = new List<DataSourceSchemaCacheState>();
        await using (var connection = await connectionFactory.OpenAsync(dataSource, cancellationToken))
        await using (var command = new SqlCommand(InformationSchemaQuery, connection) { CommandTimeout = 30 })
        await using (var reader = await command.ExecuteReaderAsync(cancellationToken))
        {
            while (await reader.ReadAsync(cancellationToken))
            {
                var sqlDataType = reader.GetString(reader.GetOrdinal("DATA_TYPE"));
                var columnName = reader.GetString(reader.GetOrdinal("COLUMN_NAME"));
                discovered.Add(new DataSourceSchemaCacheState
                {
                    DataSourceId = dataSourceId,
                    SchemaName = reader.GetString(reader.GetOrdinal("TABLE_SCHEMA")),
                    TableName = reader.GetString(reader.GetOrdinal("TABLE_NAME")),
                    ColumnName = columnName,
                    SqlDataType = sqlDataType,
                    OrdinalPosition = reader.GetInt32(reader.GetOrdinal("ORDINAL_POSITION")),
                    IsNullable = reader.GetInt32(reader.GetOrdinal("IS_NULLABLE")) == 1,
                    InferredSemanticType = InferSemanticType(columnName, sqlDataType),
                    RefreshedAt = DateTime.UtcNow,
                });
            }
        }

        var existing = await context.DataSourceSchemaCache
            .Where(s => s.DataSourceId == dataSourceId)
            .ToListAsync(cancellationToken);
        var existingByKey = existing.ToDictionary(Key, e => e);
        var discoveredKeys = discovered.Select(Key).ToHashSet();

        context.DataSourceSchemaCache.RemoveRange(existing.Where(e => !discoveredKeys.Contains(Key(e))));

        foreach (var row in discovered)
        {
            if (existingByKey.TryGetValue(Key(row), out var existingRow))
            {
                context.Entry(existingRow).CurrentValues.SetValues(row with { Id = existingRow.Id });
            }
            else
            {
                context.DataSourceSchemaCache.Add(row);
            }
        }

        return await context.SaveChangesAsync(cancellationToken);
    }

    private static string Key(DataSourceSchemaCacheState s) => $"{s.SchemaName}.{s.TableName}.{s.ColumnName}";

    /// <summary>
    /// Semantic-type heuristic (tunable): identifier if the column name looks like a key
    /// (exactly "Id", or ends with "Id"/"Code"/"Guid"/"Key"), else Date for
    /// date/time-family SQL types, else Measure for numeric SQL types, else Dimension for
    /// everything else (strings, bit flags, etc).
    /// </summary>
    public static string InferSemanticType(string columnName, string sqlDataType)
    {
        var name = columnName.Trim();
        var isIdentifierName = name.Equals("Id", StringComparison.OrdinalIgnoreCase)
            || name.EndsWith("Id", StringComparison.OrdinalIgnoreCase)
            || name.EndsWith("Code", StringComparison.OrdinalIgnoreCase)
            || name.EndsWith("Guid", StringComparison.OrdinalIgnoreCase)
            || name.EndsWith("Key", StringComparison.OrdinalIgnoreCase);

        var type = sqlDataType.Trim().ToLowerInvariant();
        var isDateType = type is "date" or "datetime" or "datetime2" or "smalldatetime" or "datetimeoffset" or "time";
        var isNumericType = type is "int" or "bigint" or "smallint" or "tinyint" or "decimal" or "numeric"
            or "float" or "real" or "money" or "smallmoney" or "bit";

        if (isDateType)
        {
            return SemanticType.Date;
        }
        if (isIdentifierName)
        {
            return SemanticType.Identifier;
        }
        if (isNumericType)
        {
            return SemanticType.Measure;
        }
        return SemanticType.Dimension;
    }
}
