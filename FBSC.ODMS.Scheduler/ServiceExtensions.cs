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

					// Registered here (code-based trigger) rather than via the XML job-store
					// plugin used by the other jobs below, since this job's cadence is a fixed
					// polling interval rather than a per-environment cron schedule.
					var dashboardCacheRefreshJobKey = new JobKey(nameof(DashboardCacheRefreshJob));
					q.AddJob<DashboardCacheRefreshJob>(opts => opts.WithIdentity(dashboardCacheRefreshJobKey));
					q.AddTrigger(opts => opts
						.ForJob(dashboardCacheRefreshJobKey)
						.WithIdentity($"{nameof(DashboardCacheRefreshJob)}-trigger")
						.WithSimpleSchedule(s => s.WithIntervalInMinutes(1).RepeatForever())
						.StartAt(DateBuilder.FutureDate(30, IntervalUnit.Second)));
				});
				services.AddQuartzServer(options =>
				{
					options.WaitForJobsToComplete = true;
				});
				services.AddTransient<FileScanJob>();
				services.AddTransient<ApprovalNotificationJob>();
				services.AddTransient<BatchUploadJob>();
				services.AddTransient<DashboardCacheRefreshJob>();
            }            
        }
    }
}
