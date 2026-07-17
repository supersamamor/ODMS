using FBSC.ODMS.Core.Constants;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using FBSC.ODMS.Infrastructure.Services.Dashboard;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;

namespace FBSC.ODMS.Scheduler.Jobs
{
    /// <summary>
    /// Proactively refreshes DashboardQueryResultCache ahead of expiry for every active
    /// DashboardQuery that has a configured refresh interval (widget.RefreshIntervalOverrideSeconds,
    /// falling back to the owning dashboard's RefreshIntervalSeconds). Queries with no
    /// configured interval are served on-demand only and are left alone here. Runs on the
    /// existing Quartz scheduler (FBSC.ODMS.Scheduler) - no second scheduling mechanism.
    /// </summary>
    [DisallowConcurrentExecution]
    public class DashboardCacheRefreshJob(
        ApplicationContext context,
        DashboardQueryExecutionService executionService,
        DashboardCacheService cacheService,
        ILogger<DashboardCacheRefreshJob> logger) : IJob
    {
        private const string SystemUserId = "System";
        private const int LookaheadSeconds = 60;
        private const int MaxRetries = 3;

        public async Task Execute(IJobExecutionContext context)
        {
            await RefreshDueQueriesAsync(context.CancellationToken);
        }

        private async Task RefreshDueQueriesAsync(CancellationToken cancellationToken)
        {
            var candidates = await context.DashboardQuery
                .Where(q => q.IsActive)
                .Include(q => q.DashboardQueryParameterList)
                .Include(q => q.DashboardWidgetList!).ThenInclude(w => w.Dashboard)
                .ToListAsync(cancellationToken);

            foreach (var query in candidates)
            {
                var refreshIntervalSeconds = EffectiveRefreshIntervalSeconds(query);
                if (refreshIntervalSeconds is null or <= 0)
                {
                    continue;
                }

                var defaultParameterValues = (query.DashboardQueryParameterList ?? [])
                    .ToDictionary(p => p.ParameterName, p => p.DefaultValue);
                var parameterSetHash = DashboardCacheService.ComputeParameterSetHash(defaultParameterValues);

                var cacheEntry = await context.DashboardQueryResultCache
                    .Where(c => c.DashboardQueryId == query.Id && c.ParameterSetHash == parameterSetHash)
                    .OrderByDescending(c => c.CachedAt)
                    .FirstOrDefaultAsync(cancellationToken);

                var dueForRefresh = cacheEntry is null
                    || cacheEntry.IsStale
                    || cacheEntry.ExpiresAt is null
                    || cacheEntry.ExpiresAt <= DateTime.UtcNow.AddSeconds(LookaheadSeconds);

                if (!dueForRefresh)
                {
                    continue;
                }

                await RefreshOneAsync(query, defaultParameterValues, parameterSetHash, refreshIntervalSeconds.Value, cancellationToken);
            }
        }

        private static int? EffectiveRefreshIntervalSeconds(DashboardQueryState query) =>
            (query.DashboardWidgetList ?? [])
                .Select(w => w.RefreshIntervalOverrideSeconds ?? w.Dashboard?.RefreshIntervalSeconds)
                .Where(interval => interval is > 0)
                .DefaultIfEmpty(null)
                .Min();

        private async Task RefreshOneAsync(
            DashboardQueryState query,
            IReadOnlyDictionary<string, string?> parameterValues,
            string parameterSetHash,
            int refreshIntervalSeconds,
            CancellationToken cancellationToken)
        {
            var jobLog = new DashboardRefreshJobState
            {
                DashboardQueryId = query.Id,
                TriggerType = RefreshTriggerType.Scheduled,
                Status = RefreshJobStatus.Running,
                QueuedAt = DateTime.UtcNow,
                StartedAt = DateTime.UtcNow,
                MaxRetries = MaxRetries,
                RetryCount = 0,
            };
            context.DashboardRefreshJob.Add(jobLog);
            await context.SaveChangesAsync(cancellationToken);

            try
            {
                var result = await executionService.ExecuteAsync(query, parameterValues, SystemUserId, Guid.NewGuid().ToString(), cancellationToken);

                if (result.Success)
                {
                    await cacheService.SaveAsync(query.Id, parameterSetHash, result, refreshIntervalSeconds, cancellationToken);
                }

                context.Entry(jobLog).CurrentValues.SetValues(jobLog with
                {
                    Status = result.Success ? RefreshJobStatus.Completed : RefreshJobStatus.Failed,
                    CompletedAt = DateTime.UtcNow,
                    DurationMs = result.DurationMs,
                    RowsCached = result.Success ? result.RowCount : null,
                    ErrorRemarks = result.ErrorMessage,
                });
                await context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DashboardCacheRefreshJob failed for DashboardQueryId {DashboardQueryId}", query.Id);
                context.Entry(jobLog).CurrentValues.SetValues(jobLog with
                {
                    Status = RefreshJobStatus.Failed,
                    CompletedAt = DateTime.UtcNow,
                    ErrorRemarks = ex.Message,
                });
                await context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
