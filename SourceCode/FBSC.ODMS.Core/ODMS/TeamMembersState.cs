using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Core.ODMS;

public record TeamMembersState : BaseEntity
{
	public string ProjectId { get; init; } = "";
	public string MemberName { get; init; } = "";
	public string Role { get; init; } = "";
	
	public ProjectState? Project { get; init; }
	
	
}
