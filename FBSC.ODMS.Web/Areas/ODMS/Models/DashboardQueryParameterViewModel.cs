using FBSC.Common.Web.Utility.Extensions;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using FBSC.Common.Web.Utility.Annotations;

namespace FBSC.ODMS.Web.Areas.ODMS.Models;

public record DashboardQueryParameterViewModel : BaseViewModel
{	
	[Display(Name = "Dashboard Query")]
	[Required]
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string DashboardQueryId { get; init; } = "";
	public string?  ReferenceFieldDashboardQueryId { get; set; }
	[Display(Name = "Sql Parameter Placeholder, E.g. @DateFrom")]
	[Required]
	[StringLength(100, ErrorMessage = "{0} length can't be more than {1}.")]
	public string ParameterName { get; init; } = "";
	[Display(Name = "Parameter Data Type")]
	[Required]
	[StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
	public string DataType { get; init; } = "";
	[Display(Name = "Dropdown, DateRange, Text, Or MultiSelect")]
	[Required]
	[StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
	public string ControlType { get; init; } = "";
	[Display(Name = "Default Value")]
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? DefaultValue { get; init; }
	[Display(Name = "Is Required")]
	public bool IsRequired { get; init; } = false;
	[Display(Name = "Optional Sql That Populates This Parameter`s Dropdown Values")]
	
	public string? LookupSourceQuery { get; init; }
	[Display(Name = "Filter Bar Order")]
	[Required]
	public int Sequence { get; init; } = 0;
	
	public DateTime LastModifiedDate { get; set; }
	public DashboardQueryViewModel? DashboardQuery { get; init; }
		
	
}
