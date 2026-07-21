using FBSC.ODMS.Application.Features.ODMS.BusinessUnit.Queries;
using FBSC.ODMS.Application.Features.ODMS.Employee.Queries;
using FBSC.ODMS.Application.Features.ODMS.Project.Queries;
using FBSC.ODMS.Application.Features.ODMS.ProjectHistory.Queries;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace FBSC.ODMS.Web.Areas.ODMS.Pages.Dropdown;


[Authorize]
public class IndexModel : BasePageModel<IndexModel>
{

    public async Task<IActionResult> OnGetSelect2ProjectData([FromQuery] Select2Request request)
    {
        var result = await Mediatr.Send(request.ToQuery<GetProjectQuery>(nameof(ProjectState.Id)));
        return new JsonResult(result.ToSelect2Response(e => new Select2Result { Id = e.Id, Text = e.Id }));
    }
    public async Task<IActionResult> OnGetSelect2ProjectHistoryData([FromQuery] Select2Request request)
    {
        var result = await Mediatr.Send(request.ToQuery<GetProjectHistoryQuery>(nameof(ProjectHistoryState.Id)));
        return new JsonResult(result.ToSelect2Response(e => new Select2Result { Id = e.Id, Text = e.Id }));
    }
    public async Task<IActionResult> OnGetSelect2EmployeeData([FromQuery] Select2Request request)
    {
        var result = await Mediatr.Send(request.ToQuery<GetEmployeeQuery>(nameof(EmployeeState.Name)));
        return new JsonResult(result.ToSelect2Response(e => new Select2Result { Id = e.Id, Text = e.Name }));
    }
    public async Task<IActionResult> OnGetSelect2BusinessUnitData([FromQuery] Select2Request request)
    {
        var result = await Mediatr.Send(request.ToQuery<GetBusinessUnitQuery>(nameof(BusinessUnitState.Name)));
        return new JsonResult(result.ToSelect2Response(e => new Select2Result { Id = e.Id, Text = e.Name }));
    }


}
