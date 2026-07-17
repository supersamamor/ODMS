using FBSC.Common.Services.Shared.Interfaces;
using FBSC.Common.Services.Shared.Models.Mail;
using FBSC.ODMS.EmailSending.Exceptions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace FBSC.ODMS.EmailSending.Services
{
    public class OneMessageEmailServiceApi(IOptions<MailSettings> emailApiConfig, IHttpClientFactory httpClientFactory) : IMailService
    {
        private readonly MailSettings _emailApiConfig = emailApiConfig.Value;

        public async Task SendAsync(MailRequest request, CancellationToken cancellationToken = default)
        {
            var client = httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", await GetJwTokenAsync(client, new CancellationToken())));
            var emailRequest = new EmailRequest { From = _emailApiConfig.EmailApiSender, To = [request.To], Text = request.Body };
            var content = new StringContent(JsonConvert.SerializeObject(emailRequest), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(_emailApiConfig.EmailApiUrl + "/api/InfoBip/sms", content, cancellationToken: cancellationToken);
            var result = await response.Content.ReadAsStringAsync(cancellationToken);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch
            {
                throw new ApiResponseException(result, response);
            }
            await response.Content.ReadAsStringAsync(cancellationToken);
        }

        private async Task<string> GetJwTokenAsync(HttpClient client, CancellationToken token)
        {
            var response = await client.PostAsync(_emailApiConfig.EmailApiUrl + $"/token?username={_emailApiConfig.EmailApiUsername}&password={_emailApiConfig.EmailApiPassword}", new StringContent(""), token);
            var result = await response.Content.ReadAsStringAsync(token);
            response.EnsureSuccessStatusCode();
            return result;
        }

        private class EmailRequest
        {
            public string? Text { get; set; }
            public List<string>? To { get; set; }
            public string? From { get; set; }
        }
    }
}
