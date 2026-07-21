using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Core.ODMS;

public record StatusReportState : BaseEntity
{
	public string ProjectId { get; init; } = "";
	public string ReportingWeekId { get; init; } = "";
	public DateTime? SubmissionDate { get; init; }
	public string? OverallHealth { get; init; }
	public string Status { get; init; } = "";
	public decimal? ActualSpend { get; init; }
	public int? ScheduleVarianceWeeks { get; init; }
	public string? ScheduleStatus { get; init; }
	public string? BudgetStatus { get; init; }
	public string? Phase { get; init; }
	public string? ReviewedById { get; init; }
	public DateTime? ReviewedDate { get; init; }
	public string? ReviewComments { get; init; }
	
	public ProjectState? Project { get; init; }
	public ReportingWeekState? ReportingWeek { get; init; }
	
	public IList<StatusReportHealthIndicatorState>? StatusReportHealthIndicatorList { get; set; }
	public IList<StatusReportMilestoneState>? StatusReportMilestoneList { get; set; }
	public IList<StatusReportRiskIssueState>? StatusReportRiskIssueList { get; set; }
	public IList<AccomplishmentState>? AccomplishmentList { get; set; }
	public IList<NextStepState>? NextStepList { get; set; }
	
}
