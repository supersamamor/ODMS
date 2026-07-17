using FBSC.ODMS.Application.Features.ODMS.DataSourceSchemaCache.Commands;
using FBSC.ODMS.Application.Features.ODMS.DataSourceSchemaCache.Queries;
using FBSC.ODMS.Web.Areas.ODMS.Models;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.DataSourceSchemaCache;

[Authorize(Policy = Permission.DataSourceSchemaCache.Edit)]
public class EditModel : BasePageModel<EditModel>
{
    [BindProperty]
    public DataSourceSchemaCacheViewModel DataSourceSchemaCache { get; set; } = new();
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
        return await PageFrom(async () => await Mediatr.Send(new GetDataSourceSchemaCacheByIdQuery(id)), DataSourceSchemaCache);
    }

    public async Task<IActionResult> OnPost()
    {		
        if (!ModelState.IsValid)
        {
            return Page();
        }
		
        return await TryThenRedirectToPage(async () => await Mediatr.Send(Mapper.Map<EditDataSourceSchemaCacheCommand>(DataSourceSchemaCache)), "Details", true);
    }	
	public PartialViewResult OnPostChangeFormValue()
    {
        ModelState.Clear();
		
        return Partial("_InputFieldsPartial", DataSourceSchemaCache);
    }
	
}
