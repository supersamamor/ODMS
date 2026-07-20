using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Application.DTOs;

public record ProjectListDto : BaseDto
{
	public string ProjectName { get; init; } = "";
	public string BusinessUnitId { get; init; } = "";
	public string Priority { get; init; } = "";
	public DateTime StartDate { get; init; }
	public string StartDateFormatted { get { return this.StartDate.ToString("MMM dd, yyyy"); } }
	public DateTime TargetEndDate { get; init; }
	public string TargetEndDateFormatted { get { return this.TargetEndDate.ToString("MMM dd, yyyy"); } }
	public decimal? EstimatedBudget { get; init; }
	public string EstimatedBudgetFormatted { get { return this.EstimatedBudget == null ? "" : this.EstimatedBudget!.Value.ToString("N2"); } }
	public string ProjectManagerId { get; init; } = "";
	public string? HealthStatus { get; init; }
	public string? Phase { get; init; }
	public string? ScheduleStatus { get; init; }
	public DateTime? LastReviewDate { get; init; }
	public string LastReviewDateFormatted { get { return this.LastReviewDate == null ? "" : this.LastReviewDate!.Value.ToString("MMM dd, yyyy"); } }
	public DateTime? LastUpdatedDate { get; init; }
	public string LastUpdatedDateFormatted { get { return this.LastUpdatedDate == null ? "" : this.LastUpdatedDate!.Value.ToString("MMM dd, yyyy"); } }
	
	
}
