using FBSC.Common.Web.Utility.Extensions;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using FBSC.Common.Web.Utility.Annotations;

namespace FBSC.ODMS.Web.Areas.ODMS.Models;

public record DashboardRefreshJobViewModel : BaseViewModel
{	
	[Display(Name = "Dashboard Query")]
	[Required]
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string DashboardQueryId { get; init; } = "";
	public string?  ReferenceFieldDashboardQueryId { get; set; }
	[Display(Name = "Manual Or Schedule")]
	[Required]
	[StringLength(20, ErrorMessage = "{0} length can't be more than {1}.")]
	public string TriggerType { get; init; } = "";
	[Display(Name = "Status")]
	[Required]
	[StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
	public string Status { get; init; } = "";
	[Display(Name = "Queued At")]
	public DateTime? QueuedAt { get; init; } = DateTime.Now;
	[Display(Name = "Started At")]
	public DateTime? StartedAt { get; init; } = DateTime.Now;
	[Display(Name = "Completed At")]
	public DateTime? CompletedAt { get; init; } = DateTime.Now;
	[Display(Name = "Duration In Milliseconds")]
	public int? DurationMs { get; init; } = 0;
	[Display(Name = "Rows Written To Result Cache")]
	public int? RowsCached { get; init; } = 0;
	[Display(Name = "Retry Count")]
	public int? RetryCount { get; init; } = 0;
	[Display(Name = "Max Retries")]
	public int? MaxRetries { get; init; } = 0;
	[Display(Name = "Error Remarks")]
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? ErrorRemarks { get; init; }
	
	public DateTime LastModifiedDate { get; set; }
	public DashboardQueryViewModel? DashboardQuery { get; init; }
		
	
}
