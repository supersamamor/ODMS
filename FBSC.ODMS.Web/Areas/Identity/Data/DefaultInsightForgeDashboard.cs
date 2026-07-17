using System.Security.Cryptography;
using System.Text;
using FBSC.Common.Identity.Abstractions;
using FBSC.ODMS.Core.Constants;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using FBSC.ODMS.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FBSC.ODMS.Web.Areas.Identity.Data;

/// <summary>
/// Seeds sample data for the dynamic dashboard engine (DataSource/DashboardQuery/ReportType/
/// DashboardTheme/Dashboard/DashboardWidget) - the same "Activity Logs" AuditLogs data
/// DefaultDashboard already seeds legacy Report records against, so the sample renders for
/// real as soon as the app starts (no external server dependency) and exercises the full
/// execution engine end to end. Follows DefaultDashboard.Seed's own pattern: construct a
/// scoped ApplicationContext directly, check for an existing record by natural key before
/// inserting, and SaveChangesAsync after each parent insert.
/// </summary>
public static class DefaultInsightForgeDashboard
{
    public static async Task Seed(IServiceProvider serviceProvider)
    {
        using var context = new ApplicationContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationContext>>(), serviceProvider.GetRequiredService<IAuthenticatedUser>());
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        var dashboard = await context.Dashboard.FirstOrDefaultAsync(d => d.Code == "SAMPLE-ACTIVITY-LOGS");
        if (dashboard != null)
        {
            return;
        }

        using var identityContext = new IdentityContext(serviceProvider.GetRequiredService<DbContextOptions<IdentityContext>>());
        var ownerUserId = (await identityContext.Users.FirstAsync(u => u.Email == "system@admin")).Id;

        // ---- Report types (structural rules per chart family) ----
        // ChartRenderer intentionally reuses Core.Constants.ReportChartType values (not a
        // chart.js-native vocabulary) so DashboardWidgetRenderHelper feeds
        // _ChartBuilderScripts.cshtml's BuildChart() the same way a legacy Report does.
        var barType = new ReportTypeState { Id = Guid.NewGuid().ToString(), Code = "BAR", Name = "Bar Chart", ChartRenderer = ReportChartType.Bar, MinColumnsRequired = 2, RequiresXAxis = true, RequiresYAxis = true, IconClass = "fas fa-chart-bar", IsActive = true };
        var pieType = new ReportTypeState { Id = Guid.NewGuid().ToString(), Code = "PIE", Name = "Pie Chart", ChartRenderer = ReportChartType.Pie, MinColumnsRequired = 2, RequiresXAxis = true, RequiresYAxis = true, IconClass = "fas fa-chart-pie", IsActive = true };
        var tableType = new ReportTypeState { Id = Guid.NewGuid().ToString(), Code = "TABLE", Name = "Table", ChartRenderer = ReportChartType.Table, MinColumnsRequired = 1, RequiresXAxis = false, RequiresYAxis = false, IconClass = "fas fa-table", IsActive = true };
        context.ReportType.AddRange(barType, pieType, tableType);
        await context.SaveChangesAsync();

        // ---- Theme ----
        var theme = new DashboardThemeState
        {
            Id = Guid.NewGuid().ToString(),
            Code = "DEFAULT",
            Name = "Default Theme",
            IsDarkMode = false,
            PrimaryColorHex = "#00BFFF",
            IsSystemDefault = true,
        };
        context.DashboardTheme.Add(theme);
        await context.SaveChangesAsync();

