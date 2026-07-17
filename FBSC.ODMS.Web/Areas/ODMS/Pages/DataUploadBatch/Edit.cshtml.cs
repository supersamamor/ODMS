using FBSC.ODMS.Application.Features.ODMS.DataUploadBatch.Commands;
using FBSC.ODMS.Application.Features.ODMS.DataUploadBatch.Queries;
using FBSC.ODMS.Web.Areas.ODMS.Models;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.DataUploadBatch;

[Authorize(Policy = Permission.DataUploadBatch.Edit)]
public class EditModel : BasePageModel<EditModel>
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
        if (!ModelState.IsValid)
        {
            return Page();
        }
		
        return await TryThenRedirectToPage(async () => await Mediatr.Send(Mapper.Map<EditDataUploadBatchCommand>(DataUploadBatch)), "Details", true);
    }	
	public PartialViewResult OnPostChangeFormValue()
    {
        ModelState.Clear();
		if (AsyncAction == "AddDataUploadColumn")
		{
			return AddDataUploadColumn();
		}
		if (AsyncAction == "RemoveDataUploadColumn")
		{
			return RemoveDataUploadColumn();
		}
		
		
        return Partial("_InputFieldsPartial", DataUploadBatch);
    }
	
	private PartialViewResult AddDataUploadColumn()
	{
		ModelState.Clear();
		if (DataUploadBatch!.DataUploadColumnList == null) { DataUploadBatch!.DataUploadColumnList = []; }
		DataUploadBatch!.DataUploadColumnList!.Add(new DataUploadColumnViewModel() { DataUploadBatchId = DataUploadBatch.Id });
		return Partial("_InputFieldsPartial", DataUploadBatch);
	}
	private PartialViewResult RemoveDataUploadColumn()
	{
		ModelState.Clear();
		DataUploadBatch.DataUploadColumnList = [..DataUploadBatch!.DataUploadColumnList!.Where(l => l.Id != RemoveSubDetailId)];
		return Partial("_InputFieldsPartial", DataUploadBatch);
	}
	
}
