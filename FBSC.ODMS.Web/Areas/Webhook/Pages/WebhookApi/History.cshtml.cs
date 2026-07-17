using FBSC.ODMS.Web.Areas.Admin.Models;
using FBSC.ODMS.Web.Areas.Admin.Queries.AuditTrail;
using FBSC.ODMS.Web.Models;
using FBSC.ApiHub.Features.WebhookApi.Queries;
using FBSC.ApiHub.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Areas.Webhook.Pages.WebhookApi;

[Authorize(Policy =  FBSC.ApiHub.WebhookPermission.WebhookApi.History)]
public class HistoryModel : BasePageModel<HistoryModel>
{
    public IList<AuditLogViewModel> AuditLogList { get; set; } = new List<AuditLogViewModel>();
    public WebhookApiViewModel WebhookApi { get; set; } = new();
    public async Task<IActionResult> OnGet(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        _ = (await Mediatr.Send(new GetWebhookApiByIdQuery(id))).Select(l=> Mapper.Map(l, WebhookApi));  
        AuditLogList = await Mediatr.Send(new GetAuditLogsByPrimaryKeyQuery(id));
        return Page();
    }
}
