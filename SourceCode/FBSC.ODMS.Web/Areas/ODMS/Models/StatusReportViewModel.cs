using FBSC.Common.Web.Utility.Extensions;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using FBSC.Common.Web.Utility.Annotations;

namespace FBSC.ODMS.Web.Areas.ODMS.Models;

public record StatusReportViewModel : BaseViewModel
{	
	[Display(Name = "Reference to Project")]
	[Required]
	
	public string ProjectId { get; init; } = "";
	public string?  ReferenceFieldProjectId { get; set; }
	[Display(Name = "Reference to Reporting Week")]
	// Resolved server-side from SelectedWeekNumber (get-or-create ReportingWeek row).
	public string ReportingWeekId { get; init; } = "";
	public string?  ReferenceFieldReportingWeekId { get; set; }
	[Display(Name = "Date Report Was Submitted")]
	public DateTime? SubmissionDate { get; init; } = DateTime.Now.Date;
	[Display(Name = "Overall RAG Status (Red, Amber, Green)")]
	[StringLength(20, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? OverallHealth { get; init; }
	[Display(Name = "Report Status (Pending Review, Approved, Changes Requested)")]
	// Stamped server-side (defaults to Pending Review on create).
	[StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
	public string Status { get; init; } = "";
	[Display(Name = "Actual Spend in Currency")]
	
	[DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
	public decimal? ActualSpend { get; init; } = 0;
	[Display(Name = "Schedule Variance in Weeks")]
	public int? ScheduleVarianceWeeks { get; init; } = 0;
	[Display(Name = "Schedule Status (On Track, Delayed)")]
	[StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? ScheduleStatus { get; init; }
	[Display(Name = "Budget Status (On Track, At Risk, Overbudget)")]
	[StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? BudgetStatus { get; init; }
	[Display(Name = "Current Phase (e.g. Development)")]
	[StringLength(100, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? Phase { get; init; }
	[Display(Name = "Reviewed By")]
	
	public string? ReviewedById { get; init; }
	[Display(Name = "Date Reviewed")]
	public DateTime? ReviewedDate { get; init; } = DateTime.Now.Date;
	[Display(Name = "Review Comments")]
	[StringLength(1000, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? ReviewComments { get; init; }
	
	// Dynamic string lists (JSON columns on the StatusReport table). Bound from
	// the [+]/[-] list components on the Add page; empty rows are pruned on post.
	[Display(Name = "Accomplishments This Week")]
	public IList<string>? Accomplishments { get; set; }
	[Display(Name = "Next Steps")]
	public IList<string>? NextSteps { get; set; }

	// The ISO week the report covers - resolved server-side to a ReportingWeek row.
	[Display(Name = "Reporting Week")]
	[Required(ErrorMessage = "'Reporting Week' is required.")]
	public int? SelectedWeekNumber { get; init; }

	// Read-only header context for the Create Report page.
	public string? ProjectName { get; set; }
	public string? BusinessUnitName { get; set; }
	public string? ProjectManagerName { get; set; }
	public string? PriorWeekHealth { get; set; }
	public decimal? BaselineBudget { get; set; }

	public DateTime LastModifiedDate { get; set; }
	public ProjectViewModel? Project { get; init; }
	public ReportingWeekViewModel? ReportingWeek { get; init; }

	public IList<StatusReportHealthIndicatorViewModel>? StatusReportHealthIndicatorList { get; set; }
	public IList<StatusReportMilestoneViewModel>? StatusReportMilestoneList { get; set; }
	public IList<StatusReportRiskIssueViewModel>? StatusReportRiskIssueList { get; set; }

}
