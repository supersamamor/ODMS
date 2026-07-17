using System.Data;
using System.Diagnostics;
using System.Text.Json;
using FBSC.Common.Data;
using FBSC.ODMS.Core.Constants;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FBSC.ODMS.Infrastructure.Services.Dashboard;

public record DashboardQueryResultColumnInfo(string ColumnName, int Ordinal, string SqlDataType);

public record DashboardQueryExecutionResult(
    bool Success,
    string? ErrorMessage,
    IReadOnlyList<DashboardQueryResultColumnInfo> Columns,
    IReadOnlyList<Dictionary<string, object?>> Rows,
    int DurationMs,
    bool IsTruncated)
{
    public int RowCount => Rows.Count;
}

/// <summary>
/// The single place a DashboardQuery's SQL text is ever executed. Every call is validated
/// SELECT-only, every parameter is bound as a real SqlParameter (SqlQueryText is never
/// string-concatenated with a value), every execution is time- and row-capped, and every
/// execution - success or failure - is written to the Audit log plus the query's own
/// denormalized last-execution fields.
/// </summary>
public class DashboardQueryExecutionService(
    ApplicationContext context,
    DataSourceConnectionFactory connectionFactory,
    SqlQueryValidator validator,
    IConfiguration configuration)
{
    private int StatementTimeoutSeconds => configuration.GetValue<int?>("DashboardEngine:StatementTimeoutSeconds") ?? 30;
    private int MaxRowCap => configuration.GetValue<int?>("DashboardEngine:MaxRowCap") ?? 5000;

    public async Task<DashboardQueryExecutionResult> ExecuteAsync(
        string dashboardQueryId,
        IReadOnlyDictionary<string, string?> parameterValues,
        string? userId,
        string? traceId,
        CancellationToken cancellationToken = default)
    {
        var query = await context.DashboardQuery
            .Include(q => q.DataSource)
            .Include(q => q.DashboardQueryParameterList)
            .SingleAsync(q => q.Id == dashboardQueryId, cancellationToken);

        return await ExecuteAsync(query, parameterValues, userId, traceId, cancellationToken);
    }

    public async Task<DashboardQueryExecutionResult> ExecuteAsync(
        DashboardQueryState query,
        IReadOnlyDictionary<string, string?> parameterValues,
        string? userId,
        string? traceId,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        var validation = validator.Validate(query.SqlQueryText);
        if (!validation.IsValid)
        {
            await RecordOutcomeAsync(query, success: false, validation.ErrorMessage, rowCount: 0, stopwatch, userId, traceId, cancellationToken);
            return new DashboardQueryExecutionResult(false, validation.ErrorMessage, [], [], (int)stopwatch.ElapsedMilliseconds, false);
        }

        if (query.DataSource is null)
        {
            const string error = "Query has no associated data source.";
            await RecordOutcomeAsync(query, success: false, error, rowCount: 0, stopwatch, userId, traceId, cancellationToken);
            return new DashboardQueryExecutionResult(false, error, [], [], (int)stopwatch.ElapsedMilliseconds, false);
        }

        try
        {
            await using var connection = await connectionFactory.OpenAsync(query.DataSource, cancellationToken);
            await using var command = new SqlCommand(query.SqlQueryText, connection)
            {
                CommandTimeout = StatementTimeoutSeconds,
                CommandType = CommandType.Text,
            };

            foreach (var declaredParameter in query.DashboardQueryParameterList ?? [])
            {
                var rawValue = parameterValues.TryGetValue(declaredParameter.ParameterName, out var v) && v is not null
                    ? v
                    : declaredParameter.DefaultValue;
                command.Parameters.Add(BuildSqlParameter(declaredParameter, rawValue));
            }

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            var columns = Enumerable.Range(0, reader.FieldCount)
                .Select(i => new DashboardQueryResultColumnInfo(reader.GetName(i), i, reader.GetDataTypeName(i)))
                .ToList();

            var rowCap = MaxRowCap;
            var rows = new List<Dictionary<string, object?>>();
            var isTruncated = false;
            while (await reader.ReadAsync(cancellationToken))
            {
                if (rows.Count >= rowCap)
                {
                    isTruncated = true;
                    break;
                }

                var row = new Dictionary<string, object?>(columns.Count);
                foreach (var column in columns)
                {
                    var value = reader.GetValue(column.Ordinal);
                    row[column.ColumnName] = value is DBNull ? null : value;
                }
                rows.Add(row);
            }

            await RecordOutcomeAsync(query, success: true, errorMessage: null, rows.Count, stopwatch, userId, traceId, cancellationToken);
            return new DashboardQueryExecutionResult(true, null, columns, rows, (int)stopwatch.ElapsedMilliseconds, isTruncated);
        }
        catch (Exception ex)
        {
            await RecordOutcomeAsync(query, success: false, ex.Message, rowCount: 0, stopwatch, userId, traceId, cancellationToken);
            return new DashboardQueryExecutionResult(false, ex.Message, [], [], (int)stopwatch.ElapsedMilliseconds, false);
        }
    }

    // internal (not private) so parameter-substitution logic is directly unit-testable
    // without opening a real SQL connection - see FBSC.ODMS.Tests.
    internal static SqlParameter BuildSqlParameter(DashboardQueryParameterState declaredParameter, string? rawValue)
    {
        var sqlDbType = declaredParameter.DataType.ToLowerInvariant() switch
        {
            "int" or "integer" => SqlDbType.Int,
            "decimal" or "numeric" or "money" => SqlDbType.Decimal,
            "bool" or "boolean" or "bit" => SqlDbType.Bit,
            "date" => SqlDbType.Date,
            "datetime" or "datetime2" => SqlDbType.DateTime2,
            _ => SqlDbType.NVarChar,
        };

        var parameter = new SqlParameter($"@{declaredParameter.ParameterName}", sqlDbType);
        if (string.IsNullOrEmpty(rawValue))
        {
            parameter.Value = DBNull.Value;
            return parameter;
        }

        parameter.Value = sqlDbType switch
        {
            SqlDbType.Int => int.Parse(rawValue),
            SqlDbType.Decimal => decimal.Parse(rawValue),
            SqlDbType.Bit => bool.Parse(rawValue),
            SqlDbType.Date or SqlDbType.DateTime2 => DateTime.Parse(rawValue),
            _ => rawValue,
        };
        return parameter;
    }

    private async Task RecordOutcomeAsync(
        DashboardQueryState query,
        bool success,
        string? errorMessage,
        int rowCount,
        Stopwatch stopwatch,
        string? userId,
        string? traceId,
        CancellationToken cancellationToken)
    {
        var durationMs = (int)stopwatch.ElapsedMilliseconds;
        var now = DateTime.UtcNow;

        var updated = query with
        {
            LastExecutedAt = now,
            LastExecutionDurationMs = durationMs,
            LastExecutionErrorRemarks = errorMessage,
            ValidationStatus = success ? QueryValidationStatus.Valid : QueryValidationStatus.Invalid,
            LastValidatedAt = now,
        };
        context.Entry(query).CurrentValues.SetValues(updated);

        context.AuditLogs.Add(new Audit
        {
            UserId = userId,
            TraceId = traceId,
            Type = "Execute",
            TableName = nameof(DashboardQueryState).Replace("State", ""),
            PrimaryKey = query.Id,
            DateTime = now,
            NewValues = JsonSerializer.Serialize(new
            {
                Status = success ? "Success" : "Failed",
                DurationMs = durationMs,
                RowCount = rowCount,
                ErrorMessage = errorMessage,
            }),
        });

        await context.SaveChangesAsync(cancellationToken);
    }
}
