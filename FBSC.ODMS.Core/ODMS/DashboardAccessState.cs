using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Core.ODMS;

public record DashboardAccessState : BaseEntity
{
	public string DashboardId { get; init; } = "";
	public string GranteeType { get; init; } = "";
	public string GranteeId { get; init; } = "";
	public string AccessLevel { get; init; } = "";
	public DateTime? GrantedAt { get; init; }
	
	public DashboardState? Dashboard { get; init; }
	
	
}
