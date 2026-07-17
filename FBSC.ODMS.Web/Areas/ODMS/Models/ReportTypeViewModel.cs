using FBSC.Common.Web.Utility.Extensions;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using FBSC.Common.Web.Utility.Annotations;

namespace FBSC.ODMS.Web.Areas.ODMS.Models;

public record ReportTypeViewModel : BaseViewModel
{	
	[Display(Name = "Code")]
	[Required]
	[StringLength(30, ErrorMessage = "{0} length can't be more than {1}.")]
	public string Code { get; init; } = "";
	[Display(Name = "Name")]
	[Required]
	[StringLength(100, ErrorMessage = "{0} length can't be more than {1}.")]
	public string Name { get; init; } = "";
	[Display(Name = "Underlying Chart.js Type Or Widget Renderer Key")]
	[Required]
	[StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
	public string ChartRenderer { get; init; } = "";
	[Display(Name = "Minimum Result Columns Needed")]
	public int? MinColumnsRequired { get; init; } = 0;
	[Display(Name = "Maximum Result Columns Supported")]
	public int? MaxColumnsRequired { get; init; } = 0;
	[Display(Name = "Requires X-Axis / Category Column")]
	public bool RequiresXAxis { get; init; } = false;
	[Display(Name = "Requires Y-Axis / Measure Column")]
	public bool RequiresYAxis { get; init; } = false;
	[Display(Name = "Requires Series / Grouping Column")]
	public bool RequiresSeriesGrouping { get; init; } = false;
	[Display(Name = "Icon Class For Picker UI")]
	[StringLength(100, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? IconClass { get; init; }
	[Display(Name = "Is Active")]
	public bool IsActive { get; init; } = false;
	
	public DateTime LastModifiedDate { get; set; }
		
	public IList<DashboardWidgetViewModel>? DashboardWidgetList { get; set; }
	
}
