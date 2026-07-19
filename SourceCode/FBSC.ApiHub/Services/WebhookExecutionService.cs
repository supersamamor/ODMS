using FBSC.ApiHub.Context;
using FBSC.ApiHub.Models;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Concurrent;

namespace FBSC.ApiHub.Services
{
    /// <summary>
    /// Core service responsible for orchestrating, executing, and logging outgoing webhooks.
    /// Supports direct execution, background queuing, and retry mechanisms.
    /// </summary>
    public class WebhookExecutionService(
        IDbContextFactory<WebhookContext> contextFactory,
        WebHookService webhookService,
        WebHookWithoutSSLValidationService webhookWithoutSSLValidationService,
        ILogger<WebhookExecutionService> logger,
        IConfiguration configuration,
        WebhookJobLockingService lockingService)
    {
        private static readonly SemaphoreSlim Semaphore = new(5);
        private static readonly ConcurrentQueue<Func<Task>> TaskQueue = new();
        private static readonly SemaphoreSlim TaskQueueSemaphore = new(1);

        /// <summary>
        /// Executes a webhook for a specific event and returns the raw string response from the destination server.
        /// </summary>
        /// <param name="eventName">The name of the event triggering the webhook.</param>
        /// <param name="dataId">The primary key or reference ID of the source data.</param>
        /// <param name="payload">The object payload to be serialized into JSON.</param>
        /// <param name="processResponse">Flag indicating if the response requires downstream background processing.</param>
        /// <param name="customQueryParameters">Dictionary of route parameters to inject into the endpoint URL.</param>
        /// <param name="cancellationToken">Cancellation token to gracefully abort the operation.</param>
        /// <returns>The raw string response body from the webhook endpoint.</returns>
        public async Task<string> ExecuteWebhooksWithResponseAsync(string eventName, string dataId, object? payload, bool processResponse = false,
            Dictionary<string, string>? customQueryParameters = null, CancellationToken cancellationToken = default)
        {
            var (result, _) = await ExecuteWebhookInternalAsync(eventName, dataId, payload, processResponse, customQueryParameters, cancellationToken);
            return result ?? string.Empty;
        }

        /// <summary>
        /// Executes a webhook for a specific event and returns the newly created Webhook Log ID.
        /// </summary>
        /// <param name="eventName">The name of the event triggering the webhook.</param>
        /// <param name="dataId">The primary key or reference ID of the source data.</param>
        /// <param name="payload">The object payload to be serialized into JSON.</param>
        /// <param name="processResponse">Flag indicating if the response requires downstream background processing.</param>
        /// <param name="customQueryParameters">Dictionary of route parameters to inject into the endpoint URL.</param>
        /// <param name="cancellationToken">Cancellation token to gracefully abort the operation.</param>
        /// <returns>The generated ID of the WebhookLog database record.</returns>
        public async Task<string> ExecuteWebhooksWithWebhookLogsIdAsync(string eventName, string dataId, object? payload, bool processResponse = false,
            Dictionary<string, string>? customQueryParameters = null, CancellationToken cancellationToken = default)
        {
            var (_, webhookLogs) = await ExecuteWebhookInternalAsync(eventName, dataId, payload, processResponse, customQueryParameters, cancellationToken);
            return webhookLogs?.Id ?? string.Empty;
        }    

        /// <summary>
        /// Internal core execution logic that builds the payload, saves the initial log, dispatches the HTTP request, and records the result. this method throws
        /// </summary>
        /// <returns>A tuple containing the string response and the generated WebhookLogsState entity.</returns>
        public async Task<(string? Result, WebhookLogsState? WebhookLogs)> ExecuteWebhookInternalAsync(
            string eventName, string dataId, object? payload, bool processResponse = false,
            Dictionary<string, string>? customQueryParameters = null, CancellationToken cancellationToken = default)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

            var webhookEvent = await context.WebhookEventAssignment
                .Include(l => l.WebhookApi)
                .Where(l => l.EventName == eventName && l.Active == true && l.WebhookApi!.Active == true)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (webhookEvent == null)
                return (null, null);

            var apiRoute = BuildRoute(webhookEvent.Route, customQueryParameters);
           
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            string? serializedPayload = JsonConvert.SerializeObject(payload, serializerSettings);
            if (!string.IsNullOrEmpty(webhookEvent?.WebhookApi?.AdditionalConfigurationJson))
            {
                serializedPayload = BuildPayloadBasedOnConfig(webhookEvent!.WebhookApi!.AdditionalConfigurationJson, customQueryParameters);
            }
            var webhookLogs = new WebhookLogsState
            {
                DataId = dataId,
                WebhookEventAssignmentId = webhookEvent!.Id,
                Payload = serializedPayload,
                ProcessResponse = processResponse,
                ProcessResponseStatus = processResponse ? WebhookStatus.Pending : null,
                ParametarizedRoute = apiRoute
            };

            webhookLogs.TagAsStarted();
            await context.AddAsync(webhookLogs, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            string result = string.Empty;
            try
            {
                result = webhookEvent.WebhookApi!.WithinPrivateNetwork
                    ? await webhookWithoutSSLValidationService.ExecuteWebhook(webhookEvent.WebhookApi, webhookEvent, serializedPayload, apiRoute, cancellationToken)
                    : await webhookService.ExecuteWebhook(webhookEvent.WebhookApi, webhookEvent, serializedPayload, apiRoute, cancellationToken);

                webhookLogs.SetResponse(result);
                webhookLogs.TagAsDone();
            }
            catch (Exception ex)
            {
                // 1. Update the log with the error
                webhookLogs.TagAsCompletedWithError(ex.Message);

                // 2. Save the failed state to the database IMMEDIATELY
                context.Update(webhookLogs);
                await context.UpdateRecordFromJobsAsync(cancellationToken);

                // 3. Format and throw the new exception safely
                var errorMessage = ex.InnerException != null
                    ? $"{ex.Message} | Inner Exception: {ex.InnerException.Message}"
                    : ex.Message;

                throw new Exception(errorMessage, ex);
            }

            // 4. If the try block succeeded, save the successful state to the database here
            context.Update(webhookLogs);
            await context.UpdateRecordFromJobsAsync(cancellationToken);

            return (result, webhookLogs);
        }

        /// <summary>
        /// Retrieves all active webhook assignments for a specific event and schedules them for execution.
        /// Supports enqueueing executions in an in-memory background task queue.
        /// </summary>
        /// <param name="immediateExecution">If true, adds the webhook tasks to the concurrent background queue immediately.</param>
        /// <returns>A comma-separated string of the generated Webhook Log IDs.</returns>
        public async Task<string> ExecuteWebhooksByEventAsync(string eventName, string dataId, object? payload,
            bool processResponse = false, Dictionary<string, string>? customQueryParameters = null,
            bool immediateExecution = false, CancellationToken cancellationToken = default)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

            var webhookEventList = await context.WebhookEventAssignment
                .Include(l => l.WebhookApi)
                .Where(l => l.EventName == eventName && l.Active && l.WebhookApi!.Active)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            if (webhookEventList.Count == 0) return string.Empty;

            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            var serializedPayload = JsonConvert.SerializeObject(payload, serializerSettings);

            var newLogs = new List<WebhookLogsState>();

            foreach (var webhookEvent in webhookEventList)
            {
                var apiRoute = BuildRoute(webhookEvent.Route, customQueryParameters);

                var log = new WebhookLogsState
                {
                    DataId = dataId,
                    WebhookEventAssignmentId = webhookEvent.Id,
                    Payload = serializedPayload,
                    ProcessResponse = processResponse,
                    ProcessResponseStatus = processResponse ? WebhookStatus.Pending : null,
                    ParametarizedRoute = apiRoute,
                };

                if (immediateExecution) log.TagAsStarted();
                newLogs.Add(log);
            }

            context.WebhookLogs.AddRange(newLogs);
            await context.SaveChangesAsync(cancellationToken);

            if (immediateExecution)
            {
                foreach (var log in newLogs)
                {
                    var capturedLogId = log.Id;
                    var capturedRoute = log.ParametarizedRoute;
                    var capturedAssignment = webhookEventList.First(x => x.Id == log.WebhookEventAssignmentId);
                    var capturedWebhookApi = capturedAssignment.WebhookApi!;

                    TaskQueue.Enqueue(async () =>
                    {
                        await Semaphore.WaitAsync();
                        try
                        {
                            await ExecuteWithRetriesAndTimeoutAsync(async () =>
                            {
                                string result = capturedWebhookApi.WithinPrivateNetwork
                                    ? await webhookWithoutSSLValidationService.ExecuteWebhook(capturedWebhookApi, capturedAssignment, serializedPayload, capturedRoute, CancellationToken.None)
                                    : await webhookService.ExecuteWebhook(capturedWebhookApi, capturedAssignment, serializedPayload, capturedRoute, CancellationToken.None);

                                using var db = await contextFactory.CreateDbContextAsync(CancellationToken.None);
                                var dbLog = await db.WebhookLogs.FindAsync(capturedLogId);
                                dbLog?.TagAsDone(result);
                                await db.SaveChangesAsync(CancellationToken.None);

                            }, 10000, CancellationToken.None);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Webhook background execution failed for Log ID {LogId}", capturedLogId);

                            using var dbError = await contextFactory.CreateDbContextAsync(CancellationToken.None);
                            var errorLog = await dbError.WebhookLogs.FindAsync(capturedLogId);
                            errorLog?.SetError(ex.Message);
                            await dbError.SaveChangesAsync(CancellationToken.None);
                        }
                        finally
                        {
                            Semaphore.Release();
                        }
                    });
                }

                _ = ProcessTaskQueueAsync();
            }

            return string.Join(",", newLogs.Select(l => l.Id));
        }

