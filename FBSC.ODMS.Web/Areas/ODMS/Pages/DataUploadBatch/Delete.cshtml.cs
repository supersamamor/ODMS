using FBSC.ODMS.Application.Features.ODMS.DataUploadBatch.Commands;
using FBSC.ODMS.Application.Features.ODMS.DataUploadBatch.Queries;
using FBSC.ODMS.Web.Areas.ODMS.Models;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.DataUploadBatch;

[Authorize(Policy = Permission.DataUploadBatch.Delete)]
public class DeleteModel : BasePageModel<DeleteModel>
{
    [BindProperty]
    public DataUploadBatchViewModel DataUploadBatch { get; set; } = new();
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
        return await PageFrom(async () => await Mediatr.Send(new GetDataUploadBatchByIdQuery(id)), DataUploadBatch);
    }

    public async Task<IActionResult> OnPost()
    {
        return await TryThenRedirectToPage(async () => await Mediatr.Send(new DeleteDataUploadBatchCommand { Id = DataUploadBatch.Id }), "Index");
    }
}
