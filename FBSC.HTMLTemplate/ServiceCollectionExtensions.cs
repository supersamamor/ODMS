using FBSC.HTMLTemplate.Context;
using FBSC.HTMLTemplate.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
namespace FBSC.HTMLTemplate;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHTMLTemplateServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssemblies([Assembly.GetExecutingAssembly()]);
        });
        services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());
        services.AddTransient<RotativaService>();
        if (configuration.GetValue<bool>("UseInMemoryDatabase"))
        {
            services.AddDbContext<HTMLTemplateContext>(options =>
                options.UseInMemoryDatabase("ApplicationContext"));
        }
        else
        {
            services.AddDbContext<HTMLTemplateContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("ApplicationContext"),
                                     o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery)));
        }
        return services;
    }
}
