using FBSC.ODMS.Application.Features.ODMS.AiSqlGenerationRequest.Commands;
using FBSC.ODMS.Application.Features.ODMS.AiSqlGenerationRequest.Queries;
using FBSC.ODMS.Web.Areas.ODMS.Models;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.AiSqlGenerationRequest;

[Authorize(Policy = Permission.AiSqlGenerationRequest.Delete)]
public class DeleteModel : BasePageModel<DeleteModel>
{
    [BindProperty]
    public AiSqlGenerationRequestViewModel AiSqlGenerationRequest { get; set; } = new();
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
        return await PageFrom(async () => await Mediatr.Send(new GetAiSqlGenerationRequestByIdQuery(id)), AiSqlGenerationRequest);
    }

    public async Task<IActionResult> OnPost()
    {
        return await TryThenRedirectToPage(async () => await Mediatr.Send(new DeleteAiSqlGenerationRequestCommand { Id = AiSqlGenerationRequest.Id }), "Index");
    }
}
