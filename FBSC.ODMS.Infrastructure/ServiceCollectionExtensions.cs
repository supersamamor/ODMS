using FBSC.ODMS.Infrastructure.Services.Dashboard;
using Microsoft.Extensions.DependencyInjection;

namespace FBSC.ODMS.Infrastructure;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the dynamic-dashboard execution engine: data source connection factory,
    /// SQL query validator, schema discovery, query execution, chart-config mapping, and
    /// result caching. Called from every host that needs to run a DashboardQuery (Web, API,
    /// and - via FBSC.ODMS.Scheduler's own project reference to Infrastructure - the
    /// background refresh job) so there is exactly one registration path.
    /// </summary>
    public static IServiceCollection AddDashboardEngineServices(this IServiceCollection services)
    {
        services.AddScoped<DataSourceConnectionFactory>();
        services.AddScoped<SqlQueryValidator>();
        services.AddScoped<DataSourceSchemaDiscoveryService>();
        services.AddScoped<DashboardQueryExecutionService>();
        services.AddScoped<ChartConfigurationService>();
        services.AddScoped<DashboardCacheService>();
        services.AddScoped<DashboardAccessAuthorizationService>();
        return services;
    }
}
