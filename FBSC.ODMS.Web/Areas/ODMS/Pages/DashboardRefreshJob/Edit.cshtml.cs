using FBSC.ODMS.Application.Features.ODMS.DashboardRefreshJob.Commands;
using FBSC.ODMS.Application.Features.ODMS.DashboardRefreshJob.Queries;
using FBSC.ODMS.Web.Areas.ODMS.Models;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.DashboardRefreshJob;

[Authorize(Policy = Permission.DashboardRefreshJob.Edit)]
public class EditModel : BasePageModel<EditModel>
{
    [BindProperty]
    public DashboardRefreshJobViewModel DashboardRefreshJob { get; set; } = new();
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
        return await PageFrom(async () => await Mediatr.Send(new GetDashboardRefreshJobByIdQuery(id)), DashboardRefreshJob);
    }

    public async Task<IActionResult> OnPost()
    {		
        if (!ModelState.IsValid)
        {
            return Page();
        }
		
        return await TryThenRedirectToPage(async () => await Mediatr.Send(Mapper.Map<EditDashboardRefreshJobCommand>(DashboardRefreshJob)), "Details", true);
    }	
	public PartialViewResult OnPostChangeFormValue()
    {
        ModelState.Clear();
		
        return Partial("_InputFieldsPartial", DashboardRefreshJob);
    }
	
}
