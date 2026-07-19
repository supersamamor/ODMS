using FBSC.ODMS.ExcelProcessor.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FBSC.ODMS.ExcelProcessor
{
    public static class ServiceExtensions
    {
        public static void AddExcelProcessor(this IServiceCollection services)
        {
            services.AddTransient<ExcelService>();
        }
    }
}
