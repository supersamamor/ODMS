using FBSC.ODMS.Application.Features.ODMS.AiSqlPromptTemplate.Commands;
using FBSC.ODMS.Application.Features.ODMS.AiSqlPromptTemplate.Queries;
using FBSC.ODMS.Web.Areas.ODMS.Models;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.AiSqlPromptTemplate;

[Authorize(Policy = Permission.AiSqlPromptTemplate.Edit)]
public class EditModel : BasePageModel<EditModel>
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
        if (!ModelState.IsValid)
        {
            return Page();
        }
		
        return await TryThenRedirectToPage(async () => await Mediatr.Send(Mapper.Map<EditAiSqlPromptTemplateCommand>(AiSqlPromptTemplate)), "Details", true);
    }	
	public PartialViewResult OnPostChangeFormValue()
    {
        ModelState.Clear();
		
        return Partial("_InputFieldsPartial", AiSqlPromptTemplate);
    }
	
}
