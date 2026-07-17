using FBSC.ODMS.Application.Helpers;
using FBSC.ODMS.Web.Areas.Admin.Queries.BatchUploadJobs;
using FBSC.ODMS.Web.Helper;
using FBSC.ODMS.Web.Models;
using FBSC.ODMS.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace FBSC.ODMS.Web.Areas.Admin.Pages.BatchUploadJobs;
[Authorize(Policy = Permission.Admin.View)]
public class IndexModel : BasePageModel<IndexModel>
{
    [DataTablesRequest]
    public DataTablesRequest? DataRequest { get; set; }
    public IActionResult OnGet()
    {
        return Page();
    }
    public async Task<IActionResult> OnPostListAllAsync()
    {
        var result = await Mediatr.Send(DataRequest!.ToQuery<GetBatchUploadJobsQuery>());
        return new JsonResult(result.Data
            .Select(e => new
            {
                e.Id,
                e.Module,
                e.FileType,
                e.StartDateTime,
                FormattedStartDateTime = e.StartDateTime == null ? "" : e.StartDateTime.Value.ApplyTimeOffset().ToString("MM/dd/yyyy hh:mm:ss tt"),
                e.Duration,
                e.Remarks,
                Status = FileUploadStatusHelper.GetBadge(e.Status),
                e.LastModifiedDate,
                e.FormattedModule

            })
            .ToDataTablesResponse(DataRequest, result.TotalCount, result.Data.TotalItemCount));
    }
}