        // ---- Data source: this same application database, so the sample renders without
        // any external server to configure ----
        var dataSource = new DataSourceState
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Sample - This Application Database",
            SystemType = "Custom",
            ConnectionKind = DataSourceConnectionKind.ExternalDatabase,
            ConnectionMode = DataSourceConnectionMode.ConnectionString,
            ConnectionStringEncrypted = configuration.GetConnectionString("ApplicationContext"),
            Description = "Sample data source pointing at this app's own database (the AuditLogs table) so the seeded sample dashboard has real data to render out of the box.",
            IsActive = true,
        };
        dataSource = dataSource.EncryptSecrets(configuration.GetValue<string>("EncryptionDecryptionKeyPrefix")!);
        context.DataSource.Add(dataSource);
        await context.SaveChangesAsync();

        // ---- Query (shared by all three sample widgets, demonstrating query reuse) ----
        const string sqlQueryText = "SELECT [Type] AS [ActivityType], COUNT(*) AS [ActivityCount] FROM [dbo].[AuditLogs] GROUP BY [Type]";
        var query = new DashboardQueryState
        {
            Id = Guid.NewGuid().ToString(),
            DataSourceId = dataSource.Id,
            Name = "Sample - Activity Counts by Type",
            Description = "Row count per audit log Type, grouped - feeds the sample Bar/Pie/Table widgets.",
            SqlQueryText = sqlQueryText,
            QueryHash = ComputeQueryHash(sqlQueryText),
            IsParameterized = false,
            GeneratedByAI = false,
            IsActive = true,
            DashboardQueryResultColumnList =
            [
                new DashboardQueryResultColumnState { Id = Guid.NewGuid().ToString(), ColumnName = "ActivityType", OrdinalPosition = 1, SqlDataType = "nvarchar", InferredRole = SemanticType.Dimension, Sequence = 0 },
                new DashboardQueryResultColumnState { Id = Guid.NewGuid().ToString(), ColumnName = "ActivityCount", OrdinalPosition = 2, SqlDataType = "int", InferredRole = SemanticType.Measure, IsAggregatable = true, DefaultAggregation = AggregationType.Sum, Sequence = 1 },
            ],
        };
        context.DashboardQuery.Add(query);
        await context.SaveChangesAsync();

        // ---- Dashboard ----
        dashboard = new DashboardState
        {
            Id = Guid.NewGuid().ToString(),
            Code = "SAMPLE-ACTIVITY-LOGS",
            Name = "Sample Dashboard - Activity Logs",
            Description = "Seeded sample dashboard demonstrating the dynamic dashboard engine against this app's own Audit Logs.",
            Category = "Sample",
            DashboardThemeId = theme.Id,
            OwnerUserId = ownerUserId,
            IsPublic = false,
            IsTemplate = false,
            RefreshIntervalSeconds = 0,
            IsActive = true,
            DashboardAccessList =
            [
                new DashboardAccessState { Id = Guid.NewGuid().ToString(), GranteeType = DashboardGranteeType.Role, GranteeId = Roles.Admin, AccessLevel = DashboardAccessLevel.View, GrantedAt = DateTime.UtcNow },
            ],
        };
        context.Dashboard.Add(dashboard);
        await context.SaveChangesAsync();

        // ---- Widgets: same query, three different visualizations ----
        context.DashboardWidget.AddRange(
            new DashboardWidgetState
            {
                Id = Guid.NewGuid().ToString(),
                DashboardId = dashboard.Id,
                DashboardQueryId = query.Id,
                ReportTypeId = barType.Id,
                Title = "Activity Counts - Bar Chart",
                XAxisColumnName = "ActivityType",
                YAxisColumnsJson = "[\"ActivityCount\"]",
                GridPositionX = 0,
                GridPositionY = 0,
                GridWidth = 6,
                GridHeight = 1,
                Sequence = 0,
            },
            new DashboardWidgetState
            {
                Id = Guid.NewGuid().ToString(),
                DashboardId = dashboard.Id,
                DashboardQueryId = query.Id,
                ReportTypeId = pieType.Id,
                Title = "Activity Counts - Pie Chart",
                XAxisColumnName = "ActivityType",
                YAxisColumnsJson = "[\"ActivityCount\"]",
                GridPositionX = 6,
                GridPositionY = 0,
                GridWidth = 6,
                GridHeight = 1,
                Sequence = 1,
            },
            new DashboardWidgetState
            {
                Id = Guid.NewGuid().ToString(),
                DashboardId = dashboard.Id,
                DashboardQueryId = query.Id,
                ReportTypeId = tableType.Id,
                Title = "Activity Counts - Table",
                GridPositionX = 0,
                GridPositionY = 1,
                GridWidth = 12,
                GridHeight = 1,
                Sequence = 2,
            });
        await context.SaveChangesAsync();
    }

    private static string ComputeQueryHash(string sqlQueryText) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(sqlQueryText)));
}
