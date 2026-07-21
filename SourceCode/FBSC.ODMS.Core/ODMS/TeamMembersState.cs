namespace FBSC.ODMS.Core.ODMS;

public record TeamMembersState : TeamMembersBase
{
	public string ProjectId { get; init; } = "";

	public ProjectState? Project { get; init; }
	public EmployeeState? Employee { get; init; }

}
