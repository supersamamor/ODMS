using FBSC.ODMS.Web.Models;
using FBSC.ApiHub.Features.WebhookLogs.Queries;
using FBSC.ApiHub.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Areas.Webhook.Pages.WebhookApi;

[Authorize(Policy = FBSC.ApiHub.WebhookPermission.WebhookLogs.View)]
public class ViewLogModel : BasePageModel<ViewLogModel>
{
    public WebhookLogsViewModel WebhookLogs { get; set; } = new();
    [BindProperty]
    public string WebhookApiId { get; set; } = string.Empty;
    public async Task<IActionResult> OnGet(string? id, string webhookApiId)
    {
        if (id == null)
        {
            return NotFound();
        }
        WebhookApiId =  webhookApiId;
        return await PageFrom(async () => await Mediatr.Send(new GetWebhookLogsByIdQuery(id)), WebhookLogs);
    }
}
