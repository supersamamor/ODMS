using FBSC.ODMS.Application.Features.ODMS.Project.Queries;
using FBSC.ODMS.Application.Features.ODMS.StatusReport.Commands;
using FBSC.ODMS.Application.Features.ODMS.StatusReport.Queries;
using FBSC.ODMS.Core.Constants;
using FBSC.ODMS.Web.Areas.ODMS.Models;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.StatusReport;

/// <summary>
/// Create Weekly Status Report - reached from MyProjects via the per-project
/// action button (Project Status Report / Add permission). Accomplishments and
/// Next Steps are dynamic [+]/[-] string lists persisted as JSON columns;
/// health indicators are the six fixed RAG dimensions; milestones and the
/// risks/issues register are dynamic client-side rows.
/// </summary>
[Authorize(Policy = Permission.ProjectStatusReport.Create)]
public class AddModel : BasePageModel<AddModel>
{
    [BindProperty]
    public StatusReportViewModel StatusReport { get; set; } = new();

    public async Task<IActionResult> OnGet(string? projectId, bool importLast = false)
    {
        if (string.IsNullOrEmpty(projectId))
        {
            return NotFound();
        }
        var projectOption = await Mediatr.Send(new GetProjectByIdQuery(projectId));
        var project = projectOption.MatchUnsafe<Core.ODMS.ProjectState?>(p => p, () => null);
        if (project == null)
        {
            return NotFound();
        }

        StatusReport = StatusReport with
        {
            ProjectId = project.Id,
            SubmissionDate = DateTime.Today,
        };
        StatusReport.ProjectName = project.ProjectName;
        StatusReport.BusinessUnitName = project.BusinessUnit?.Name;
        StatusReport.ProjectManagerName = project.Employee?.Name;
        StatusReport.BaselineBudget = project.ApprovedBudget;
        StatusReport.StatusReportHealthIndicatorList =
            [.. HealthIndicatorAreas.List.Select(area => new StatusReportHealthIndicatorViewModel { Area = area })];

        var latest = await Mediatr.Send(new GetLatestStatusReportByProjectQuery(project.Id, IncludeDetails: importLast));
        latest.IfSome(prior =>
        {
            StatusReport.PriorWeekHealth = prior.OverallHealth;
            if (importLast)
            {
                ImportFromPriorReport(prior);
            }
        });
        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        // Prune blank rows the dynamic list components may post.
        StatusReport.Accomplishments = [.. (StatusReport.Accomplishments ?? []).Where(a => !string.IsNullOrWhiteSpace(a))];
        StatusReport.NextSteps = [.. (StatusReport.NextSteps ?? []).Where(n => !string.IsNullOrWhiteSpace(n))];

        if (!ModelState.IsValid)
        {
            return Page();
        }
        var command = Mapper.Map<AddStatusReportCommand>(StatusReport) with
        {
            SelectedYear = ISOWeek.GetYear(DateTime.Today),
        };
        return await TryThenRedirectToPage(async () => await Mediatr.Send(command), "/MyProjects/Index");
    }

    /// <summary>"Import Last Report": prefill this report from the prior one.</summary>
    private void ImportFromPriorReport(Core.ODMS.StatusReportState prior)
    {
        StatusReport.Accomplishments = [.. prior.Accomplishments];
        StatusReport.NextSteps = [.. prior.NextSteps];
        StatusReport = StatusReport with
        {
            OverallHealth = prior.OverallHealth,
            ActualSpend = prior.ActualSpend,
            ScheduleVarianceWeeks = prior.ScheduleVarianceWeeks,
        };
        if (prior.StatusReportHealthIndicatorList?.Count > 0)
        {
            // Keep the fixed six-area ordering; graft prior status/comment by area.
            StatusReport.StatusReportHealthIndicatorList =
                [.. HealthIndicatorAreas.List.Select(area =>
                {
                    var match = prior.StatusReportHealthIndicatorList.FirstOrDefault(h => h.Area == area);
                    return new StatusReportHealthIndicatorViewModel
                    {
                        Area = area,
                        Status = match?.Status ?? "",
                        Comment = match?.Comment,
                    };
                })];
        }
        StatusReport.StatusReportMilestoneList =
            [.. (prior.StatusReportMilestoneList ?? []).Select(m => new StatusReportMilestoneViewModel
            {
                Name = m.Name,
                StartDate = m.StartDate,
                TargetEndDate = m.TargetEndDate,
                Status = m.Status,
            })];
        StatusReport.StatusReportRiskIssueList =
            [.. (prior.StatusReportRiskIssueList ?? []).Select(r => new StatusReportRiskIssueViewModel
            {
                Code = r.Code,
                Type = r.Type,
                Title = r.Title,
                Severity = r.Severity,
                Status = r.Status,
                OwnerId = r.OwnerId,
                DateRaised = r.DateRaised,
                Notes = r.Notes,
            })];
    }
}
