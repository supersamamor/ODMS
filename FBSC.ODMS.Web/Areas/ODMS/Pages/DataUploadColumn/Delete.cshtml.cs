using FBSC.ODMS.Application.Features.ODMS.DataUploadColumn.Commands;
using FBSC.ODMS.Application.Features.ODMS.DataUploadColumn.Queries;
using FBSC.ODMS.Web.Areas.ODMS.Models;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.DataUploadColumn;

[Authorize(Policy = Permission.DataUploadColumn.Delete)]
public class DeleteModel : BasePageModel<DeleteModel>
{
    [BindProperty]
    public DataUploadColumnViewModel DataUploadColumn { get; set; } = new();
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
        return await PageFrom(async () => await Mediatr.Send(new GetDataUploadColumnByIdQuery(id)), DataUploadColumn);
    }

    public async Task<IActionResult> OnPost()
    {
        return await TryThenRedirectToPage(async () => await Mediatr.Send(new DeleteDataUploadColumnCommand { Id = DataUploadColumn.Id }), "Index");
    }
}
