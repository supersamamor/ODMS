using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Core.ODMS;

public record DashboardState : BaseEntity
{
	public string Code { get; init; } = "";
	public string Name { get; init; } = "";
	public string? Description { get; init; }
	public string? Category { get; init; }
	public string? DashboardThemeId { get; init; }
	public string OwnerUserId { get; init; } = "";
	public bool IsPublic { get; init; }
	public bool IsTemplate { get; init; }
	public int? RefreshIntervalSeconds { get; init; }
	public DateTime? LastPublishedAt { get; init; }
	public bool IsActive { get; init; }
	
	public DashboardThemeState? DashboardTheme { get; init; }
	
	public IList<DashboardWidgetState>? DashboardWidgetList { get; set; }
	public IList<DashboardWidgetState>? DrillDownDashboardWidgetList { get; set; }
	public IList<DashboardAccessState>? DashboardAccessList { get; set; }
	
}
