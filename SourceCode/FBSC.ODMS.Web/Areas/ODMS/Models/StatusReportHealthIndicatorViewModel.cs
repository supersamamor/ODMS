using FBSC.Common.Web.Utility.Extensions;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using FBSC.Common.Web.Utility.Annotations;

namespace FBSC.ODMS.Web.Areas.ODMS.Models;

public record StatusReportHealthIndicatorViewModel : BaseViewModel
{	
		[Display(Name = "Health Area (Scope, Schedule, Budget, Quality, Risks, Issues)")]
	[Required]
	[StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
	public string Area { get; init; } = "";
	[Display(Name = "RAG Status (Red, Amber, Green)")]
	[Required]
	[StringLength(20, ErrorMessage = "{0} length can't be more than {1}.")]
	public string Status { get; init; } = "";
	[Display(Name = "Comment")]
	[StringLength(500, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? Comment { get; init; }
	
	public DateTime LastModifiedDate { get; set; }
	public StatusReportViewModel? StatusReport { get; init; }
		
	
}
