using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Core.ODMS;

public record ProjectState : BaseEntity
{
	public string ProjectName { get; init; } = "";
    public string BusinessUnitId { get; init; } = "";
    public string Priority { get; init; } = "";
	public DateTime StartDate { get; init; }
	public DateTime TargetEndDate { get; init; }
	public decimal? EstimatedBudget { get; init; }
	public string? ProjectDescription { get; init; }
	public string ProjectManagerId { get; init; } = "";	
	public string? HealthStatus { get; init; }
	public string? Phase { get; init; }
	public string? ScheduleStatus { get; init; }
	public DateTime? LastReviewDate { get; init; }
	public DateTime? LastUpdatedDate { get; init; }
	
	public BusinessUnitState? BusinessUnit { get; init; }
	public EmployeeState? Employee { get; init; }
	
	public IList<TeamMembersState>? TeamMembersList { get; set; }
	public IList<ProjectHistoryState>? ProjectHistoryList { get; set; }
	
}
