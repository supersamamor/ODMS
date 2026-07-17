using FBSC.ODMS.Application.Features.ODMS.DashboardAccess.Commands;
using FBSC.ODMS.Application.Features.ODMS.DashboardAccess.Queries;
using FBSC.ODMS.Web.Areas.ODMS.Models;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.DashboardAccess;

[Authorize(Policy = Permission.DashboardAccess.Edit)]
public class EditModel : BasePageModel<EditModel>
{
    [BindProperty]
    public DashboardAccessViewModel DashboardAccess { get; set; } = new();
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
        return await PageFrom(async () => await Mediatr.Send(new GetDashboardAccessByIdQuery(id)), DashboardAccess);
    }

    public async Task<IActionResult> OnPost()
    {		
        if (!ModelState.IsValid)
        {
            return Page();
        }
		
        return await TryThenRedirectToPage(async () => await Mediatr.Send(Mapper.Map<EditDashboardAccessCommand>(DashboardAccess)), "Details", true);
    }	
	public PartialViewResult OnPostChangeFormValue()
    {
        ModelState.Clear();
		
        return Partial("_InputFieldsPartial", DashboardAccess);
    }
	
}
