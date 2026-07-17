using FBSC.ODMS.Application.Features.ODMS.DashboardQueryParameter.Commands;
using FBSC.ODMS.Application.Features.ODMS.DashboardQueryParameter.Queries;
using FBSC.ODMS.Web.Areas.ODMS.Models;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.DashboardQueryParameter;

[Authorize(Policy = Permission.DashboardQueryParameter.Delete)]
public class DeleteModel : BasePageModel<DeleteModel>
{
    [BindProperty]
    public DashboardQueryParameterViewModel DashboardQueryParameter { get; set; } = new();
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
        return await PageFrom(async () => await Mediatr.Send(new GetDashboardQueryParameterByIdQuery(id)), DashboardQueryParameter);
    }

    public async Task<IActionResult> OnPost()
    {
        return await TryThenRedirectToPage(async () => await Mediatr.Send(new DeleteDashboardQueryParameterCommand { Id = DashboardQueryParameter.Id }), "Index");
    }
}
