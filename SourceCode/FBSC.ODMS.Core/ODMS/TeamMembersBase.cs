using FBSC.Common.Core.Base.Models;

namespace FBSC.ODMS.Core.ODMS;

/// <summary>
/// Columns shared by <see cref="TeamMembersState"/> and
/// <see cref="TeamMembersHistoryState"/>, per the final ODMS DatabaseStructure
/// workbook. Members are Employee references (no more free-text names).
/// </summary>
public abstract record TeamMembersBase : BaseEntity
{
	public string EmployeeId { get; init; } = "";
	public string Role { get; init; } = "";
	public string? MemberLevel { get; init; }
}
