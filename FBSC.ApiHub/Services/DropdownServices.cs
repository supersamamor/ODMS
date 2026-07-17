using FBSC.Common.Data;
using FBSC.ApiHub.Context;
using FBSC.ApiHub.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FBSC.ApiHub.Services
{
    public class DropdownServices(WebhookContext context)
    {
        public SelectList GetWebhookEventAssignmentList(string? id)
        {
            return context.GetSingle<WebhookEventAssignmentState>(e => e.Id == id, new()).Result.Match(
                Some: e => new SelectList(new List<SelectListItem> { new() { Value = e.Id, Text = e.EventName } }, "Value", "Text", e.EventName),
                None: () => new SelectList(new List<SelectListItem>(), "Value", "Text")
            );
        }
        public static IEnumerable<SelectListItem> GrantTypeList()
        {
            IList<SelectListItem> items =
            [
                new() { Text = GrantType.ClientCredentials, Value = GrantType.ClientCredentials, },
                new() { Text = GrantType.BasicAuthentication, Value = GrantType.BasicAuthentication, },
                new() { Text = GrantType.BearerToken, Value = GrantType.BearerToken, },
                new() { Text = GrantType.ApiKey, Value = GrantType.ApiKey, },          
                new() { Text = GrantType.None, Value = GrantType.None, },
            ];
            return items;
        }   
        public static IEnumerable<SelectListItem> HttpMethodList()
        {
            IList<SelectListItem> items =
            [
                new() { Text = HttpMethod.Post.ToString(), Value =  HttpMethod.Post.ToString(), },
                new() { Text = HttpMethod.Put.ToString(), Value =  HttpMethod.Put.ToString(), },
                new() { Text = HttpMethod.Get.ToString(), Value =  HttpMethod.Get.ToString(), },
            ];
            return items;
        }
    }
}