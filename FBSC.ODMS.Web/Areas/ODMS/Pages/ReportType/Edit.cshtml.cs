using FBSC.ODMS.Application.Features.ODMS.ReportType.Commands;
using FBSC.ODMS.Application.Features.ODMS.ReportType.Queries;
using FBSC.ODMS.Web.Areas.ODMS.Models;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.ReportType;

[Authorize(Policy = Permission.ReportType.Edit)]
public class EditModel : BasePageModel<EditModel>
{
    [BindProperty]
    public ReportTypeViewModel ReportType { get; set; } = new();
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
        return await PageFrom(async () => await Mediatr.Send(new GetReportTypeByIdQuery(id)), ReportType);
    }

    public async Task<IActionResult> OnPost()
    {		
        if (!ModelState.IsValid)
        {
            return Page();
        }
		
        return await TryThenRedirectToPage(async () => await Mediatr.Send(Mapper.Map<EditReportTypeCommand>(ReportType)), "Details", true);
    }	
	public PartialViewResult OnPostChangeFormValue()
    {
        ModelState.Clear();
		if (AsyncAction == "AddDashboardWidget")
		{
			return AddDashboardWidget();
		}
		if (AsyncAction == "RemoveDashboardWidget")
		{
			return RemoveDashboardWidget();
		}
		
		
        return Partial("_InputFieldsPartial", ReportType);
    }
	
	private PartialViewResult AddDashboardWidget()
	{
		ModelState.Clear();
		if (ReportType!.DashboardWidgetList == null) { ReportType!.DashboardWidgetList = []; }
		ReportType!.DashboardWidgetList!.Add(new DashboardWidgetViewModel() { ReportTypeId = ReportType.Id });
		return Partial("_InputFieldsPartial", ReportType);
	}
	private PartialViewResult RemoveDashboardWidget()
	{
		ModelState.Clear();
		ReportType.DashboardWidgetList = [..ReportType!.DashboardWidgetList!.Where(l => l.Id != RemoveSubDetailId)];
		return Partial("_InputFieldsPartial", ReportType);
	}
	
}
