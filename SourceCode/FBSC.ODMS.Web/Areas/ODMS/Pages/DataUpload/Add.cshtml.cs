using FBSC.ODMS.Application.Features.ODMS.DataUpload.Commands;
using FBSC.ODMS.Web.Areas.ODMS.Models;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.DataUpload;

[Authorize(Policy = Permission.DataUpload.Create)]
public class AddModel : BasePageModel<AddModel>
{
    [BindProperty]
    public DataUploadViewModel DataUpload { get; set; } = new();
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
		if (DataUpload.FilePathForm != null && await UploadFile<DataUploadViewModel>(WebConstants.DataUpload, nameof(DataUpload.FilePath), DataUpload.Id, DataUpload.FilePathForm) == "") { return Page(); }
		
        return await TryThenRedirectToPage(async () => await Mediatr.Send(Mapper.Map<AddDataUploadCommand>(DataUpload)), "Details", true);
    }	
	public PartialViewResult OnPostChangeFormValue()
    {
        ModelState.Clear();
		
        return Partial("_InputFieldsPartial", DataUpload);
    }
	
}
