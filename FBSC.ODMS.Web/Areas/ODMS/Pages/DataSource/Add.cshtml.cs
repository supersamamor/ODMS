using FBSC.ODMS.Application.Features.ODMS.DataSource.Commands;
using FBSC.ODMS.Web.Areas.ODMS.Models;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.DataSource;

[Authorize(Policy = Permission.DataSource.Create)]
public class AddModel : BasePageModel<AddModel>
{
    [BindProperty]
    public DataSourceViewModel DataSource { get; set; } = new();
    [BindProperty]
    public string? RemoveSubDetailId { get; set; }
    [BindProperty]
    public string? AsyncAction { get; set; }
    public IActionResult OnGet()
    {
		
        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
		
        if (!ModelState.IsValid)
        {
            return Page();
        }
		
        return await TryThenRedirectToPage(async () => await Mediatr.Send(Mapper.Map<AddDataSourceCommand>(DataSource)), "Details", true);
    }	
	public PartialViewResult OnPostChangeFormValue()
    {
        ModelState.Clear();
		if (AsyncAction == "AddDataSourceSchemaCache")
		{
			return AddDataSourceSchemaCache();
		}
		if (AsyncAction == "RemoveDataSourceSchemaCache")
		{
			return RemoveDataSourceSchemaCache();
		}
		
		
        return Partial("_InputFieldsPartial", DataSource);
    }
	
	private PartialViewResult AddDataSourceSchemaCache()
	{
		ModelState.Clear();
		if (DataSource!.DataSourceSchemaCacheList == null) { DataSource!.DataSourceSchemaCacheList = []; }
		DataSource!.DataSourceSchemaCacheList!.Add(new DataSourceSchemaCacheViewModel() { DataSourceId = DataSource.Id });
		return Partial("_InputFieldsPartial", DataSource);
	}
	private PartialViewResult RemoveDataSourceSchemaCache()
	{
		ModelState.Clear();
		DataSource.DataSourceSchemaCacheList = [..DataSource!.DataSourceSchemaCacheList!.Where(l => l.Id != RemoveSubDetailId)];
		return Partial("_InputFieldsPartial", DataSource);
	}
	
}
