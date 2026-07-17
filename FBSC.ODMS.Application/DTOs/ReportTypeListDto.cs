using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Application.DTOs;

public record ReportTypeListDto : BaseDto
{
	public string Code { get; init; } = "";
	public string Name { get; init; } = "";
	public string ChartRenderer { get; init; } = "";
	public int? MinColumnsRequired { get; init; }
	public string MinColumnsRequiredFormatted { get { return this.MinColumnsRequired == null ? "" : this.MinColumnsRequired!.Value.ToString("#,##0"); } }
	public int? MaxColumnsRequired { get; init; }
	public string MaxColumnsRequiredFormatted { get { return this.MaxColumnsRequired == null ? "" : this.MaxColumnsRequired!.Value.ToString("#,##0"); } }
	public bool RequiresXAxis { get; init; }
	public string RequiresXAxisFormatted { get { return this.RequiresXAxis == true ? "Yes" : "No"; } }
	public bool RequiresYAxis { get; init; }
	public string RequiresYAxisFormatted { get { return this.RequiresYAxis == true ? "Yes" : "No"; } }
	public bool RequiresSeriesGrouping { get; init; }
	public string RequiresSeriesGroupingFormatted { get { return this.RequiresSeriesGrouping == true ? "Yes" : "No"; } }
	public string? IconClass { get; init; }
	public bool IsActive { get; init; }
	public string IsActiveFormatted { get { return this.IsActive == true ? "Yes" : "No"; } }
	
	public string StatusBadge { get; set; } = "";
}
