using FBSC.Common.Services.Shared.Interfaces;
using FBSC.ODMS.EmailSending.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace FBSC.ODMS.EmailSending
{
    public static class ServiceCollectionExtensions
    {
        public static void AddEmailSendingAService(this IServiceCollection services, IConfiguration configuration)
		{
			services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
			if ((configuration["MailSettings:SendingType"]!.ToUpper()).Equals(SendingType.SMTP.ToString(), StringComparison.CurrentCultureIgnoreCase))
			{
				services.AddTransient<IMailService, SMTPEmailService>();
			}
			else if (configuration["MailSettings:SendingType"]!.Equals(SendingType.OneMessage.ToString(), StringComparison.CurrentCultureIgnoreCase))
			{
				services.AddTransient<IMailService, OneMessageEmailServiceApi>();
				services.AddHttpClient<IMailService, OneMessageEmailServiceApi>(c =>
				{
					c.BaseAddress = new Uri(configuration.GetValue<string>("MailSettings:EmailApiUrl")!);
				});
			}
		}
    }
}
