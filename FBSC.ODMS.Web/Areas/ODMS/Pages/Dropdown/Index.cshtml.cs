using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using FBSC.ODMS.Application.Features.ODMS.DashboardQuery.Queries;
using FBSC.ODMS.Application.Features.ODMS.DashboardTheme.Queries;
using FBSC.ODMS.Application.Features.ODMS.Dashboard.Queries;
using FBSC.ODMS.Application.Features.ODMS.ReportType.Queries;
using FBSC.ODMS.Application.Features.ODMS.DataSource.Queries;
using FBSC.ODMS.Application.Features.ODMS.DataUploadBatch.Queries;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.Dropdown;


[Authorize]
public class IndexModel : BasePageModel<IndexModel>
{   

	//Repeatable
	public async Task<IActionResult> OnGetSelect2DashboardQueryData([FromQuery] Select2Request request)
	{
		var result = await Mediatr.Send(request.ToQuery<GetDashboardQueryQuery>(nameof(DashboardQueryState.QueryHash)));
		return new JsonResult(result.ToSelect2Response(e => new Select2Result { Id = e.Id, Text = e.Name }));
	}
	public async Task<IActionResult> OnGetSelect2DashboardThemeData([FromQuery] Select2Request request)
	{
		var result = await Mediatr.Send(request.ToQuery<GetDashboardThemeQuery>(nameof(DashboardThemeState.Code)));
		return new JsonResult(result.ToSelect2Response(e => new Select2Result { Id = e.Id, Text = e.Code }));
	}
	public async Task<IActionResult> OnGetSelect2DashboardData([FromQuery] Select2Request request)
	{
		var result = await Mediatr.Send(request.ToQuery<GetDashboardQuery>(nameof(DashboardState.Code)));
		return new JsonResult(result.ToSelect2Response(e => new Select2Result { Id = e.Id, Text = e.Code }));
	}
	public async Task<IActionResult> OnGetSelect2ReportTypeData([FromQuery] Select2Request request)
	{
		var result = await Mediatr.Send(request.ToQuery<GetReportTypeQuery>(nameof(ReportTypeState.Code)));
		return new JsonResult(result.ToSelect2Response(e => new Select2Result { Id = e.Id, Text = e.Code }));
	}
	public async Task<IActionResult> OnGetSelect2DataSourceData([FromQuery] Select2Request request)
	{
		var result = await Mediatr.Send(request.ToQuery<GetDataSourceQuery>(nameof(DataSourceState.Name)));
		return new JsonResult(result.ToSelect2Response(e => new Select2Result { Id = e.Id, Text = e.Name }));
	}
	public async Task<IActionResult> OnGetSelect2DataUploadBatchData([FromQuery] Select2Request request)
	{
		var result = await Mediatr.Send(request.ToQuery<GetDataUploadBatchQuery>(nameof(DataUploadBatchState.Id)));
		return new JsonResult(result.ToSelect2Response(e => new Select2Result { Id = e.Id, Text = e.Id }));
	}
	
}
