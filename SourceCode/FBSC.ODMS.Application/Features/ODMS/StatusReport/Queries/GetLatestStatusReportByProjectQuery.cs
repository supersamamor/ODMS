using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FBSC.ODMS.Application.Features.ODMS.StatusReport.Queries;

/// <summary>
/// The most recent status report for a project (by reporting-week start date,
/// then submission date). Used to auto-populate "Prior Week's Health" and to
/// power the "Import Last Report" prefill on the Create Report page.
/// </summary>
public record GetLatestStatusReportByProjectQuery(string ProjectId, bool IncludeDetails = false) : IRequest<Option<StatusReportState>>;

public class GetLatestStatusReportByProjectQueryHandler(ApplicationContext context) : IRequestHandler<GetLatestStatusReportByProjectQuery, Option<StatusReportState>>
{
    public async Task<Option<StatusReportState>> Handle(GetLatestStatusReportByProjectQuery request, CancellationToken cancellationToken = default)
    {
        var query = context.StatusReport.AsNoTracking()
            .Where(s => s.ProjectId == request.ProjectId);
        if (request.IncludeDetails)
        {
            query = query
                .Include(s => s.StatusReportHealthIndicatorList)
                .Include(s => s.StatusReportMilestoneList)
                .Include(s => s.StatusReportRiskIssueList);
        }
        return await query
            .OrderByDescending(s => s.ReportingWeek!.StartDate)
            .ThenByDescending(s => s.SubmissionDate)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
