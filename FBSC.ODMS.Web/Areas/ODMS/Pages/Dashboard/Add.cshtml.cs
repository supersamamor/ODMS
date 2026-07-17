using FBSC.ODMS.Application.Features.ODMS.Dashboard.Commands;
using FBSC.ODMS.Web.Areas.ODMS.Models;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.Dashboard;

[Authorize(Policy = Permission.Dashboard.Create)]
public class AddModel : BasePageModel<AddModel>
{
    [BindProperty]
    public DashboardViewModel Dashboard { get; set; } = new();
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
		
        return await TryThenRedirectToPage(async () => await Mediatr.Send(Mapper.Map<AddDashboardCommand>(Dashboard)), "Details", true);
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
		if (AsyncAction == "AddDashboardWidget")
		{
			return AddDashboardWidget();
		}
		if (AsyncAction == "RemoveDashboardWidget")
		{
			return RemoveDashboardWidget();
		}
		if (AsyncAction == "AddDashboardAccess")
		{
			return AddDashboardAccess();
		}
		if (AsyncAction == "RemoveDashboardAccess")
		{
			return RemoveDashboardAccess();
		}
		
		
        return Partial("_InputFieldsPartial", Dashboard);
    }
	
	private PartialViewResult AddDashboardWidget()
	{
		ModelState.Clear();
		if (Dashboard!.DashboardWidgetList == null) { Dashboard!.DashboardWidgetList = []; }
		Dashboard!.DashboardWidgetList!.Add(new DashboardWidgetViewModel() { DrillDownDashboardId = Dashboard.Id });
		return Partial("_InputFieldsPartial", Dashboard);
	}
	private PartialViewResult RemoveDashboardWidget()
	{
		ModelState.Clear();
		Dashboard.DashboardWidgetList = [..Dashboard!.DashboardWidgetList!.Where(l => l.Id != RemoveSubDetailId)];
		return Partial("_InputFieldsPartial", Dashboard);
	}

	private PartialViewResult AddDrillDownDashboardWidget()
	{
		ModelState.Clear();
		if (Dashboard!.DashboardWidgetList == null) { Dashboard!.DashboardWidgetList = []; }
		Dashboard!.DashboardWidgetList!.Add(new DashboardWidgetViewModel() { DrillDownDashboardId = Dashboard.Id });
		return Partial("_InputFieldsPartial", Dashboard);
	}
	private PartialViewResult RemoveDrillDownDashboardWidget()
	{
		ModelState.Clear();
		Dashboard.DashboardWidgetList = [..Dashboard!.DashboardWidgetList!.Where(l => l.Id != RemoveSubDetailId)];
		return Partial("_InputFieldsPartial", Dashboard);
	}

	private PartialViewResult AddDashboardAccess()
	{
		ModelState.Clear();
		if (Dashboard!.DashboardAccessList == null) { Dashboard!.DashboardAccessList = []; }
		Dashboard!.DashboardAccessList!.Add(new DashboardAccessViewModel() { DashboardId = Dashboard.Id });
		return Partial("_InputFieldsPartial", Dashboard);
	}
	private PartialViewResult RemoveDashboardAccess()
	{
		ModelState.Clear();
		Dashboard.DashboardAccessList = [..Dashboard!.DashboardAccessList!.Where(l => l.Id != RemoveSubDetailId)];
		return Partial("_InputFieldsPartial", Dashboard);
	}
	
}
