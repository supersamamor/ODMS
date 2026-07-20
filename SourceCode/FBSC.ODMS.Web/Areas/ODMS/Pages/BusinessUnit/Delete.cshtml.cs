using FBSC.ODMS.Application.Features.ODMS.BusinessUnit.Commands;
using FBSC.ODMS.Application.Features.ODMS.BusinessUnit.Queries;
using FBSC.ODMS.Web.Areas.ODMS.Models;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.BusinessUnit;

[Authorize(Policy = Permission.BusinessUnit.Delete)]
public class DeleteModel : BasePageModel<DeleteModel>
{
    [BindProperty]
    public BusinessUnitViewModel BusinessUnit { get; set; } = new();
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
        return await PageFrom(async () => await Mediatr.Send(new GetBusinessUnitByIdQuery(id)), BusinessUnit);
    }

    public async Task<IActionResult> OnPost()
    {
        return await TryThenRedirectToPage(async () => await Mediatr.Send(new DeleteBusinessUnitCommand { Id = BusinessUnit.Id }), "Index");
    }
}
