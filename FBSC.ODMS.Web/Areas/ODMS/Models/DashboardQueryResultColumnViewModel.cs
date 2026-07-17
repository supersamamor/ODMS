using FBSC.Common.Web.Utility.Extensions;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using FBSC.Common.Web.Utility.Annotations;

namespace FBSC.ODMS.Web.Areas.ODMS.Models;

public record DashboardQueryResultColumnViewModel : BaseViewModel
{	
	[Display(Name = "Dashboard Query")]
	[Required]
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string DashboardQueryId { get; init; } = "";
	public string?  ReferenceFieldDashboardQueryId { get; set; }
	[Display(Name = "Column Name As Returned By The Query")]
	[Required]
	[StringLength(150, ErrorMessage = "{0} length can't be more than {1}.")]
	public string ColumnName { get; init; } = "";
	[Display(Name = "Column Order In Result Set")]
	[Required]
	public int OrdinalPosition { get; init; } = 0;
	[Display(Name = "Native Sql Data Type Returned")]
	[StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? SqlDataType { get; init; }
	[Display(Name = "Dimension, Measure, DateAxis, Label, Or Identifier")]
	[Required]
	[StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
	public string InferredRole { get; init; } = "";
	[Display(Name = "Can Be Summed/Averaged/Counted")]
	public bool IsAggregatable { get; init; } = false;
	[Display(Name = "SUM, COUNT, AVG, MIN, Or MAX")]
	[StringLength(20, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? DefaultAggregation { get; init; }
	[Display(Name = "Display Format, E.g. Currency Or Percent Mask")]
	[StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? FormatString { get; init; }
	[Display(Name = "Column Order For Mapping Pickers")]
	[Required]
	public int Sequence { get; init; } = 0;
	
	public DateTime LastModifiedDate { get; set; }
	public DashboardQueryViewModel? DashboardQuery { get; init; }
		
	
}
