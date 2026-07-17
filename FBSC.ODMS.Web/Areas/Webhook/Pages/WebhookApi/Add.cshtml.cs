using FBSC.ApiHub.Features.WebhookApi.Commands;
using FBSC.ApiHub.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FBSC.ODMS.Web.Models;
using FBSC.Common.Utility.Extensions;

namespace FBSC.ODMS.Web.Areas.Webhook.Pages.WebhookApi;

[Authorize(Policy = FBSC.ApiHub.WebhookPermission.WebhookApi.Create)]
public class AddModel : BasePageModel<AddModel>
{
    [BindProperty]
    public WebhookApiViewModel WebhookApi { get; set; } = new();
    [BindProperty]
    public string? RemoveSubDetailId { get; set; }
    [BindProperty]
    public string? AsyncAction { get; set; }
    public IActionResult OnGet()
    {
		
        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
		WebhookApi.AdditionalConfigurationJson?.ValidateJson(ModelState);
        // 3. Standard Validation Check
        // If the JSON was invalid, ModelState.IsValid will be false, 
        // and returning Page() pushes the errors straight to your UI.
        if (!ModelState.IsValid)
        {
            return Page();
        }
        return await TryThenRedirectToPage(async () => await Mediatr.Send(Mapper.Map<AddWebhookApiCommand>(WebhookApi)), "Details", true);
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
