using FBSC.ODMS.Application.Features.ODMS.Approval.Commands;
using FBSC.ODMS.Application.Features.ODMS.Approval.Queries;
using FBSC.ODMS.Application.Features.ODMS.ReportType.Queries;
using FBSC.ODMS.Web.Areas.ODMS.Models;
using FBSC.ODMS.Web.Models;
using FBSC.ODMS.Core.ODMS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.ReportType;

[Authorize(Policy = Permission.ReportType.Approve)]
public class ApproveModel : BasePageModel<ApproveModel>
{
    [BindProperty]
    public ReportTypeViewModel ReportType { get; set; } = new();
    [BindProperty]
    public string? ApprovalStatus { get; set; }
	[BindProperty]
	public string? ApprovalRemarks { get; set; }
    public async Task<IActionResult> OnGet(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        _ = (await Mediatr.Send(new GetApprovalStatusPerApproverByIdQuery(id, ApprovalModule.ReportType))).Select(l => ApprovalStatus = l);
        return await PageFrom(async () => await Mediatr.Send(new GetReportTypeByIdQuery(id)), ReportType);
    }

    public async Task<IActionResult> OnPost(string handler)
    {
        if (handler == "Approve")
        {
            return await Approve();
        }
        else if (handler == "Reject")
        {
            return await Reject();
        }
        return Page();
    }
    private async Task<IActionResult> Approve()
    {
        return await TryThenRedirectToPage(async () => await Mediatr.Send(new ApproveCommand(ReportType.Id, ApprovalRemarks, ApprovalModule.ReportType)), "Approve", true);
    }
    private async Task<IActionResult> Reject()
    {
        return await TryThenRedirectToPage(async () => await Mediatr.Send(new RejectCommand(ReportType.Id, ApprovalRemarks, ApprovalModule.ReportType)), "Approve", true);
    }
}
