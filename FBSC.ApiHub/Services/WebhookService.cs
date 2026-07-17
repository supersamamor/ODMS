using FBSC.ApiHub.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
namespace FBSC.ApiHub.Services
{
    public class WebHookService(
        IDbContextFactory<WebhookContext> contextFactory,
        HttpClient httpClient,
        IConfiguration configuration)
        : BaseWebHookService(contextFactory, httpClient, configuration)
    {
    }

    public class WebHookWithoutSSLValidationService : BaseWebHookService
    {
        public WebHookWithoutSSLValidationService(
            IDbContextFactory<WebhookContext> contextFactory,
            HttpClient httpClient,
            IConfiguration configuration)
            : base(contextFactory, httpClient, configuration)
        {
            // Failsafe guard against accidental production injection
            if (!configuration.GetValue<bool>("IsTest"))
            {
                //Todo: Enable this on BGC Template
                //throw new InvalidOperationException("CRITICAL: SSL Bypass service cannot be instantiated in a production environment.");
            }
        }
    }
}