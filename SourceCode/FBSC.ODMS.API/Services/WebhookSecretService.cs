using FBSC.ODMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace FBSC.ODMS.API.Services
{
    public interface IWebhookSecretService
    {
        Task<string?> GetClientSecretAsync(string? clientId);
    }
    public class WebhookSecretService(IMemoryCache cache, IdentityContext dbContext) : IWebhookSecretService
    {
        public async Task<string?> GetClientSecretAsync(string? clientId)
        {
            // 1. Guard Clause: If the ID is null or whitespace, we cannot look it up.
            // Return null immediately to save a cache lookup and a database roundtrip.
            if (string.IsNullOrWhiteSpace(clientId))
            {
                return null;
            }
            var cacheKey = $"WebhookSecret_{clientId}";
            return await cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                // Cache for 1 hour
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                // 2. Querying with a guaranteed non-null clientId
                var application = await dbContext.OpenIddictApplications
                    .AsNoTracking()
                    .FirstOrDefaultAsync(app => app.ClientId == clientId);
                return application?.WebhookHmacSecret;
            });
        }
    }
}
