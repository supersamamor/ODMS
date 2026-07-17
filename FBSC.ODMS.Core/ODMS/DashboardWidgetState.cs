using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Core.ODMS;

public record DashboardWidgetState : BaseEntity
{
	public string DashboardId { get; init; } = "";
	public string DashboardQueryId { get; init; } = "";
	public string ReportTypeId { get; init; } = "";
	public string Title { get; init; } = "";
	public string? XAxisColumnName { get; init; }
	public string? YAxisColumnsJson { get; init; }
	public string? SeriesColumnName { get; init; }
	public string? AggregationOverride { get; init; }
	public string? DrillDownDashboardId { get; init; }
	public int GridPositionX { get; init; }
	public int GridPositionY { get; init; }
	public int GridWidth { get; init; }
	public int GridHeight { get; init; }
	public int? RefreshIntervalOverrideSeconds { get; init; }
	public int? Sequence { get; init; }
	
	public DashboardState? Dashboard { get; init; }
	public DashboardQueryState? DashboardQuery { get; init; }
	public ReportTypeState? ReportType { get; init; }
	public DashboardState? DrillDownDashboard { get; init; }
	
	
}
