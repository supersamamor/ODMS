using FBSC.ODMS.Application.Features.ODMS.DashboardRefreshJob.Commands;
using FBSC.ODMS.Web.Areas.ODMS.Models;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.DashboardRefreshJob;

[Authorize(Policy = Permission.DashboardRefreshJob.Create)]
public class AddModel : BasePageModel<AddModel>
{
    [BindProperty]
    public DashboardRefreshJobViewModel DashboardRefreshJob { get; set; } = new();
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
		
        return await TryThenRedirectToPage(async () => await Mediatr.Send(Mapper.Map<AddDashboardRefreshJobCommand>(DashboardRefreshJob)), "Details", true);
    }	
	public PartialViewResult OnPostChangeFormValue()
    {
        ModelState.Clear();
		
        return Partial("_InputFieldsPartial", DashboardRefreshJob);
    }
	
}
