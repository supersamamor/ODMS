using FBSC.ApiHub.Context;
using FBSC.ApiHub.Models;
using Microsoft.EntityFrameworkCore;

namespace FBSC.ApiHub.Services
{
    /// <summary>
    /// This service is for future use, when we need multiple instances of the Quartz job running concurrently (e.g. multiple servers or multiple threads).
    /// </summary>
    /// <param name="contextFactory"></param>
    public class WebhookJobLockingService(IDbContextFactory<WebhookContext> contextFactory)
    {                 
        public async Task<bool> TryClaimWebhookResponseAsync(string webhookLogId, string instanceName, CancellationToken cancellationToken = default)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

            int rowsUpdated = await context.WebhookLogs
                .Where(w => w.Id == webhookLogId && w.ProcessResponseStatus == WebhookStatus.Pending)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(w => w.ProcessResponseStatus, WebhookStatus.Executing)
                    .SetProperty(w => w.LockedByInstance, instanceName),
                    cancellationToken);

            return rowsUpdated == 1;
        }      
        public async Task<bool> TryClaimWebhookAsync(string webhookLogId, string instanceName, CancellationToken cancellationToken = default)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

            int rowsUpdated = await context.WebhookLogs
                .Where(w => w.Id == webhookLogId && w.Status == WebhookStatus.Pending)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(w => w.Status, WebhookStatus.Executing)
                    .SetProperty(w => w.LockedByInstance, instanceName) // Stamp ownership
                    .SetProperty(w => w.DateTimeStarted, DateTime.UtcNow),
                    cancellationToken);

            return rowsUpdated == 1;
        }     
        public async Task ResetMyOrphanedWebhooksAsync(string myInstanceName, CancellationToken cancellationToken = default)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

            // Sweep for jobs that this exact server left behind when it crashed
            await context.WebhookLogs
                .Where(w => w.Status == WebhookStatus.Executing && w.LockedByInstance == myInstanceName)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(w => w.Status, WebhookStatus.Pending)
                    .SetProperty(w => w.LockedByInstance, (string?)null) // Clear the lock
                    .SetProperty(w => w.Error, $"CRASH RECOVERY: Job resumed by {myInstanceName}."),
                    cancellationToken);
        }
    }
}