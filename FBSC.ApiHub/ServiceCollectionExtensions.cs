using FBSC.ApiHub.Context;
using FBSC.ApiHub.Services;
using FBSC.Common.Utility.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace FBSC.ApiHub;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiHubServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssemblies([Assembly.GetExecutingAssembly()]);
        });
        services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddTransient(typeof(CompositeValidator<>));

        // 1. Register WebHookService with strict Connection Pooling limits
        services.AddHttpClient<WebHookService>()
            .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
            {
                // Forces DNS refresh and prevents stale tunnels
                PooledConnectionLifetime = TimeSpan.FromMinutes(2),
                // Closes idle connections BEFORE Oracle's firewall drops them (prevents silent retries)
                PooledConnectionIdleTimeout = TimeSpan.FromSeconds(30),
                // Prevents silent duplicate requests caused by HTTP redirects
                AllowAutoRedirect = false
            });

        // 2. Register WebHookWithoutSSLValidationService with SSL Bypass AND Connection Pooling limits
        services.AddHttpClient<WebHookWithoutSSLValidationService>()
            .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
            {
                // Modern way to bypass SSL validation on SocketsHttpHandler
                SslOptions = new System.Net.Security.SslClientAuthenticationOptions
                {
                    RemoteCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                },
                // Apply the same strict timeout rules
                PooledConnectionLifetime = TimeSpan.FromMinutes(2),
                PooledConnectionIdleTimeout = TimeSpan.FromSeconds(30),
                AllowAutoRedirect = false
            });

        services.AddTransient<WebhookExecutionService>();
        services.AddTransient<WebhookJobLockingService>();

        if (configuration.GetValue<bool>("UseInMemoryDatabase"))
        {
            services.AddDbContextFactory<WebhookContext>(options =>
                options.UseInMemoryDatabase("ApplicationContext"));
        }
        else
        {
            services.AddDbContextFactory<WebhookContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("ApplicationContext"),
                    o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery)
                ));
        }

        services.AddTransient<DropdownServices>();

        return services;
    }
}
