using FBSC.Common.Web.Utility.Extensions;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using FBSC.Common.Web.Utility.Annotations;

namespace FBSC.ODMS.Web.Areas.ODMS.Models;

public record StatusReportRiskIssueViewModel : BaseViewModel
{	
		[Display(Name = "Risk / Issue Code (e.g. R-001, I-001)")]
	[Required]
	[StringLength(20, ErrorMessage = "{0} length can't be more than {1}.")]
	public string Code { get; init; } = "";
	[Display(Name = "Type (Risk or Issue)")]
	[Required]
	[StringLength(20, ErrorMessage = "{0} length can't be more than {1}.")]
	public string Type { get; init; } = "";
	[Display(Name = "Risk / Issue Title")]
	[Required]
	[StringLength(255, ErrorMessage = "{0} length can't be more than {1}.")]
	public string Title { get; init; } = "";
	[Display(Name = "Severity (High, Medium, Low)")]
	[Required]
	[StringLength(20, ErrorMessage = "{0} length can't be more than {1}.")]
	public string Severity { get; init; } = "";
	[Display(Name = "Status (Open, In Progress, Closed)")]
	[Required]
	[StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
	public string Status { get; init; } = "";
	[Display(Name = "Risk / Issue Owner")]
	[Required]
	
	public string OwnerId { get; init; } = "";
	public string?  ReferenceFieldOwnerId { get; set; }
	[Display(Name = "Date Raised")]
	[Required]
	public DateTime DateRaised { get; init; } = DateTime.Now.Date;
	[Display(Name = "Notes / Remarks")]
	[StringLength(1000, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? Notes { get; init; }
	
	public DateTime LastModifiedDate { get; set; }
	public StatusReportViewModel? StatusReport { get; init; }
	public EmployeeViewModel? Employee { get; init; }
		
	
}
