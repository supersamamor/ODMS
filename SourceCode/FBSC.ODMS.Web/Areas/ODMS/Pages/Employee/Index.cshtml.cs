using FBSC.ODMS.Application.Features.ODMS.Employee.Queries;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Web.Models;
using FBSC.ODMS.ExcelProcessor.Services;
using FBSC.ODMS.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.Employee;

[Authorize(Policy = Permission.Employee.View)]
public class IndexModel(IConfiguration configuration) : BasePageModel<IndexModel>
{   
	private readonly string? _uploadPath = configuration.GetValue<string>("UsersUpload:SecureUploadFilePath");
    [DataTablesRequest]
    public DataTablesRequest? DataRequest { get; set; }
	[BindProperty]
    public BatchUploadModel BatchUpload { get; set; } = new();
    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostListAllAsync()
    {
		var result = await Mediatr.Send(DataRequest!.ToQuery<GetEmployeeQuery>());
		return new JsonResult(result.Data.ToDataTablesResponse(DataRequest, result.TotalCount, result.Data.TotalItemCount));	
    } 

	public async Task<IActionResult> OnPostBatchUploadAsync()
    {        
        return await BatchUploadAsync<IndexModel, EmployeeState>(BatchUpload.BatchUploadForm, nameof(EmployeeState), "Index");
    }

    public IActionResult OnPostDownloadTemplate()
    {
        ModelState.Clear();
		BatchUpload.BatchUploadFileName = ExcelService.ExportTemplate<EmployeeState>(_uploadPath + "\\" + WebConstants.ExcelTemplateSubFolder);
        NotyfService.Success(Localizer["Successfully downloaded upload template."]);
        return Page();
    }
}
