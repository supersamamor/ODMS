using FBSC.ODMS.Application.Features.ODMS.AiSqlPromptTemplate.Commands;
using FBSC.ODMS.Application.Features.ODMS.AiSqlPromptTemplate.Queries;
using FBSC.ODMS.Web.Areas.ODMS.Models;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.AiSqlPromptTemplate;

[Authorize(Policy = Permission.AiSqlPromptTemplate.Delete)]
public class DeleteModel : BasePageModel<DeleteModel>
{
    [BindProperty]
    public AiSqlPromptTemplateViewModel AiSqlPromptTemplate { get; set; } = new();
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
        return await PageFrom(async () => await Mediatr.Send(new GetAiSqlPromptTemplateByIdQuery(id)), AiSqlPromptTemplate);
    }

    public async Task<IActionResult> OnPost()
    {
        return await TryThenRedirectToPage(async () => await Mediatr.Send(new DeleteAiSqlPromptTemplateCommand { Id = AiSqlPromptTemplate.Id }), "Index");
    }
}
