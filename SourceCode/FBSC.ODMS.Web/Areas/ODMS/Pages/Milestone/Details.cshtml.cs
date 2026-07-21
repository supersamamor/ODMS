using FBSC.ODMS.Application.Features.ODMS.Milestone.Queries;
using FBSC.ODMS.Web.Areas.ODMS.Models;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.Milestone;

[Authorize(Policy = Permission.Milestone.View)]
public class DetailsModel : BasePageModel<DetailsModel>
{
    public MilestoneViewModel Milestone { get; set; } = new();
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
        return await PageFrom(async () => await Mediatr.Send(new GetMilestoneByIdQuery(id)), Milestone);
    }
}
