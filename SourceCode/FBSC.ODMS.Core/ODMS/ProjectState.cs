namespace FBSC.ODMS.Core.ODMS;

public record ProjectState : ProjectBase
{
	public BusinessUnitState? BusinessUnit { get; init; }
	public EmployeeState? Employee { get; init; }
	public EmployeeState? DeputyProjectManager { get; init; }
	public EmployeeState? TechnologyBusinessPartner { get; init; }

	public IList<TeamMembersState>? TeamMembersList { get; set; }
	public IList<ProjectHistoryState>? ProjectHistoryList { get; set; }
    public IList<StatusReportState>? StatusReportList { get; set; }
    public IList<ProjectMilestoneState>? ProjectMilestoneList { get; set; }
    public IList<RiskIssueState>? RiskIssueList { get; set; }
}
