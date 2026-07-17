using FBSC.ODMS.Application.Features.ODMS.DashboardTheme.Commands;
using FBSC.ODMS.Application.Features.ODMS.DashboardTheme.Queries;
using FBSC.ODMS.Web.Areas.ODMS.Models;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.DashboardTheme;

[Authorize(Policy = Permission.DashboardTheme.Edit)]
public class EditModel : BasePageModel<EditModel>
{
    [BindProperty]
    public DashboardThemeViewModel DashboardTheme { get; set; } = new();
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
        return await PageFrom(async () => await Mediatr.Send(new GetDashboardThemeByIdQuery(id)), DashboardTheme);
    }

    public async Task<IActionResult> OnPost()
    {		
        if (!ModelState.IsValid)
        {
            return Page();
        }
		
        return await TryThenRedirectToPage(async () => await Mediatr.Send(Mapper.Map<EditDashboardThemeCommand>(DashboardTheme)), "Details", true);
    }	
	public PartialViewResult OnPostChangeFormValue()
    {
        ModelState.Clear();
		
        return Partial("_InputFieldsPartial", DashboardTheme);
    }
	
}
