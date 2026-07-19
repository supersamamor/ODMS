using FBSC.ApiHub.Context;
using FBSC.ApiHub.Models;
using FBSC.Common.Identity.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FBSC.ODMS.Web.Areas.Identity.Data;

public static class DefaultApiHub
{
    public static async Task Seed(IServiceProvider serviceProvider)
    {
        using var context = new WebhookContext(serviceProvider.GetRequiredService<DbContextOptions<WebhookContext>>(), serviceProvider.GetRequiredService<IAuthenticatedUser>());
        var entity = await context.WebhookApi.FirstOrDefaultAsync(e => e.Name == "Generative AI Integration");
        if (entity == null)
        {
            var webhookApi = new WebhookApiState
            {
                Id = "bb34a4d7-a404-43cf-a46d-f6c884f91163", // Inherited from BaseEntity
                Name = "Generative AI Integration",
                ClientId = "Generative AI Integration",
                GrantType = GrantType.ApiKey,
                BaseUrl = "https://generativelanguage.googleapis.com/v1beta/models",
                AuthenticationUrl = null,
                WithinPrivateNetwork = false,
                Scope = "ai.generate",
                Active = true,

                // Utilizing Raw String Literals for clean, enterprise-grade JSON configuration handling                
                AdditionalConfigurationJson = """
                {
                    "contents": [
                    {
                        "role": "user",
                        "parts": [
                        {
                            "text": "{PromptMessage}"
                        }
                        ]
                    }
                    ],
                    "generationConfig": {
                    "maxOutputTokens": 5000,
                    "temperature": 0.7,
                    "topP": 0.9,
                    "candidateCount": 1
                    }
                }
                """,
                // Utilizing C# 12 Collection Expressions for related entity initialization
                WebhookEventAssignmentList =
                [
                    new WebhookEventAssignmentState
                    {
                        Id = Guid.NewGuid().ToString(),
                        EventName = "Prompt Generative AI",
                        Route = "gemini-3-flash-preview:generateContent",
                        Method = "POST",
                        Active = true
                    },
                ]
            };   
            context.WebhookApi.Add(webhookApi);
            // Note: ClientSecret and HMAC have `private set` modifiers. 
            // If these need to be initialized at creation, they must be set via their respective 
            // Domain/Helper methods (e.g., EncryptClientSecret) rather than object initializers.
            await context.SaveChangesAsync();


        }
    }
}
