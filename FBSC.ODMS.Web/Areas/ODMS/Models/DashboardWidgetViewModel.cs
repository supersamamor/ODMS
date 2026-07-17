using FBSC.Common.Web.Utility.Extensions;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using FBSC.Common.Web.Utility.Annotations;

namespace FBSC.ODMS.Web.Areas.ODMS.Models;

public record DashboardWidgetViewModel : BaseViewModel
{	
	[Display(Name = "Dashboard")]
	[Required]
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string DashboardId { get; init; } = "";
	public string?  ReferenceFieldDashboardId { get; set; }
	[Display(Name = "Dashboard Query")]
	[Required]
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string DashboardQueryId { get; init; } = "";
	public string?  ReferenceFieldDashboardQueryId { get; set; }
	[Display(Name = "Report Type")]
	[Required]
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string ReportTypeId { get; init; } = "";
	public string?  ReferenceFieldReportTypeId { get; set; }
	[Display(Name = "Widget Title")]
	[Required]
	[StringLength(150, ErrorMessage = "{0} length can't be more than {1}.")]
	public string Title { get; init; } = "";
	[Display(Name = "Maps To A DashboardQueryResultColumn.ColumnName")]
	[StringLength(150, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? XAxisColumnName { get; init; }
	[Display(Name = "Json Array Of Measure Column Names")]
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? YAxisColumnsJson { get; init; }
	[Display(Name = "Optional Grouping/Series Column")]
	[StringLength(150, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? SeriesColumnName { get; init; }
	[Display(Name = "Overrides The Result Column`s Default Aggregation")]
	[StringLength(20, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? AggregationOverride { get; init; }
	[Display(Name = "Optional Dashboard Opened On Widget Click")]
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? DrillDownDashboardId { get; init; }
	public string?  ReferenceFieldDrillDownDashboardId { get; set; }
	[Display(Name = "Grid Column Position")]
	[Required]
	public int GridPositionX { get; init; } = 0;
	[Display(Name = "Grid Row Position")]
	[Required]
	public int GridPositionY { get; init; } = 0;
	[Display(Name = "Grid Width In Columns")]
	[Required]
	public int GridWidth { get; init; } = 0;
	[Display(Name = "Grid Height In Rows")]
	[Required]
	public int GridHeight { get; init; } = 0;
	[Display(Name = "Overrides Dashboard-Level Refresh Interval")]
	public int? RefreshIntervalOverrideSeconds { get; init; } = 0;
	[Display(Name = "Tab/Load Order")]
	public int? Sequence { get; init; } = 0;
	
	public DateTime LastModifiedDate { get; set; }
	public DashboardViewModel? Dashboard { get; init; }
	public DashboardQueryViewModel? DashboardQuery { get; init; }
	public ReportTypeViewModel? ReportType { get; init; }
	public DashboardViewModel? DrillDownDashboard { get; init; }
		
	
}
