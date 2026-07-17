using FBSC.ODMS.Application.Features.ODMS.DashboardQueryResultColumn.Commands;
using FBSC.ODMS.Application.Features.ODMS.DashboardQueryResultColumn.Queries;
using FBSC.ODMS.Web.Areas.ODMS.Models;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.DashboardQueryResultColumn;

[Authorize(Policy = Permission.DashboardQueryResultColumn.Delete)]
public class DeleteModel : BasePageModel<DeleteModel>
{
    [BindProperty]
    public DashboardQueryResultColumnViewModel DashboardQueryResultColumn { get; set; } = new();
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
        return await PageFrom(async () => await Mediatr.Send(new GetDashboardQueryResultColumnByIdQuery(id)), DashboardQueryResultColumn);
    }

    public async Task<IActionResult> OnPost()
    {
        return await TryThenRedirectToPage(async () => await Mediatr.Send(new DeleteDashboardQueryResultColumnCommand { Id = DashboardQueryResultColumn.Id }), "Index");
    }
}
