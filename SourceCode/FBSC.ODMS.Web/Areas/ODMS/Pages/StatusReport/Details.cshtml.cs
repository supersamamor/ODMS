using FBSC.ODMS.Application.Features.ODMS.StatusReport.Commands;
using FBSC.ODMS.Application.Features.ODMS.StatusReport.Queries;
using FBSC.ODMS.Core.Constants;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Web.Areas.ODMS.Models;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.StatusReport;

/// <summary>
/// Read-only View Status Report page (design replica). Reached from MyProjects'
/// per-row View action with ?projectId= (opens the project's latest report) or
/// directly with ?id= for a specific report. Approve / Request Changes are
/// visible only to holders of the Approval permission while the report is
/// Pending Review; both capture remarks via the shared remarks modal.
/// </summary>
[Authorize(Policy = Permission.ProjectStatusReport.View)]
public class DetailsModel : BasePageModel<DetailsModel>
{
    [BindProperty]
    public StatusReportViewModel StatusReport { get; set; } = new();
    [BindProperty]
    public string? ReviewRemarks { get; set; }

    public async Task<IActionResult> OnGet(string? id, string? projectId)
    {
        if (string.IsNullOrEmpty(id) && string.IsNullOrEmpty(projectId))
        {
            return NotFound();
        }
        var reportOption = await Mediatr.Send(new GetStatusReportDetailsQuery(id, projectId));
        var report = reportOption.MatchUnsafe<StatusReportState?>(r => r, () => null);
        if (report == null)
        {
            // MyProjects "View" on a project with no report yet: send the user
            // back with a clear message instead of a bare 404.
            NotyfService.Information(Localizer["No status report has been submitted for this project yet."]);
            return RedirectToPage("/MyProjects/Index");
        }

        Mapper.Map(report, StatusReport);
        StatusReport.ProjectName = report.Project?.ProjectName;
        StatusReport.ProjectCode = report.Project?.ProjectCode;
        StatusReport.BusinessUnitName = report.Project?.BusinessUnit?.Name;
        StatusReport.ProjectManagerName = report.Project?.Employee?.Name;
        StatusReport.BaselineBudget = report.Project?.ApprovedBudget;
        if (report.ReportingWeek != null)
        {
            StatusReport.ReportWeekDisplay = $"{report.ReportingWeek.StartDate:MMM d} - {report.ReportingWeek.EndDate:MMM d, yyyy}";
        }
        // Fixed presentation order for the six health dimensions.
        StatusReport.StatusReportHealthIndicatorList =
            [.. (StatusReport.StatusReportHealthIndicatorList ?? [])
                .OrderBy(h => Array.IndexOf(HealthIndicatorAreas.List, h.Area))];
        return Page();
    }

    public async Task<IActionResult> OnPost(string handler)
    {
        if (!(await HttpContext.RequestServices.GetRequiredService<IAuthorizationService>()
                .AuthorizeAsync(User, Permission.PendingApprovals.Approve)).Succeeded)
        {
            return Forbid();
        }
        var outcome = handler switch
        {
            "Approve" => StatusReportStatuses.Approved,
            "RequestChanges" => StatusReportStatuses.ChangesRequested,
            _ => null,
        };
        if (outcome == null)
        {
            return Page();
        }
        var reviewedById = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return await TryThenRedirectToPage(
            async () => await Mediatr.Send(new ReviewStatusReportCommand(StatusReport.Id, outcome, ReviewRemarks, reviewedById)),
            "Details", true);
    }
}
