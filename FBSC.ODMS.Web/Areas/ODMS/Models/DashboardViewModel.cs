using FBSC.Common.Web.Utility.Extensions;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using FBSC.Common.Web.Utility.Annotations;

namespace FBSC.ODMS.Web.Areas.ODMS.Models;

public record DashboardViewModel : BaseViewModel
{	
	[Display(Name = "Code")]
	[Required]
	[StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
	public string Code { get; init; } = "";
	[Display(Name = "Name")]
	[Required]
	[StringLength(150, ErrorMessage = "{0} length can't be more than {1}.")]
	public string Name { get; init; } = "";
	[Display(Name = "Description")]
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? Description { get; init; }
	[Display(Name = "HRIS, Accounting, Sales, Or Custom Grouping")]
	[StringLength(100, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? Category { get; init; }
	[Display(Name = "Dashboard Theme")]
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? DashboardThemeId { get; init; }
	public string?  ReferenceFieldDashboardThemeId { get; set; }
	[Display(Name = "Owner")]
	[Required]
	[StringLength(100, ErrorMessage = "{0} length can't be more than {1}.")]
	public string OwnerUserId { get; init; } = "";
	[Display(Name = "Is Public")]
	public bool IsPublic { get; init; } = false;
	[Display(Name = "Is Reusable Template")]
	public bool IsTemplate { get; init; } = false;
	[Display(Name = "Default Widget Refresh Interval")]
	public int? RefreshIntervalSeconds { get; init; } = 0;
	[Display(Name = "Last Published At")]
	public DateTime? LastPublishedAt { get; init; } = DateTime.Now;
	[Display(Name = "Is Active")]
	public bool IsActive { get; init; } = false;
	
	public DateTime LastModifiedDate { get; set; }
	public DashboardThemeViewModel? DashboardTheme { get; init; }
		
	public IList<DashboardWidgetViewModel>? DashboardWidgetList { get; set; }
	public IList<DashboardWidgetViewModel>? DrillDownDashboardWidgetList { get; set; }
	public IList<DashboardAccessViewModel>? DashboardAccessList { get; set; }
	
}
