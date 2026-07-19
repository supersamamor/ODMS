using FBSC.HTMLTemplate;
using FBSC.HTMLTemplate.Features.HTMLTemplate.Commands;
using FBSC.HTMLTemplate.Models;
using FBSC.HTMLTemplate.ViewModels;
using FBSC.ODMS.Web.Models;
using FBSC.ODMS.Web.Service;
using LanguageExt.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static LanguageExt.Prelude;

namespace FBSC.ODMS.Web.Areas.HTMLTemplate.Pages.Setup;
[Authorize(Policy = HTMLTemplatePermission.HTMLTemplate.Create)]
public class AddModel(AIReportQueryGenerationServices aiReportQueryGenerationServices) : BasePageModel<AddModel>
{
    [BindProperty]
    public HTMLTemplateViewModel HTMLTemplateForm { get; set; } = new();
    [BindProperty]
    public string? RemoveSubDetailId { get; set; }
    [BindProperty]
    public string? AsyncAction { get; set; }
    public IActionResult OnGet()
    {
		
        return Page();
    }
    public async Task<IActionResult> OnPostAsync(string handler)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }
        if (handler == "Save")
        {
            return await TryThenRedirectToPage(async () => await Mediatr.Send(Mapper.Map<AddHTMLTemplateCommand>(HTMLTemplateForm)), "Details", true);
        }
        else if (handler == "GenerateQueryFromAI")
        {
            try
            {
                if (string.IsNullOrEmpty(HTMLTemplateForm.Description))
                {
                    NotyfService.Warning(Localizer["Report description is required if you want to generate query via AI."]);
                    return Page();
                }           
                var htmlTemplate = await aiReportQueryGenerationServices.GenerateHTMLTemplateViaObject(HTMLTemplateForm, token: new CancellationToken());
                var command = Mapper.Map<AddHTMLTemplateCommand>(HTMLTemplateForm);
                command.SetHTMLTemplate(htmlTemplate ?? "");               
                var result = await TryAsync(async () => await Mediatr.Send(command))
               .IfFail(ex =>
               {
                   Logger.LogError(ex, "Exception in OnPostAsync");
                   return Fail<Error, HTMLTemplateState>(Localizer[$"Something went wrong. Please contact the system administrator."] + $" TraceId = {HttpContext.TraceIdentifier}");
               });
                return result.Match<IActionResult>(
                    Succ: entity =>
                    {
                        NotyfService.Success(Localizer["Report query generated successfully."]);
                        return RedirectToPage("Edit", new { id = entity.Id });
                    },
                    Fail: errors =>
                    {
                        AddModelError(errors, notifyError: true);
                        return Page();
                    });
            }
            catch (Exception ex)
            {
                NotyfService.Error(Localizer["An error occurred while generating query from AI."]);
                Logger.LogError(ex, "An error occurred while generating query from AI.");
                return Page();
            }
        }
        return Page();
    }

	public PartialViewResult OnPostChangeFormValue()
    {
        ModelState.Clear();
		
        return Partial("_InputFieldsPartial", HTMLTemplateForm);
    }
	
}
