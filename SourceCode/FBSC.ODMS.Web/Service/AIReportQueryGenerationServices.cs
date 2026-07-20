using FBSC.ApiHub.Services;
using FBSC.GenAI.Services.AI.Providers;
using FBSC.HTMLTemplate.ViewModels;
using FBSC.ODMS.Application.Constants;
using FBSC.ODMS.Application.Helpers;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;

namespace FBSC.ODMS.Web.Service
{
    public class AIReportQueryGenerationServices(WebhookExecutionService webhookExecutionService, AiResponseParserFactory aiResponseParserFactory, IConfiguration configuration, ApplicationContext context)
    {
        private static readonly List<string> ExcludedTables =
        [
            SanitizeTableName(nameof(ReportState)),
            SanitizeTableName(nameof(ReportQueryFilterState)),
            SanitizeTableName(nameof(ReportAIIntegrationState)),
            SanitizeTableName(nameof(ReportRoleAssignmentState)),
            SanitizeTableName(nameof(UploadProcessorState)),
            SanitizeTableName(nameof(ApprovalState)),
            SanitizeTableName(nameof(ApprovalRecordState)),
            SanitizeTableName(nameof(ApproverSetupState)),
            SanitizeTableName(nameof(ApproverAssignmentState)),
            "__EFMigrationsHistory",
            "AspNetRoleClaims",
            "AspNetUserClaims",
            "AspNetUserLogins",
            "AspNetUserTokens",
            "AuditLogs",
            "Entities",
            "OpenIddictApplications",
            "OpenIddictAuthorizations",
            "OpenIddictScopes",
            "OpenIddictTokens",
        ];
        private static string SanitizeTableName(string input)
        {
            const string suffix = "State";
            if (input.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            {
                return input[..^suffix.Length];
            }
            return input;
        }
        public async Task<string?> SQLReportQueryGeneration(string reportDescription, string reportOrChartType, string? dataSourceId = null, CancellationToken token = new CancellationToken())
        {
            var (connectionString, onlyTableName) = await ResolveSchemaSourceAsync(dataSourceId, token);
            string minifiedDatabaseStructure = MinifyJson(await GetDatabaseStructureInJsonAsync(connectionString, onlyTableName));

            // Chart-specific query shape (e.g. "needs a Label field + numeric fields"),
            // looked up instead of switched on so new chart types don't require
            // touching this method again — see ReportChartType.ChartQueryFormat.
            string chartQueryFormat = Core.Constants.ReportChartType.ChartQueryFormat.TryGetValue(reportOrChartType, out var format)
                ? format
                : string.Empty;

            var chatGptPrompt = $@"You are a Microsoft SQL Server (T-SQL) expert generating a report query.

            Report description: {reportDescription}

            Database structure (JSON): {minifiedDatabaseStructure}

            Requirements:
            - Write a single SELECT statement only. Never use INSERT, UPDATE, DELETE, DROP, ALTER, TRUNCATE, EXEC, or multiple statements separated by semicolons.
            - Use only tables and columns that exist in the database structure above — never invent column or table names.
            - Use square-bracket delimited identifiers for table and column names, e.g. [ColumnName].
            - Return only the raw SQL query. Do not include comments, explanations, or markdown code fences.
            {chartQueryFormat}";

            Dictionary<string, string> customQueryParameters = new()
            {
                { "PromptMessage", chatGptPrompt.Replace("\"", "") },
            };

            var result = await webhookExecutionService.ExecuteWebhooksWithResponseAsync(
                WebhookEvents.PromptGenerativeAI,
                "ReportQueryBuilder",
                null,
                customQueryParameters: customQueryParameters,
                cancellationToken: token);

            return aiResponseParserFactory.ParseAutoDetect(result);
        }
        private static string MinifyJson(string json)
        {
            var parsedObject = JsonConvert.DeserializeObject(json);
            return JsonConvert.SerializeObject(parsedObject, Formatting.None);
        }
        /// <summary>
        /// Determines what schema the AI should see for a report: the target external SQL Server
        /// when the report's DataSourceId points at a SqlServer-type DataSource, just the one
        /// generated table when it points at a FileUpload-type DataSource, or the app's own default
        /// ReportContext database (today's behavior) when no DataSourceId is set.
        /// </summary>
        private async Task<(string ConnectionString, string? OnlyTableName)> ResolveSchemaSourceAsync(string? dataSourceId, CancellationToken token)
        {
            var defaultConnectionString = configuration.GetConnectionString("ReportContext")!;
            if (string.IsNullOrWhiteSpace(dataSourceId))
            {
                return (defaultConnectionString, null);
            }

            var dataSource = await context.DataSource
                .AsNoTracking()
                .Where(d => d.Id == dataSourceId && d.IsActive)
                .FirstOrDefaultAsync(token);

            if (dataSource is null)
            {
                return (defaultConnectionString, null);
            }

            if (string.Equals(dataSource.DataSourceType, Core.Constants.DataSourceTypes.FileUpload, StringComparison.OrdinalIgnoreCase))
            {
                // The table lives in the same database as ReportContext; restrict the schema shown
                // to the AI to just that one generated table so it can't hallucinate other tables.
                return (defaultConnectionString, dataSource.GeneratedTableName);
            }

            var resolvedConnectionString = await ReportDataHelper.ResolveConnectionStringAsync(context, configuration, dataSourceId, token);
            return (resolvedConnectionString, null);
        }
        public async Task<string> GetDatabaseStructureInJsonAsync(string connectionString, string? onlyTableName = null)
        {
            using SqlConnection connection = new(connectionString);
            connection.Open();
            DataTable tables = connection.GetSchema("Tables");
            List<object> databaseSchema = [];
            foreach (DataRow row in tables.Rows)
            {
                string tableName = (string)row["TABLE_NAME"];
                //ignore case sensitivity
                if (onlyTableName != null)
                {
                    if (!string.Equals(tableName, onlyTableName, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                }
                else if (ExcludedTables.Contains(tableName))
                {
                    continue;
                }
                using SqlCommand command = new($"SELECT TOP 0 * FROM [{tableName}]", connection);
                using SqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.KeyInfo);
                DataTable? schemaTable = await reader.GetSchemaTableAsync();
                var tableSchema = new
                {
                    TableName = tableName,
                    Columns = new List<object>()
                };
                if (schemaTable != null)
                {
                    foreach (DataRow schemaRow in schemaTable.Rows)
                    {
                        tableSchema.Columns.Add(new
                        {
                            ColumnName = schemaRow["ColumnName"],
                            DataType = schemaRow["DataType"].ToString()
                        });
                    }
                    databaseSchema.Add(tableSchema);
                }
            }
            return JsonConvert.SerializeObject(databaseSchema, Formatting.Indented);
        }
        public async Task<string?> GenerateHTMLTemplate(string reportDescription, string reportOrChartType, string? sqlQuery, CancellationToken token = new CancellationToken())
        {
            // Skip generation if it's not a type that requires HTML
            if ((reportOrChartType != Core.Constants.ReportChartType.PDF &&
                reportOrChartType != Core.Constants.ReportChartType.CustomHtml) || sqlQuery == null)
            {
                return string.Empty;
            }

            // Using $$""" allows { } to be treated as literal strings for the AI prompt,      // while C# variables are injected using {{ }}     
            var prompt = $$"""
            You are an expert Enterprise UI/UX Designer and Frontend Developer creating a modern HTML widget template for a dashboard.
        
            Report Description: {{reportDescription}}
            Target Output Type: {{reportOrChartType}}
            Source SQL Query: {{sqlQuery}}
        
            Strict Requirements:
            1. Layout: Create a highly responsive layout using inline CSS Flexbox (e.g., flex-wrap: nowrap, overflow-x: auto for horizontal scrolling cards, or flex-wrap: wrap for grids).
            2. Theming & Styling: You MUST use our existing CSS variables for all styling to maintain theme compliance. Do NOT hardcode colors.
                - Backgrounds: var(--bg-light)
                - Borders/Radii: var(--border-color), var(--border-radius-base)
                - Primary/Semantic Colors: var(--custom-primary), var(--color-success), var(--color-warning), var(--color-danger), var(--color-violet)
                - Typography: var(--font-size-dense) for labels, standard font weights (e.g., 700 for emphasis).
                - Shadows: Use subtle box-shadows (e.g., 0 .125rem .25rem rgba(0,0,0,.075)).
            3. Data Binding Engine: This template will be processed by a custom HTML parser. 
                - You MUST wrap repeatable row data (like individual stats cards or table rows) inside this exact comment block: <!--{#foreach Table}--> [repeatable html] <!--{/foreach}-->
            4. Placeholders & SQL Matching: Analyze the SELECT clause of the Source SQL Query provided above. Extract the exact column names or aliases. 
                - Your placeholders MUST perfectly match these SQL field names, wrapped in single curly braces.
                - For example, if the SQL contains `[e].[EmployeeCode]` or `[p].[Name] AS [ParentName]`, you MUST output {EmployeeCode} and {ParentName}. Do not invent field names.
            5. Icons: Use standard FontAwesome classes (e.g., fas fa-id-badge, fas fa-file-signature) mapped logically to the data fields.
            6. Output Format: Return ONLY the raw HTML string. Do not include markdown code fences (like ```html), comments explaining your code, or outer <html>/<body> tags. Provide the exact string to be rendered.
            """;

            Dictionary<string, string> customQueryParameters = new()
            {
                { "PromptMessage", prompt.Replace("\"", "'") }
            };

            var result = await webhookExecutionService.ExecuteWebhooksWithResponseAsync(
                WebhookEvents.PromptGenerativeAI,
                "ReportQueryBuilder",
                null,
                customQueryParameters: customQueryParameters,
                cancellationToken: token);

            return aiResponseParserFactory.ParseAutoDetect(result);
        }
        public async Task<string?> GenerateHTMLTemplateViaObject(HTMLTemplateViewModel setup, CancellationToken token = new CancellationToken())
        {
            // Using $$""" allows { } to be treated as literal strings for the AI prompt,      // while C# variables are injected using {{ }}     
            var prompt = $$"""
            You are an expert Enterprise Report Designer creating a professional HTML document template intended for PDF generation via wkhtmltopdf (Rotativa).
        
            Template Name: {{setup.HTMLTemplateName}}
            Report Description: {{setup.Description}}
        
            Canvas Specifications:
            - Paper Size: {{setup.PaperSize}}
            - Orientation: {{setup.Orientation}}
            - Margins (Top/Bottom/Left/Right): {{setup.MarginTop}}px / {{setup.MarginBottom}}px / {{setup.MarginLeft}}px / {{setup.MarginRight}}px
        
            Strict Requirements:
            1. Layout & PDF Compatibility: You are designing specifically for the Canvas Specifications listed above. Ensure the HTML width optimally fills a {{setup.PaperSize}} {{setup.Orientation}} space. Use robust, standard HTML `<table>` structures for data presentation. Do NOT use CSS Flexbox or CSS Grid, as they break in PDF rendering engines. 
            2. 100% Self-Contained Styling: Rotativa has NO access to external CSS libraries or local stylesheets. You MUST use strictly inline CSS (e.g., style="...") or a single embedded `<style>` block. Do NOT use `<link>` tags, external CDNs, or CSS variables.
            3. Corporate Aesthetic: Use a clean, corporate design with standard hex color codes (e.g., #333333 for text, #f2f2f2 for table headers, #dddddd for borders). Ensure high contrast and professional typography (e.g., Arial, Helvetica, sans-serif).
            4. Data Binding Engine & Collections: This template will be processed by a custom HTML parser. 
               - Assume the data will be provided as a collection of objects. You MUST wrap the repeatable HTML block (like table rows `<tr>`) inside this exact comment block: <!--{#foreach Table}--> [repeatable html] <!--{/foreach}-->
            5. Placeholders & Inferred Object Mapping: Since the exact C# object schema is not yet defined, infer the most logical data fields based on the Template Name and Report Description.
               - Create placeholders that follow standard C# PascalCase property naming conventions, wrapped in single curly braces (e.g., {Status}, {TotalAmount}, {EmployeeName}). Do not use spaces in placeholder names.
            6. Document Structure: Include a professional report header with the Template Name. Structure the data logically, ensuring table headers (`<th>`) align cleanly with the data rows (`<td>`). Prevent awkward page breaks inside rows by applying `page-break-inside: avoid;` to row elements where appropriate.
            7. Output Format: Return ONLY the raw HTML string. Do not include markdown code fences (like ```html), comments explaining your code, or outer <html>/<body> tags (a root <div> is preferred). Provide the exact string to be rendered.
            """;

            Dictionary<string, string> customQueryParameters = new()
            {
                { "PromptMessage", prompt.Replace("\"", "'") }
            };

            var result = await webhookExecutionService.ExecuteWebhooksWithResponseAsync(
                WebhookEvents.PromptGenerativeAI,
                "ReportQueryBuilder",
                null,
                customQueryParameters: customQueryParameters,
                cancellationToken: token);

            return aiResponseParserFactory.ParseAutoDetect(result);
        }
    }
}
