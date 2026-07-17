using System.Text.Json;
using FBSC.ApiHub.Services;
using FBSC.GenAI.Services.AI.Providers;
using FBSC.ODMS.Application.Constants;
using FBSC.ODMS.Core.Constants;
using FBSC.ODMS.Infrastructure.Data;
using FBSC.ODMS.Infrastructure.Services.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace FBSC.ODMS.Web.Service
{
    public record DashboardQueryGenerationResult(string? GeneratedSqlQueryText, int ConfidenceScore, string ValidationStatus, string? ErrorRemarks);

    /// <summary>
    /// NL-prompt-to-SQL generation for an arbitrary DataSourceState, following the exact same
    /// webhook-based AI pipeline as AIReportQueryGenerationServices (WebhookExecutionService +
    /// AiResponseParserFactory) instead of a second AI integration, but scoped to the
    /// DataSourceSchemaCache of the selected data source rather than the app's own database.
    /// The generated SQL is never executed here - it is only ever returned for a human to
    /// review and approve (see AiSqlGenerationRequestState.ValidationStatus).
    /// </summary>
    public class AIDashboardQueryGenerationServices(
        ApplicationContext context,
        WebhookExecutionService webhookExecutionService,
        AiResponseParserFactory aiResponseParserFactory,
        DataSourceSchemaDiscoveryService schemaDiscoveryService,
        SqlQueryValidator sqlQueryValidator)
    {
        private const string DefaultPromptTemplate = """
            You are a Microsoft SQL Server (T-SQL) expert generating a dashboard widget query.

            Request: {NaturalLanguagePrompt}

            Database schema (JSON - table name to columns, each with its SQL type and inferred role): {SchemaJson}

            Requirements:
            - Write a single SELECT statement only. Never use INSERT, UPDATE, DELETE, DROP, ALTER, TRUNCATE, EXEC, or multiple statements separated by semicolons.
            - Use only tables and columns that exist in the schema above - never invent column or table names.
            - Use square-bracket delimited identifiers for table and column names, e.g. [ColumnName].
            - Return only the raw SQL query. Do not include comments, explanations, or markdown code fences.
            {ChartQueryFormat}
            """;

        public async Task<DashboardQueryGenerationResult> GenerateSqlAsync(
            string dataSourceId,
            string naturalLanguagePrompt,
            string? reportOrChartType = null,
            CancellationToken cancellationToken = default)
        {
            var schemaJson = await GetDataSourceSchemaJsonAsync(dataSourceId, cancellationToken);
            var promptTemplate = await GetPromptTemplateAsync(dataSourceId, cancellationToken);
            var chartQueryFormat = reportOrChartType is not null
                && Core.Constants.ReportChartType.ChartQueryFormat.TryGetValue(reportOrChartType, out var format)
                    ? format
                    : string.Empty;

            var prompt = BuildPrompt(promptTemplate, naturalLanguagePrompt, schemaJson, chartQueryFormat);

            Dictionary<string, string> customQueryParameters = new()
            {
                { "PromptMessage", prompt.Replace("\"", "") },
            };

            var rawResponse = await webhookExecutionService.ExecuteWebhooksWithResponseAsync(
                WebhookEvents.PromptGenerativeAI,
                "ReportQueryBuilder",
                null,
                customQueryParameters: customQueryParameters,
                cancellationToken: cancellationToken);

            var generatedSql = aiResponseParserFactory.ParseAutoDetect(rawResponse);
            if (string.IsNullOrWhiteSpace(generatedSql))
            {
                return new DashboardQueryGenerationResult(null, 0, QueryValidationStatus.Invalid, "The AI provider did not return a SQL query.");
            }

            // Syntactic validity only informs the confidence score shown to the reviewer -
            // it never skips the human approval step (ValidationStatus is always
            // PendingApproval here; only an explicit Approve action can change that).
            var validation = sqlQueryValidator.Validate(generatedSql);
            var confidenceScore = validation.IsValid ? 80 : 20;
            return new DashboardQueryGenerationResult(generatedSql, confidenceScore, QueryValidationStatus.PendingApproval, validation.ErrorMessage);
        }

        private async Task<string> GetDataSourceSchemaJsonAsync(string dataSourceId, CancellationToken cancellationToken)
        {
            var hasCachedSchema = await context.DataSourceSchemaCache.AnyAsync(s => s.DataSourceId == dataSourceId, cancellationToken);
            if (!hasCachedSchema)
            {
                // UploadedFile data sources are cataloged entirely by UploadedFileIngestionService
                // at upload time - DiscoverAsync only applies to ExternalDatabase connections.
                // An UploadedFile source with nothing uploaded yet just has no schema to offer.
                var connectionKind = await context.DataSource
                    .Where(d => d.Id == dataSourceId)
                    .Select(d => d.ConnectionKind)
                    .SingleAsync(cancellationToken);
                if (connectionKind == Core.Constants.DataSourceConnectionKind.ExternalDatabase)
                {
                    await schemaDiscoveryService.DiscoverAsync(dataSourceId, cancellationToken);
                }
            }

            var columns = await context.DataSourceSchemaCache
                .Where(s => s.DataSourceId == dataSourceId)
                .OrderBy(s => s.SchemaName).ThenBy(s => s.TableName).ThenBy(s => s.OrdinalPosition)
                .Select(s => new { s.SchemaName, s.TableName, s.ColumnName, s.SqlDataType, s.InferredSemanticType })
                .ToListAsync(cancellationToken);

            var tables = columns
                .GroupBy(c => $"{c.SchemaName}.{c.TableName}")
                .Select(g => new
                {
                    TableName = g.Key,
                    Columns = g.Select(c => new { c.ColumnName, c.SqlDataType, c.InferredSemanticType }),
                });

            return JsonSerializer.Serialize(tables);
        }

        private async Task<string?> GetPromptTemplateAsync(string dataSourceId, CancellationToken cancellationToken)
        {
            var systemType = await context.DataSource
                .Where(d => d.Id == dataSourceId)
                .Select(d => d.SystemType)
                .SingleAsync(cancellationToken);

            return await context.AiSqlPromptTemplate
                .Where(t => t.SystemType == systemType && t.IsActive)
                .OrderBy(t => t.Sequence)
                .Select(t => t.PromptTemplate)
                .FirstOrDefaultAsync(cancellationToken);
        }

        private static string BuildPrompt(string? template, string naturalLanguagePrompt, string schemaJson, string chartQueryFormat)
        {
            var basePrompt = string.IsNullOrWhiteSpace(template) ? DefaultPromptTemplate : template;
            return basePrompt
                .Replace("{NaturalLanguagePrompt}", naturalLanguagePrompt)
                .Replace("{SchemaJson}", schemaJson)
                .Replace("{ChartQueryFormat}", chartQueryFormat);
        }
    }
}
