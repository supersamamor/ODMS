using FBSC.Common.Identity.Abstractions;
using FBSC.ODMS.Application.Features.ODMS.Dashboard.Queries;
using FBSC.ODMS.Infrastructure.Services.Dashboard;
using FBSC.ODMS.Web.Areas.ODMS.Models;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.Dashboard;

[Authorize(Policy = Permission.Dashboard.View)]
public class DetailsModel(DashboardAccessAuthorizationService dashboardAccessAuthorizationService, IAuthenticatedUser authenticatedUser) : BasePageModel<DetailsModel>
{
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

        // Blanket Permission.Dashboard.View only proves the user CAN view dashboards in
        // general - DashboardAccessState (DashboardShare) still gates this specific one.
        var roleNames = User.FindAll(Claims.Role).Select(c => c.Value).ToList();
        if (!await dashboardAccessAuthorizationService.CanViewAsync(id, authenticatedUser.UserId, roleNames))
        {
            return Forbid();
        }

        return await PageFrom(async () => await Mediatr.Send(new GetDashboardByIdQuery(id)), Dashboard);
    }
}
