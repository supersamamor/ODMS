using FBSC.Common.Identity.Abstractions;
using FBSC.ODMS.Application.Features.ODMS.Dashboard.Commands;
using FBSC.ODMS.Application.Features.ODMS.Dashboard.Queries;
using FBSC.ODMS.Infrastructure.Services.Dashboard;
using FBSC.ODMS.Web.Areas.ODMS.Models;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.Dashboard;

[Authorize(Policy = Permission.Dashboard.Delete)]
public class DeleteModel(DashboardAccessAuthorizationService dashboardAccessAuthorizationService, IAuthenticatedUser authenticatedUser) : BasePageModel<DeleteModel>
{
    [BindProperty]
    public DashboardViewModel Dashboard { get; set; } = new();
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
        if (!await IsOwnerAsync(id))
        {
            return Forbid();
        }
        return await PageFrom(async () => await Mediatr.Send(new GetDashboardByIdQuery(id)), Dashboard);
    }

    public async Task<IActionResult> OnPost()
    {
        if (!await IsOwnerAsync(Dashboard.Id))
        {
            return Forbid();
        }
        return await TryThenRedirectToPage(async () => await Mediatr.Send(new DeleteDashboardCommand { Id = Dashboard.Id }), "Index");
    }

    // Deletion is deliberately stricter than the Edit share level - only the dashboard owner
    // (DashboardState.OwnerUserId or a DashboardAccessState grant of "Owner") can delete it.
    private async Task<bool> IsOwnerAsync(string dashboardId)
    {
        var roleNames = User.FindAll(Claims.Role).Select(c => c.Value).ToList();
        return await dashboardAccessAuthorizationService.IsOwnerAsync(dashboardId, authenticatedUser.UserId, roleNames);
    }
}
