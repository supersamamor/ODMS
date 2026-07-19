using FBSC.ApiHub.Context;
using FBSC.ApiHub.Models;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace FBSC.ApiHub.Services
{
    public class BaseWebHookService(
        IDbContextFactory<WebhookContext> contextFactory,
        HttpClient httpClient,
        IConfiguration configuration)
    {
        public async Task<string> ExecuteWebhook(
            WebhookApiState webhookApi,
            WebhookEventAssignmentState webhookEvent,
            string? payload,
            string apiRoute,
            CancellationToken token)
        {
            // Strict null validation
            ArgumentNullException.ThrowIfNull(webhookApi);
            ArgumentNullException.ThrowIfNull(webhookEvent);
            ArgumentException.ThrowIfNullOrWhiteSpace(apiRoute);

            // Modern pattern matching for HTTP Method
            HttpMethod method = webhookEvent.Method?.ToUpperInvariant() switch
            {
                "POST" => HttpMethod.Post,
                "PUT" => HttpMethod.Put,
                "GET" => HttpMethod.Get,
                "DELETE" => HttpMethod.Delete,
                "PATCH" => HttpMethod.Patch,
                _ => HttpMethod.Get,
            };

            // Safe URI Construction (Prevents double slashes)
            string baseUrl = webhookApi.BaseUrl?.TrimEnd('/') ?? string.Empty;
            string safeRoute = apiRoute.TrimStart('/');

            // API Key Query String Safety Check
            if (webhookApi.GrantType == GrantType.ApiKey)
            {
                var apiKey = GetApiKey(webhookApi);
                string separator = safeRoute.Contains('?') ? "&" : "?";
                safeRoute = $"{safeRoute}{separator}key={apiKey}";
            }

            var requestUri = new Uri($"{baseUrl}/{safeRoute}");

            using var request = new HttpRequestMessage
            {
                Method = method,
                RequestUri = requestUri
            };

            // Content Injection
            if (method != HttpMethod.Get && !string.IsNullOrEmpty(payload))
            {
                request.Content = new StringContent(payload, Encoding.UTF8, "application/json");
            }

            // HMAC Outbound Payload Structuring
            if (!string.IsNullOrEmpty(payload))
            {
                string keyPrefix = configuration.GetValue<string>("EncryptionDecryptionKeyPrefix")
                    ?? throw new InvalidOperationException("EncryptionDecryptionKeyPrefix is missing from configuration.");

                string decryptedHmacKey = webhookApi.GetDecryptedHMAC(keyPrefix);

                if (!string.IsNullOrEmpty(decryptedHmacKey))
                {
                    // CRITICAL: Ensure this matches what the receiver validates! 
                    // If the receiver expects just the payload, keep this as is. 
                    // If they expect Method/Route/Payload, change it back to the multi-line string.
                    string signature = GenerateHmacSignature(payload, decryptedHmacKey);
                    request.Headers.Add("X-HMAC-Signature", signature);
                }
            }

            // Modern Authentication Switch
            switch (webhookApi.GrantType)
            {
                case GrantType.ClientCredentials:
                    var jwToken = await GetAccessTokenAsync(webhookApi, token);
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwToken?.AccessToken);
                    break;

                case GrantType.BasicAuthentication:
                    var encodedCredentials = GetBasicAuthentication(webhookApi);
                    request.Headers.Authorization = new AuthenticationHeaderValue("Basic", encodedCredentials);
                    break;

                case GrantType.BearerToken:
                    var bearerToken = GetBearerToken(webhookApi);
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
                    break;

                    // ApiKey is already handled above in the URI construction
            }

            using var response = await httpClient.SendAsync(request, token);

            // Check for unhandled status codes gracefully
            if (!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync(token);
                throw new WebhookApiResponseException(response, errorContent);
            }

            return await response.Content.ReadAsStringAsync(token);
        }
        private static string GenerateHmacSignature(string payload, string secretKey)
        {
            // Defensive programming
            ArgumentException.ThrowIfNullOrWhiteSpace(payload);
            ArgumentException.ThrowIfNullOrWhiteSpace(secretKey);

            byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);
            byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);

            using var hmac = new HMACSHA256(keyBytes);
            byte[] hash = hmac.ComputeHash(payloadBytes);

            return Convert.ToBase64String(hash);
        }
        protected async Task<WebhookJwToken?> GetAccessTokenAsync(WebhookApiState webhook, CancellationToken token)
        {
            WebhookJwToken? jwToken = new(webhook);

            if (webhook.IsAccessTokenExpired())
            {
                jwToken = webhook.IsRefreshTokenExpired() || string.IsNullOrEmpty(webhook.RefreshToken)
                    ? await RequestNewTokenAsync(webhook, token)
                    : await RefreshJwTokenAsync(webhook, token);

                if (jwToken != null)
                {
                    await SaveWebHookAsync(webhook, jwToken, token);
                }
            }

            return jwToken;
        }

        protected string GetBasicAuthentication(WebhookApiState webhook)
        {
            string credentials = $"{webhook.ClientId}:{webhook.GetDecryptedClientSecret(configuration.GetValue<string>("EncryptionDecryptionKeyPrefix")!)}";
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));
        }

        protected string GetBearerToken(WebhookApiState webhook)
        {
            return webhook.GetDecryptedClientSecret(configuration.GetValue<string>("EncryptionDecryptionKeyPrefix")!);
        }
        protected string GetApiKey(WebhookApiState webhook)
        {
            return webhook.GetDecryptedClientSecret(configuration.GetValue<string>("EncryptionDecryptionKeyPrefix")!);
        }

        protected async Task<WebhookJwToken?> RefreshJwTokenAsync(WebhookApiState webhook, CancellationToken token)
        {
            var headers = new List<KeyValuePair<string?, string?>>
            {
                new("grant_type", "refresh_token"),
                new("client_id", webhook.ClientId),
                new("client_secret", webhook.GetDecryptedClientSecret(configuration.GetValue<string>("EncryptionDecryptionKeyPrefix")!)),
                new("refresh_token", webhook.RefreshToken)
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"{webhook.AuthenticationUrl}/connect/token")
            {
                Content = new FormUrlEncodedContent(headers)
            };

            var response = await httpClient.SendAsync(request, token);
            var result = await response.Content.ReadAsStringAsync(token);
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<WebhookJwToken>(result);
        }

        protected async Task<WebhookJwToken?> RequestNewTokenAsync(WebhookApiState webhook, CancellationToken token)
        {
            var headers = new List<KeyValuePair<string?, string?>>
            {
                new("grant_type", webhook.GrantType),
                new("client_id", webhook.ClientId),
                new("client_secret", webhook.GetDecryptedClientSecret(configuration.GetValue<string>("EncryptionDecryptionKeyPrefix")!)),
                new("scope", webhook.Scope)
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"{webhook.AuthenticationUrl}/connect/token")
            {
                Content = new FormUrlEncodedContent(headers)
            };

            var response = await httpClient.SendAsync(request, token);
            var result = await response.Content.ReadAsStringAsync(token);
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<WebhookJwToken>(result);
        }

        protected async Task SaveWebHookAsync(WebhookApiState webhook, WebhookJwToken jwToken, CancellationToken token)
        {
            using var context = await contextFactory.CreateDbContextAsync(token);

            var dbEntity = await context.WebhookApi
                .FirstOrDefaultAsync(w => w.Id == webhook.Id, token);

            if (dbEntity != null)
            {
                dbEntity.UpdateJwToken(
                    jwToken.ExpiresIn,
                    jwToken.RefreshExpiresIn,
                    jwToken.RequestDateTime,
                    jwToken.AccessToken,
                    jwToken.RefreshToken);

                await context.SaveChangesAsync(token);
            }
        }
    }
    public class WebhookJwToken
    {
        public WebhookJwToken() { }

        public WebhookJwToken(WebhookApiState webhook)
        {
            this.ExpiresIn = webhook.ExpiresIn != null ? webhook.ExpiresIn.Value : 0;
            this.RefreshExpiresIn = webhook.RefreshExpiresIn != null ? webhook.RefreshExpiresIn.Value : 0;
            this.RequestDateTime = webhook.RequestDateTime;
            this.AccessToken = webhook.GetDecryptedAccessToken();
            this.RefreshToken = webhook.GetDecryptedRefreshToken();
        }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; } = "";
        [JsonProperty("token_type")]
        public string TokenType { get; set; } = "";
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
        [JsonProperty("id_token")]
        public string IdToken { get; set; } = "";
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; } = "";
        public DateTime? RequestDateTime { get; set; } = DateTime.UtcNow;
        [JsonProperty("refresh_expires_in")]
        public int RefreshExpiresIn { get; set; }
    }

    public class WebhookErrorModel
    {
        [JsonProperty("type")]
        public string Type { get; set; } = "";
        [JsonProperty("title")]
        public string Title { get; set; } = "";
        [JsonProperty("status")]
        public int Status { get; set; }
        [JsonProperty("traceId")]
        public string TraceId { get; set; } = "";
    }

    public class PagedListApiResponse<T>
    {
        public MetaData MetaData { get; set; } = new MetaData();
        public IList<T> Data { get; set; } = [];
        public int TotalCount => Data.Count;
    }

    public class MetaData
    {
        public int PageCount { get; set; }
        public int TotalItemCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
        public bool IsFirstPage { get; set; }
        public bool IsLastPage { get; set; }
        public int FirstItemOnPage { get; set; }
        public int LastItemOnPage { get; set; }
    }

    public class WebhookApiResponseException : Exception
    {
        public string? WebhookApiResponse { get; set; } = "";

        public WebhookApiResponseException() { }

        public WebhookApiResponseException(HttpResponseMessage response, string? apiResult = "")
            : base(string.IsNullOrEmpty(apiResult) ? response.ToString() : apiResult)
        {
            WebhookApiResponse = apiResult;
        }

        public WebhookApiResponseException(string? message, Exception inner)
            : base(message, inner) { }
    }
}