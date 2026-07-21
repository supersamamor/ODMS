using FBSC.Common.Web.Utility.Extensions;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using FBSC.Common.Web.Utility.Annotations;

namespace FBSC.ODMS.Web.Areas.ODMS.Models;

public record StatusReportMilestoneViewModel : BaseViewModel
{	
		[Display(Name = "Milestone Name")]
	[Required]
	[StringLength(255, ErrorMessage = "{0} length can't be more than {1}.")]
	public string Name { get; init; } = "";
	[Display(Name = "Milestone Start Date")]
	[Required]
	public DateTime StartDate { get; init; } = DateTime.Now.Date;
	[Display(Name = "Milestone Target End Date")]
	[Required]
	public DateTime TargetEndDate { get; init; } = DateTime.Now.Date;
	[Display(Name = "Milestone Status (Not Started, In Progress, Completed)")]
	[Required]
	[StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
	public string Status { get; init; } = "";
	
	public DateTime LastModifiedDate { get; set; }
	public StatusReportViewModel? StatusReport { get; init; }
		
	
}
