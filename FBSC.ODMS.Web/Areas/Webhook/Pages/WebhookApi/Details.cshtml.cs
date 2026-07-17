using FBSC.ODMS.Web.Models;
using FBSC.ApiHub.Features.WebhookApi.Queries;
using FBSC.ApiHub.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Areas.Webhook.Pages.WebhookApi;

[Authorize(Policy = FBSC.ApiHub.WebhookPermission.WebhookApi.View)]
public class DetailsModel : BasePageModel<DetailsModel>
{
    public WebhookApiViewModel WebhookApi { get; set; } = new() { IsEdit = true };
    [BindProperty]
    public string? RemoveSubDetailId { get; set; }
    [BindProperty]
    public string? AsyncAction { get; set; }
    public async Task<IActionResult> OnGet(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        return await PageFrom(async () => await Mediatr.Send(new GetWebhookApiByIdQuery(id)), WebhookApi);
    }
}
