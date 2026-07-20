using FBSC.ODMS.Application.Features.ODMS.DataSource.Commands;
using FBSC.ODMS.Application.Features.ODMS.UploadProcessor.Commands;
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
		if (DataSource.DataSourceType == Core.Constants.DataSourceTypes.FileUpload && DataSource.UploadedFileForm != null)
		{
			var uploadedFilePath = await UploadFile<DataSourceViewModel>(WebConstants.DataSource, nameof(DataSource.UploadedFilePath), DataSource.Id, DataSource.UploadedFileForm);
			if (uploadedFilePath == "") { return Page(); }
			DataSource = DataSource with { UploadedFilePath = uploadedFilePath };
		}

        var result = await TryThenRedirectToPage(async () => await Mediatr.Send(Mapper.Map<AddDataSourceCommand>(DataSource)), "Details", true);

        if (result is RedirectToPageResult && DataSource.DataSourceType == Core.Constants.DataSourceTypes.FileUpload && !string.IsNullOrEmpty(DataSource.UploadedFilePath))
        {
            await Mediatr.Send(new UploadProcessorCommand
            {
                FilePath = DataSource.UploadedFilePath,
                FileType = Core.Constants.FileType.Excel,
                Module = Core.Constants.UploadModules.DataSourceFileImport,
                UploadType = Core.Constants.UploadProcessingType.PerFile,
                TargetEntityId = DataSource.Id
            });
        }

        return result;
    }
	public PartialViewResult OnPostChangeFormValue()
    {
        ModelState.Clear();
		
        return Partial("_InputFieldsPartial", DataSource);
    }
	
}
