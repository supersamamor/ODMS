namespace FBSC.ODMS.Core.ODMS;

public record ProjectHistoryState : ProjectBase
{
	public string ProjectId { get; init; } = "";

	public ProjectState? Project { get; init; }
	public BusinessUnitState? BusinessUnit { get; init; }
	public EmployeeState? Employee { get; init; }
	public EmployeeState? DeputyProjectManager { get; init; }
	public EmployeeState? TechnologyBusinessPartner { get; init; }

	public IList<TeamMembersHistoryState>? TeamMembersHistoryList { get; set; }

}
