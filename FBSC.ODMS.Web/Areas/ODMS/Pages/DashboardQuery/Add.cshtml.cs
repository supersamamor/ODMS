using FBSC.ODMS.Application.Features.ODMS.DashboardQuery.Commands;
using FBSC.ODMS.Web.Areas.ODMS.Models;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.DashboardQuery;

[Authorize(Policy = Permission.DashboardQuery.Create)]
public class AddModel : BasePageModel<AddModel>
{
    [BindProperty]
    public DashboardQueryViewModel DashboardQuery { get; set; } = new();
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
		
        return await TryThenRedirectToPage(async () => await Mediatr.Send(Mapper.Map<AddDashboardQueryCommand>(DashboardQuery)), "Details", true);
    }	
	public PartialViewResult OnPostChangeFormValue()
    {
        ModelState.Clear();
		if (AsyncAction == "AddDashboardQueryParameter")
		{
			return AddDashboardQueryParameter();
		}
		if (AsyncAction == "RemoveDashboardQueryParameter")
		{
			return RemoveDashboardQueryParameter();
		}
		if (AsyncAction == "AddDashboardQueryResultColumn")
		{
			return AddDashboardQueryResultColumn();
		}
		if (AsyncAction == "RemoveDashboardQueryResultColumn")
		{
			return RemoveDashboardQueryResultColumn();
		}
		if (AsyncAction == "AddDashboardQueryResultCache")
		{
			return AddDashboardQueryResultCache();
		}
		if (AsyncAction == "RemoveDashboardQueryResultCache")
		{
			return RemoveDashboardQueryResultCache();
		}
		if (AsyncAction == "AddDashboardWidget")
		{
			return AddDashboardWidget();
		}
		if (AsyncAction == "RemoveDashboardWidget")
		{
			return RemoveDashboardWidget();
		}
		if (AsyncAction == "AddDashboardRefreshJob")
		{
			return AddDashboardRefreshJob();
		}
		if (AsyncAction == "RemoveDashboardRefreshJob")
		{
			return RemoveDashboardRefreshJob();
		}
		
		
        return Partial("_InputFieldsPartial", DashboardQuery);
    }
	
	private PartialViewResult AddDashboardQueryParameter()
	{
		ModelState.Clear();
		if (DashboardQuery!.DashboardQueryParameterList == null) { DashboardQuery!.DashboardQueryParameterList = []; }
		DashboardQuery!.DashboardQueryParameterList!.Add(new DashboardQueryParameterViewModel() { DashboardQueryId = DashboardQuery.Id });
		return Partial("_InputFieldsPartial", DashboardQuery);
	}
	private PartialViewResult RemoveDashboardQueryParameter()
	{
		ModelState.Clear();
		DashboardQuery.DashboardQueryParameterList = [..DashboardQuery!.DashboardQueryParameterList!.Where(l => l.Id != RemoveSubDetailId)];
		return Partial("_InputFieldsPartial", DashboardQuery);
	}

	private PartialViewResult AddDashboardQueryResultColumn()
	{
		ModelState.Clear();
		if (DashboardQuery!.DashboardQueryResultColumnList == null) { DashboardQuery!.DashboardQueryResultColumnList = []; }
		DashboardQuery!.DashboardQueryResultColumnList!.Add(new DashboardQueryResultColumnViewModel() { DashboardQueryId = DashboardQuery.Id });
		return Partial("_InputFieldsPartial", DashboardQuery);
	}
	private PartialViewResult RemoveDashboardQueryResultColumn()
	{
		ModelState.Clear();
		DashboardQuery.DashboardQueryResultColumnList = [..DashboardQuery!.DashboardQueryResultColumnList!.Where(l => l.Id != RemoveSubDetailId)];
		return Partial("_InputFieldsPartial", DashboardQuery);
	}

	private PartialViewResult AddDashboardQueryResultCache()
	{
		ModelState.Clear();
		if (DashboardQuery!.DashboardQueryResultCacheList == null) { DashboardQuery!.DashboardQueryResultCacheList = []; }
		DashboardQuery!.DashboardQueryResultCacheList!.Add(new DashboardQueryResultCacheViewModel() { DashboardQueryId = DashboardQuery.Id });
		return Partial("_InputFieldsPartial", DashboardQuery);
	}
	private PartialViewResult RemoveDashboardQueryResultCache()
	{
		ModelState.Clear();
		DashboardQuery.DashboardQueryResultCacheList = [..DashboardQuery!.DashboardQueryResultCacheList!.Where(l => l.Id != RemoveSubDetailId)];
		return Partial("_InputFieldsPartial", DashboardQuery);
	}

	private PartialViewResult AddDashboardWidget()
	{
		ModelState.Clear();
		if (DashboardQuery!.DashboardWidgetList == null) { DashboardQuery!.DashboardWidgetList = []; }
		DashboardQuery!.DashboardWidgetList!.Add(new DashboardWidgetViewModel() { DashboardQueryId = DashboardQuery.Id });
		return Partial("_InputFieldsPartial", DashboardQuery);
	}
	private PartialViewResult RemoveDashboardWidget()
	{
		ModelState.Clear();
		DashboardQuery.DashboardWidgetList = [..DashboardQuery!.DashboardWidgetList!.Where(l => l.Id != RemoveSubDetailId)];
		return Partial("_InputFieldsPartial", DashboardQuery);
	}

	private PartialViewResult AddDashboardRefreshJob()
	{
		ModelState.Clear();
		if (DashboardQuery!.DashboardRefreshJobList == null) { DashboardQuery!.DashboardRefreshJobList = []; }
		DashboardQuery!.DashboardRefreshJobList!.Add(new DashboardRefreshJobViewModel() { DashboardQueryId = DashboardQuery.Id });
		return Partial("_InputFieldsPartial", DashboardQuery);
	}
	private PartialViewResult RemoveDashboardRefreshJob()
	{
		ModelState.Clear();
		DashboardQuery.DashboardRefreshJobList = [..DashboardQuery!.DashboardRefreshJobList!.Where(l => l.Id != RemoveSubDetailId)];
		return Partial("_InputFieldsPartial", DashboardQuery);
	}
	
}
