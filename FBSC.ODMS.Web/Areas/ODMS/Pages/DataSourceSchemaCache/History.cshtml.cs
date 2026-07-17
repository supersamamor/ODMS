using FBSC.ODMS.Application.Features.ODMS.DataSourceSchemaCache.Queries;
using FBSC.ODMS.Web.Areas.Admin.Models;
using FBSC.ODMS.Web.Areas.Admin.Queries.AuditTrail;
using FBSC.ODMS.Web.Areas.ODMS.Models;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.DataSourceSchemaCache;

[Authorize(Policy = Permission.DataSourceSchemaCache.History)]
public class HistoryModel : BasePageModel<HistoryModel>
{
    public IList<AuditLogViewModel> AuditLogList { get; set; } = [];
    public DataSourceSchemaCacheViewModel DataSourceSchemaCache { get; set; } = new();
    public async Task<IActionResult> OnGet(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        _ = (await Mediatr.Send(new GetDataSourceSchemaCacheByIdQuery(id))).Select(l=> Mapper.Map(l, DataSourceSchemaCache));  
        AuditLogList = await Mediatr.Send(new GetAuditLogsByPrimaryKeyQuery(id));
        return Page();
    }
}
