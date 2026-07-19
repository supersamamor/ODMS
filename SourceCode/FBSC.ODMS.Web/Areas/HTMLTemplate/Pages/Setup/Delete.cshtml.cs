using FBSC.HTMLTemplate;
using FBSC.HTMLTemplate.Features.HTMLTemplate.Commands;
using FBSC.HTMLTemplate.Features.HTMLTemplate.Queries;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Areas.HTMLTemplate.Pages.Setup;

[Authorize(Policy = HTMLTemplatePermission.HTMLTemplate.Delete)]
public class DeleteModel : BasePageModel<DeleteModel>
{
    [BindProperty]
    public FBSC.HTMLTemplate.ViewModels.HTMLTemplateViewModel HTMLTemplateForm { get; set; } = new();
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
        return await PageFrom(async () => await Mediatr.Send(new GetHTMLTemplateByIdQuery(id)), HTMLTemplateForm);
    }

    public async Task<IActionResult> OnPost()
    {
        return await TryThenRedirectToPage(async () => await Mediatr.Send(new DeleteHTMLTemplateCommand { Id = HTMLTemplateForm.Id }), "Index");
    }
}
