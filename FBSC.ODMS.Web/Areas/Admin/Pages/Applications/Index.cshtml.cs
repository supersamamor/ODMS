using FBSC.ODMS.Web.Areas.Admin.Models;
using FBSC.ODMS.Web.Areas.Admin.Queries.Applications;
using FBSC.ODMS.Web.Models;
using FBSC.ODMS.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Areas.Admin.Pages.Applications;

[Authorize(Policy = Permission.Applications.View)]
public class IndexModel : BasePageModel<IndexModel>
{
    [DataTablesRequest]
    public DataTablesRequest? DataRequest { get; set; }

    public ApplicationViewModel Application { get; set; } = new();

    public async Task<IActionResult> OnPostListAllAsync()
    {
        var result = await Mediatr.Send(DataRequest!.ToQuery<GetApplicationsQuery>());
        return new JsonResult(result.Data
            .Select(e => new
            {
                e.ClientId,
                e.DisplayName,
            })
            .ToDataTablesResponse(DataRequest, result.TotalCount, result.Data.TotalItemCount));
    }
}
