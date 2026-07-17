using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Core.ODMS;

public record ReportTypeState : BaseEntity
{
	public string Code { get; init; } = "";
	public string Name { get; init; } = "";
	public string ChartRenderer { get; init; } = "";
	public int? MinColumnsRequired { get; init; }
	public int? MaxColumnsRequired { get; init; }
	public bool RequiresXAxis { get; init; }
	public bool RequiresYAxis { get; init; }
	public bool RequiresSeriesGrouping { get; init; }
	public string? IconClass { get; init; }
	public bool IsActive { get; init; }
	
	
	public IList<DashboardWidgetState>? DashboardWidgetList { get; set; }
	
}
