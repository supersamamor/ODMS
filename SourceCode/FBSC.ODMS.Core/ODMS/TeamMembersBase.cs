using FBSC.Common.Core.Base.Models;

namespace FBSC.ODMS.Core.ODMS;

/// <summary>
/// Columns for a project team member. Members are Employee references (no more
/// free-text names). Kept as a base record for shared configuration.
/// </summary>
public abstract record TeamMembersBase : BaseEntity
{
	// Nullable: a team member may be recorded before the person is known
	// ("(Unknown)" in the UI), which persists as NULL.
	public string? EmployeeId { get; init; }
	public string Role { get; init; } = "";
	public string? MemberLevel { get; init; }
}
