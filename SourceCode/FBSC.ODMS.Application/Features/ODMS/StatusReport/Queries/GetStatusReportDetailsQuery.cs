using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FBSC.ODMS.Application.Features.ODMS.StatusReport.Queries;

/// <summary>
/// A single status report with everything the read-only View page renders:
/// project (+ BU + PM), reporting week, health indicators, milestones, and the
/// risk/issue register with owner names. Pass <paramref name="Id"/> for a
/// specific report, or only <paramref name="ProjectId"/> for the project's
/// latest report (the MyProjects "View" action).
/// </summary>
public record GetStatusReportDetailsQuery(string? Id, string? ProjectId = null) : IRequest<Option<StatusReportState>>;

public class GetStatusReportDetailsQueryHandler(ApplicationContext context) : IRequestHandler<GetStatusReportDetailsQuery, Option<StatusReportState>>
{
    public async Task<Option<StatusReportState>> Handle(GetStatusReportDetailsQuery request, CancellationToken cancellationToken = default)
    {
        var query = context.StatusReport.AsNoTracking()
            .Include(s => s.Project)!.ThenInclude(p => p!.BusinessUnit)
            .Include(s => s.Project)!.ThenInclude(p => p!.Employee)
            .Include(s => s.ReportingWeek)
            .Include(s => s.StatusReportHealthIndicatorList)
            .Include(s => s.StatusReportMilestoneList)
            .Include(s => s.StatusReportRiskIssueList)!.ThenInclude(r => r.Owner)
            .AsSplitQuery();

        if (!string.IsNullOrEmpty(request.Id))
        {
            return await query.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        }
        if (!string.IsNullOrEmpty(request.ProjectId))
        {
            return await query
                .Where(s => s.ProjectId == request.ProjectId)
                .OrderByDescending(s => s.ReportingWeek!.StartDate)
                .ThenByDescending(s => s.SubmissionDate)
                .FirstOrDefaultAsync(cancellationToken);
        }
        return Option<StatusReportState>.None;
    }
}
