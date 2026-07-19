using FBSC.ODMS.Web.Models;
using FBSC.ApiHub.Features.WebhookApi.Commands;
using FBSC.ApiHub.Features.WebhookApi.Queries;
using FBSC.ApiHub.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FBSC.Common.Utility.Extensions;

namespace FBSC.ODMS.Web.Areas.Webhook.Pages.WebhookApi;

[Authorize(Policy =  ApiHub.WebhookPermission.WebhookApi.Edit)]
public class EditModel : BasePageModel<EditModel>
{
    [BindProperty]
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
    public async Task<IActionResult> OnPost()
    {
        WebhookApi.AdditionalConfigurationJson?.ValidateJson(ModelState);
        if (!ModelState.IsValid)
        {
            return Page();
        }
        return await TryThenRedirectToPage(async () => await Mediatr.Send(Mapper.Map<EditWebhookApiCommand>(WebhookApi)), "Details", true);
    }
    public IActionResult OnPostChangeFormValue()
    {
        ModelState.Clear();
        if (AsyncAction == "AddWebhookEventAssignment")
        {
            return AddWebhookEventAssignment();
        }
        if (AsyncAction == "RemoveWebhookEventAssignment")
        {
            return RemoveWebhookEventAssignment();
        }


        return Partial("_InputFieldsPartial", WebhookApi);
    }

    private IActionResult AddWebhookEventAssignment()
    {
        ModelState.Clear();
        if (WebhookApi!.WebhookEventAssignmentList == null) { WebhookApi!.WebhookEventAssignmentList = new List<WebhookEventAssignmentViewModel>(); }
        WebhookApi!.WebhookEventAssignmentList!.Add(new WebhookEventAssignmentViewModel() { WebhookApiId = WebhookApi.Id, NewRecord = true });
        return Partial("_InputFieldsPartial", WebhookApi);
    }
    private IActionResult RemoveWebhookEventAssignment()
    {
        ModelState.Clear();
        WebhookApi.WebhookEventAssignmentList = WebhookApi!.WebhookEventAssignmentList!.Where(l => l.Id != RemoveSubDetailId).ToList();
        return Partial("_InputFieldsPartial", WebhookApi);
    }

}
