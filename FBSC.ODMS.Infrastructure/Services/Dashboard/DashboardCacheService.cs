using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FBSC.ODMS.Infrastructure.Services.Dashboard;

public record CachedQueryPayload(IReadOnlyList<DashboardQueryResultColumnInfo> Columns, IReadOnlyList<Dictionary<string, object?>> Rows);

/// <summary>
/// Reads/writes DashboardQueryResultCacheState, keyed by a hash of the resolved parameter
/// set so the same query with different filter values gets independent cache entries.
/// </summary>
public class DashboardCacheService(ApplicationContext context)
{
    public static string ComputeParameterSetHash(IReadOnlyDictionary<string, string?> parameterValues)
    {
        var canonical = string.Join("&", parameterValues
            .OrderBy(kv => kv.Key, StringComparer.Ordinal)
            .Select(kv => $"{kv.Key}={kv.Value}"));
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(canonical));
        return Convert.ToHexString(hashBytes);
    }

    public async Task<CachedQueryPayload?> TryGetFreshAsync(string dashboardQueryId, string parameterSetHash, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var cacheEntry = await context.DashboardQueryResultCache
            .Where(c => c.DashboardQueryId == dashboardQueryId
                && c.ParameterSetHash == parameterSetHash
                && !c.IsStale
                && (c.ExpiresAt == null || c.ExpiresAt > now))
            .OrderByDescending(c => c.CachedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (cacheEntry?.ResultJson is null)
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<CachedQueryPayload>(cacheEntry.ResultJson);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    public async Task SaveAsync(
        string dashboardQueryId,
        string parameterSetHash,
        DashboardQueryExecutionResult result,
        int cacheDurationSeconds,
        CancellationToken cancellationToken = default)
    {
        var payload = new CachedQueryPayload(result.Columns, result.Rows);
        var resultJson = JsonSerializer.Serialize(payload);
        var now = DateTime.UtcNow;

        var existing = await context.DashboardQueryResultCache
            .Where(c => c.DashboardQueryId == dashboardQueryId && c.ParameterSetHash == parameterSetHash)
            .ToListAsync(cancellationToken);
        context.DashboardQueryResultCache.RemoveRange(existing);

        context.DashboardQueryResultCache.Add(new DashboardQueryResultCacheState
        {
            DashboardQueryId = dashboardQueryId,
            ParameterSetHash = parameterSetHash,
            ResultJson = resultJson,
            RowCount = result.RowCount,
            CacheSizeBytes = Encoding.UTF8.GetByteCount(resultJson),
            CachedAt = now,
            ExpiresAt = cacheDurationSeconds > 0 ? now.AddSeconds(cacheDurationSeconds) : null,
            IsStale = false,
        });

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task InvalidateAsync(string dashboardQueryId, CancellationToken cancellationToken = default)
    {
        var entries = await context.DashboardQueryResultCache
            .Where(c => c.DashboardQueryId == dashboardQueryId && !c.IsStale)
            .ToListAsync(cancellationToken);
        foreach (var entry in entries)
        {
            context.Entry(entry).CurrentValues.SetValues(entry with { IsStale = true });
        }
        await context.SaveChangesAsync(cancellationToken);
    }
}
