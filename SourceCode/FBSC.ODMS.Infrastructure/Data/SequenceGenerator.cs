using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace FBSC.ODMS.Infrastructure.Data;

/// <summary>
/// Thread-safe, gap-free issuer of monotonic sequence numbers backed by the
/// SequenceCounter table. The whole reserve-and-increment is a single SQL
/// statement guarded by <c>UPDLOCK, SERIALIZABLE</c>, so:
/// <list type="bullet">
///   <item>concurrent callers for the same key serialize on that one row (an
///     exclusive row lock held only for the statement's duration);</item>
///   <item>different keys never touch the same row, so unrelated inserts run
///     fully in parallel with no contention and no deadlock (single resource,
///     consistent lock order);</item>
///   <item>the increment enlists in the caller's ambient EF transaction, so if
///     the insert that consumes the number rolls back, the counter rolls back
///     too - no gaps, no duplicates.</item>
/// </list>
/// For bulk imports, <see cref="ReserveAsync"/> hands out a contiguous block in
/// one round-trip so many rows can be code-stamped in memory and then inserted
/// without a per-row database call.
/// Lives in Infrastructure so both the Application command handlers and the
/// Scheduler batch job (which does not reference Application) can use it.
/// </summary>
public static class SequenceGenerator
{
    private const string ReserveSql = @"
SET NOCOUNT ON;
DECLARE @last bigint;
UPDATE dbo.SequenceCounter WITH (UPDLOCK, SERIALIZABLE)
   SET @last = [Value] = [Value] + @inc
 WHERE [Key] = @key;
IF @@ROWCOUNT = 0
BEGIN
   INSERT dbo.SequenceCounter([Key], [Value]) VALUES (@key, @inc);
   SET @last = @inc;
END
SELECT @last;";

    /// <summary>Issues the next single value for <paramref name="key"/>.</summary>
    public static Task<long> NextAsync(ApplicationContext context, string key, CancellationToken cancellationToken = default)
        => ReserveAsync(context, key, 1, cancellationToken);

    /// <summary>
    /// Reserves a contiguous block of <paramref name="count"/> values for
    /// <paramref name="key"/> and returns the FIRST value of the block
    /// (values are start .. start + count - 1).
    /// </summary>
    public static async Task<long> ReserveAsync(ApplicationContext context, string key, int count, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentOutOfRangeException.ThrowIfLessThan(count, 1);

        var connection = context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        await using var command = connection.CreateCommand();
        command.Transaction = context.Database.CurrentTransaction?.GetDbTransaction();
        command.CommandText = ReserveSql;

        var keyParam = command.CreateParameter();
        keyParam.ParameterName = "@key";
        keyParam.Value = key;
        command.Parameters.Add(keyParam);

        var incParam = command.CreateParameter();
        incParam.ParameterName = "@inc";
        incParam.Value = (long)count;
        command.Parameters.Add(incParam);

        var last = Convert.ToInt64(await command.ExecuteScalarAsync(cancellationToken));
        return last - count + 1;
    }
}
