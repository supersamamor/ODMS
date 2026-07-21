using FBSC.ODMS.Application.Features.ODMS.Project.Queries;
using FBSC.ODMS.Web.Attributes;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.MyProjects;

/// <summary>
/// "My Projects" - the status-reporting entry point. Same listing/data fetch as
/// Project/Index but gated by the Project Status Report permission (NOT the base
/// Project permission), so report authors don't need registry access.
/// </summary>
[Authorize(Policy = Permission.ProjectStatusReport.View)]
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
        var result = await Mediatr.Send(DataRequest!.ToQuery<GetProjectQuery>());
        return new JsonResult(result.Data.ToDataTablesResponse(DataRequest, result.TotalCount, result.Data.TotalItemCount));
    }
}