        /// <summary>
        /// Background worker method that continuously processes delegates from the in-memory TaskQueue.
        /// </summary>
        private static async Task ProcessTaskQueueAsync()
        {
            await TaskQueueSemaphore.WaitAsync();
            try
            {
                while (TaskQueue.TryDequeue(out var task))
                {
                    _ = task();
                }
            }
            finally
            {
                TaskQueueSemaphore.Release();
            }
        }

        /// <summary>
        /// Wraps a task with an exponential backoff retry mechanism and a timeout token.
        /// </summary>
        /// <param name="task">The asynchronous function to execute.</param>
        /// <param name="timeoutMilliseconds">Maximum time allowed before cancellation.</param>
        /// <param name="cancellationToken">Global cancellation token.</param>
        /// <param name="maxRetries">Maximum number of retry attempts.</param>
        /// <param name="delayMilliseconds">Base delay for exponential backoff calculations.</param>
        private static async Task ExecuteWithRetriesAndTimeoutAsync(Func<Task> task, int timeoutMilliseconds,
            CancellationToken cancellationToken, int maxRetries = 3, int delayMilliseconds = 500)
        {
            int retries = 0;
            while (true)
            {
                using var timeoutCts = new CancellationTokenSource(timeoutMilliseconds);
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);
                try
                {
                    await task();
                    break;
                }
                catch (TaskCanceledException) when (timeoutCts.IsCancellationRequested)
                {
                    throw new TimeoutException("The webhook execution timed out.");
                }
                catch (Exception)
                {
                    retries++;
                    if (retries >= maxRetries) throw;
                    await Task.Delay(delayMilliseconds * (int)Math.Pow(2, retries));
                }
            }
        }

        /// <summary>
        /// Explicitly handles the execution of webhooks designated strictly for HTTP GET requests.
        /// </summary>
        public async Task<string> ExecuteGetMethodAsync(string eventName, CancellationToken cancellationToken,
            Dictionary<string, string>? customQueryParameters = null)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

            var webhookEvent = await context.WebhookEventAssignment
                .Include(l => l.WebhookApi)
                .Where(l => l.EventName == eventName && l.Active == true && l.WebhookApi!.Active == true)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (webhookEvent == null) return string.Empty;

            var apiRoute = BuildRoute(webhookEvent.Route, customQueryParameters);

            return webhookEvent.WebhookApi!.WithinPrivateNetwork
                ? await webhookWithoutSSLValidationService.ExecuteWebhook(webhookEvent.WebhookApi, webhookEvent, null, apiRoute, cancellationToken)
                : await webhookService.ExecuteWebhook(webhookEvent.WebhookApi, webhookEvent, null, apiRoute, cancellationToken);
        }

        /// <summary>
        /// Intended for Quartz Scheduler/Background Jobs. Sweeps the database for pending webhooks and executes them in parallel.
        /// Implements database row-locking to ensure thread safety across distributed instances.
        /// </summary>
        /// <param name="instanceName">The identifier of the server/pod claiming the jobs.</param>
        /// <param name="cancellationToken">Cancellation token to gracefully abort the batch processing.</param>
        public async Task ProcessWebhookJobAsync(string instanceName, CancellationToken cancellationToken)
        {
            List<WebhookLogsState> pendingWebhookJobs;

            using (var context = await contextFactory.CreateDbContextAsync(cancellationToken))
            {
                pendingWebhookJobs = await context.WebhookLogs
                    .Where(l => l.Status == WebhookStatus.Pending)
                    .OrderBy(l => l.CreatedDate)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);
            }

            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = configuration.GetValue<int>("WebHookJobMaxDegreeOfParallelism", 5),
                CancellationToken = cancellationToken
            };

            await Parallel.ForEachAsync(pendingWebhookJobs, parallelOptions, async (webhookLog, token) =>
            {
                bool claimed = await lockingService.TryClaimWebhookAsync(webhookLog.Id, instanceName, token);

                if (!claimed) return;

                using var iterationContext = await contextFactory.CreateDbContextAsync(token);

                var trackedLog = await iterationContext.WebhookLogs
                    .Include(l => l.WebhookEventAssignment)
                        .ThenInclude(wa => wa!.WebhookApi)
                    .FirstOrDefaultAsync(l => l.Id == webhookLog.Id, token);

                if (trackedLog == null) return;

                var webhookAssignment = trackedLog.WebhookEventAssignment!;
                var webhookApi = webhookAssignment.WebhookApi!;

                try
                {
                    var result = webhookApi.WithinPrivateNetwork
                        ? await webhookWithoutSSLValidationService.ExecuteWebhook(webhookApi, webhookAssignment, trackedLog.Payload, trackedLog.ParametarizedRoute, token)
                        : await webhookService.ExecuteWebhook(webhookApi, webhookAssignment, trackedLog.Payload, trackedLog.ParametarizedRoute, token);

                    trackedLog.TagAsDone(result);
                }
                catch (Exception ex)
                {
                    trackedLog.TagAsFailed(ex.Message);
                    logger.LogError(ex, "Webhook failed for Log ID: {Id}", trackedLog.Id);
                }

                await iterationContext.UpdateRecordFromJobsAsync(token);
            });
        }

        /// <summary>
        /// Sweeps the database for webhooks that have completed but still require specific downstream business logic to process the response.
        /// </summary>
        /// <param name="instanceName">The identifier of the server/pod claiming the jobs.</param>
        /// <param name="webhookHandler">A delegate containing the custom business logic to run against the webhook response.</param>
        /// <param name="cancellationToken">Cancellation token to gracefully abort the operation.</param>
        public async Task ProcessWebhookResponseJobAsync(string instanceName, Func<WebhookLogsState, Task> webhookHandler, CancellationToken cancellationToken = default)
        {
            List<WebhookLogsState> webhookLogsList;
            using (var listContext = await contextFactory.CreateDbContextAsync(cancellationToken))
            {
                webhookLogsList = await listContext.WebhookLogs
                    .Where(l => l.ProcessResponseStatus == WebhookStatus.Pending)
                    .Include(l => l.WebhookEventAssignment)
                    .OrderBy(l => l.CreatedDate)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);
            }

            foreach (var webhookLog in webhookLogsList)
            {
                bool claimed = await lockingService.TryClaimWebhookResponseAsync(webhookLog.Id, instanceName, cancellationToken);

                if (!claimed) continue;

                using var iterationContext = await contextFactory.CreateDbContextAsync(cancellationToken);

                var trackedLog = await iterationContext.WebhookLogs
                    .Include(l => l.WebhookEventAssignment)
                    .FirstOrDefaultAsync(l => l.Id == webhookLog.Id, cancellationToken);

                if (trackedLog == null) continue;

                try
                {
                    await webhookHandler(trackedLog);
                    trackedLog.TagProcessResponseStatusAsDone();
                }
                catch (Exception ex)
                {
                    trackedLog.TagProcessResponseStatusAsFailed(ex.Message);
                    logger.LogError(ex, "Failed to process webhook response for Log ID {LogId}: {Message}", trackedLog.Id, ex.Message);
                }

                await iterationContext.UpdateRecordFromJobsAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Replaces token placeholders in a routing URL (e.g., '{id}') with actual values provided via a dictionary.
        /// </summary>
        /// <param name="route">The unformatted API route string.</param>
        /// <param name="queryParameters">Dictionary mapping placeholder keys to their target values.</param>
        /// <returns>The fully formatted routing string.</returns>
        private static string BuildRoute(string route, Dictionary<string, string>? queryParameters)
        {        
            if (queryParameters?.Count > 0)
            {
                foreach (var param in queryParameters)
                {
                    var placeholder = "{" + param.Key.ToLower() + "}";
                    route = route.Replace(placeholder, param.Value, StringComparison.OrdinalIgnoreCase);
                }
            }    
            return route;
        }
        private static string? BuildPayloadBasedOnConfig(string? additionalConfigurationJson, Dictionary<string, string>? queryParameters)
        {
            if (string.IsNullOrEmpty(additionalConfigurationJson))
            {
                return null;
            }
            if (queryParameters?.Count > 0)
            {
                foreach (var param in queryParameters)
                {
                    var placeholder = "{" + param.Key.ToLower() + "}";
                    additionalConfigurationJson = additionalConfigurationJson.Replace(placeholder, param.Value, StringComparison.OrdinalIgnoreCase);
                }
            }
            return additionalConfigurationJson;
        }
    }
}