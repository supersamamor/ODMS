using FBSC.ODMS.Scheduler.Jobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.AspNetCore;
using Quartz.Job;
using Quartz.Simpl;
namespace FBSC.ODMS.Scheduler
{
    public static class ServiceExtensions
    {
        public static void AddScheduler(this IServiceCollection services, IConfiguration config)
        {
			if (config.GetValue<bool>("EnableQuartzJob"))
            {
				services.Configure<QuartzOptions>(config.GetSection("Quartz"));
				services.AddQuartz(q =>
				{
					q.UseJobFactory<MicrosoftDependencyInjectionJobFactory>();
					q.UseSimpleTypeLoader();
					q.UseInMemoryStore();
				});
				services.AddQuartzServer(options =>
				{
					options.WaitForJobsToComplete = true;
				});
				services.AddTransient<FileScanJob>();
				services.AddTransient<ApprovalNotificationJob>();
				services.AddTransient<BatchUploadJob>();
            }            
        }
    }
}
