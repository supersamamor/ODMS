using FBSC.ODMS.Application.Features.ODMS.Report.Queries;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Web.Areas.ODMS.Models;
using FBSC.ODMS.Web.Models;
using FBSC.ODMS.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FBSC.ODMS.Web.Helper;


namespace FBSC.ODMS.Web.Areas.ODMS.Pages.ReportSetup;

[Authorize(Policy = Permission.ReportSetup.View)]
public class IndexModel : BasePageModel<IndexModel>
{
    public ReportViewModel Report { get; set; } = new();

    [DataTablesRequest]
    public DataTablesRequest? DataRequest { get; set; }

    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostListAllAsync()
    {		
        var result = await Mediatr.Send(DataRequest!.ToQuery<GetReportQuery>());
        return new JsonResult(result.Data
            .Select(e => new
            {
                e.Id,
                e.ReportName,
				e.QueryType,
                DisplayOnDashboard = DisplayOnDashboardHelper.GetBadge(e.DisplayOnDashboard),
                e.ReportOrChartType,
                e.Sequence,
                e.LastModifiedDate
            })
            .ToDataTablesResponse(DataRequest, result.TotalCount, result.Data.TotalItemCount));
    } 
	
	public async Task<IActionResult> OnGetSelect2Data([FromQuery] Select2Request request)
    {
        var result = await Mediatr.Send(request.ToQuery<GetReportQuery>(nameof(ReportState.ReportName)));
        return new JsonResult(result.ToSelect2Response(e => new Select2Result { Id = e.Id, Text = e.ReportName }));
    }
}
