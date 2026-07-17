using FBSC.ODMS.Web.Models;
using FBSC.ApiHub.Features.WebhookApi.Queries;
using FBSC.ApiHub.Features.WebhookLogs.Commands;
using FBSC.ApiHub.Features.WebhookLogs.Queries;
using FBSC.ApiHub.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FBSC.ODMS.Web.Attributes;


namespace FBSC.ODMS.Web.Areas.Webhook.Pages.WebhookApi;

[Authorize(Policy = ApiHub.WebhookPermission.WebhookApi.Logs)]
public class LogsModel : BasePageModel<LogsModel>
{
    public WebhookLogsViewModel WebhookLogs { get; set; } = new();

    [DataTablesRequest]
    public DataTablesRequest? DataRequest { get; set; }
    [BindProperty]
    public string WebhookApiId { get; set; } = "";
    [BindProperty]
    public string WebhookApiName { get; set; } = "";
    public async Task<IActionResult> OnGetAsync(string webhookApiId)
    {
        WebhookApiId = webhookApiId;
        _ = (await Mediatr.Send(new GetWebhookApiByIdQuery(webhookApiId))).Select(l => WebhookApiName = l.Name);
        return Page();
    }

    public async Task<IActionResult> OnPostListAllAsync(string webhookApiId)
    {
        var dataRequest = DataRequest!.ToQuery<GetWebhookLogsQuery>();
        dataRequest.WebhookApiId = webhookApiId;
        var result = await Mediatr.Send(dataRequest);       
        return new JsonResult(result.Data.ToDataTablesResponse(DataRequest, result.TotalCount, result.Data.TotalItemCount));
    }

    public async Task<IActionResult> OnGetReprocess(string webhookLogsId, string webhookApiId)
    {
        WebhookApiId = webhookApiId;
        if (!ModelState.IsValid)
        {
            return Page();
        }
        await Mediatr.Send(new ResendWebhookLogsCommand(webhookLogsId));
        NotyfService.Success(Localizer["Transaction successful"]);
        return RedirectToPage("Logs", new { webhookApiId });
    }
}
