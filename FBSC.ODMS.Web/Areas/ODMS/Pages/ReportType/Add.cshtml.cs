using FBSC.ODMS.Application.Features.ODMS.ReportType.Commands;
using FBSC.ODMS.Web.Areas.ODMS.Models;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.ReportType;

[Authorize(Policy = Permission.ReportType.Create)]
public class AddModel : BasePageModel<AddModel>
{
    [BindProperty]
    public ReportTypeViewModel ReportType { get; set; } = new();
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
		
        if (!ModelState.IsValid)
        {
            return Page();
        }
		
        return await TryThenRedirectToPage(async () => await Mediatr.Send(Mapper.Map<AddReportTypeCommand>(ReportType)), "Details", true);
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
