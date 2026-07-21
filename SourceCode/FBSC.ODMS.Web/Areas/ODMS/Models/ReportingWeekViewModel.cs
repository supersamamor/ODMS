using FBSC.Common.Web.Utility.Extensions;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using FBSC.Common.Web.Utility.Annotations;

namespace FBSC.ODMS.Web.Areas.ODMS.Models;

public record ReportingWeekViewModel : BaseViewModel
{	
	[Display(Name = "Reporting Year (e.g. 2026)")]
	[Required]
	public int Year { get; init; } = 0;
	[Display(Name = "Work Week Number (e.g. 29)")]
	[Required]
	public int WeekNumber { get; init; } = 0;
	[Display(Name = "Week Start Date")]
	[Required]
	public DateTime StartDate { get; init; } = DateTime.Now.Date;
	[Display(Name = "Week End Date")]
	[Required]
	public DateTime EndDate { get; init; } = DateTime.Now.Date;
	
	public DateTime LastModifiedDate { get; set; }
		
	public IList<StatusReportViewModel>? StatusReportList { get; set; }
	
}
