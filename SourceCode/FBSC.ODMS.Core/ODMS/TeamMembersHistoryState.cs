namespace FBSC.ODMS.Core.ODMS;

public record TeamMembersHistoryState : TeamMembersBase
{
	public string ProjectHistoryId { get; init; } = "";

	public ProjectHistoryState? ProjectHistory { get; init; }
	public EmployeeState? Employee { get; init; }

}
