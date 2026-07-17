using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Application.DTOs;

public record DashboardWidgetListDto : BaseDto
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
	public string GridPositionXFormatted { get { return this.GridPositionX.ToString("#,##0"); } }
	public int GridPositionY { get; init; }
	public string GridPositionYFormatted { get { return this.GridPositionY.ToString("#,##0"); } }
	public int GridWidth { get; init; }
	public string GridWidthFormatted { get { return this.GridWidth.ToString("#,##0"); } }
	public int GridHeight { get; init; }
	public string GridHeightFormatted { get { return this.GridHeight.ToString("#,##0"); } }
	public int? RefreshIntervalOverrideSeconds { get; init; }
	public string RefreshIntervalOverrideSecondsFormatted { get { return this.RefreshIntervalOverrideSeconds == null ? "" : this.RefreshIntervalOverrideSeconds!.Value.ToString("#,##0"); } }
	public int? Sequence { get; init; }
	public string SequenceFormatted { get { return this.Sequence == null ? "" : this.Sequence!.Value.ToString("#,##0"); } }
	
	
}
