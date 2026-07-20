using FBSC.ODMS.Application.Features.ODMS.Project.Commands;
using FBSC.ODMS.Application.Features.ODMS.Project.Queries;
using FBSC.ODMS.Web.Areas.ODMS.Models;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.Project;

[Authorize(Policy = Permission.Project.Delete)]
public class DeleteModel : BasePageModel<DeleteModel>
{
    [BindProperty]
    public ProjectViewModel Project { get; set; } = new();
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
        return await PageFrom(async () => await Mediatr.Send(new GetProjectByIdQuery(id)), Project);
    }

    public async Task<IActionResult> OnPost()
    {
        return await TryThenRedirectToPage(async () => await Mediatr.Send(new DeleteProjectCommand { Id = Project.Id }), "Index");
    }
}
