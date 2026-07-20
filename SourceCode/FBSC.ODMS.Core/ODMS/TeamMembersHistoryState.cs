using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Core.ODMS;

public record TeamMembersHistoryState : BaseEntity
{
	public string ProjectHistoryId { get; init; } = "";
	public string MemberName { get; init; } = "";
	public string Role { get; init; } = "";
	
	public ProjectHistoryState? ProjectHistory { get; init; }
	
	
}
