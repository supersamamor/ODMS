using FBSC.ODMS.Application.Features.ODMS.DataUploadBatch.Queries;
using FBSC.ODMS.Web.Areas.Admin.Models;
using FBSC.ODMS.Web.Areas.Admin.Queries.AuditTrail;
using FBSC.ODMS.Web.Areas.ODMS.Models;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.DataUploadBatch;

[Authorize(Policy = Permission.DataUploadBatch.History)]
public class HistoryModel : BasePageModel<HistoryModel>
{
    public IList<AuditLogViewModel> AuditLogList { get; set; } = [];
    public DataUploadBatchViewModel DataUploadBatch { get; set; } = new();
    public async Task<IActionResult> OnGet(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        _ = (await Mediatr.Send(new GetDataUploadBatchByIdQuery(id))).Select(l=> Mapper.Map(l, DataUploadBatch));  
        AuditLogList = await Mediatr.Send(new GetAuditLogsByPrimaryKeyQuery(id));
        return Page();
    }
}
