using FBSC.ODMS.Application.Features.ODMS.Project.Queries;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Web.Models;
using FBSC.ODMS.ExcelProcessor.Services;
using FBSC.ODMS.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.Project;

[Authorize(Policy = Permission.Project.View)]
public class IndexModel(IConfiguration configuration) : BasePageModel<IndexModel>
{   
	private readonly string? _uploadPath = configuration.GetValue<string>("UsersUpload:SecureUploadFilePath");
    [DataTablesRequest]
    public DataTablesRequest? DataRequest { get; set; }
	[BindProperty]
    public BatchUploadModel BatchUpload { get; set; } = new();
    // Delivery Tower/Category pill filter, posted alongside the DataTables payload.
    [BindProperty]
    public string? DeliveryCategory { get; set; }
    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostListAllAsync()
    {
		var query = DataRequest!.ToQuery<GetProjectQuery>();
		query.DeliveryCategory = DeliveryCategory;
		var result = await Mediatr.Send(query);
		return new JsonResult(result.Data.ToDataTablesResponse(DataRequest, result.TotalCount, result.Data.TotalItemCount));
    }

	public async Task<IActionResult> OnPostBatchUploadAsync()
    {        
        return await BatchUploadAsync<IndexModel, ProjectState>(BatchUpload.BatchUploadForm, nameof(ProjectState), "Index");
    }

    public IActionResult OnPostDownloadTemplate()
    {
        ModelState.Clear();
		BatchUpload.BatchUploadFileName = ExcelService.ExportTemplate<ProjectState>(_uploadPath + "\\" + WebConstants.ExcelTemplateSubFolder);
        NotyfService.Success(Localizer["Successfully downloaded upload template."]);
        return Page();
    }
}
