using FBSC.Common.Core.Base.Models;

namespace FBSC.ODMS.Core.ODMS;

public record EmployeeState : BaseEntity
{
	public string? Department { get; init; }
	public string Email { get; init; } = "";
	public string EmployeeCode { get; init; } = "";
	public string Name { get; init; } = "";
	public string Position { get; init; } = "";
	/// <summary>Employee rank, mapped to the Ranks (a.k.a. MemberLevels) constant.</summary>
	public string? Rank { get; init; }
	public string? UserId { get; init; }


	public IList<ProjectState>? ProjectList { get; set; }

}
