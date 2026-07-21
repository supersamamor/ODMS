using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Application.DTOs;

public record ProjectHistoryListDto : BaseDto
{
    public string ProjectName { get; init; } = "";
    public string BusinessUnitId { get; init; } = "";
    public string Priority { get; init; } = "";
    public DateTime BaselineStartDate { get; init; }
    public string BaselineStartDateFormatted { get { return this.BaselineStartDate.ToString("MMM dd, yyyy"); } }
    public DateTime BaselineEndDate { get; init; }
    public string BaselineEndDateFormatted { get { return this.BaselineEndDate.ToString("MMM dd, yyyy"); } }
    public decimal? ApprovedBudget { get; init; }
    public string ApprovedBudgetFormatted { get { return this.ApprovedBudget == null ? "" : this.ApprovedBudget!.Value.ToString("N2"); } }
    public string ProjectManagerId { get; init; } = string.Empty;
    public string? Phase { get; init; }
    public string? ScheduleStatus { get; init; }
    public DateTime? LastReviewDate { get; init; }
    public string LastReviewDateFormatted { get { return this.LastReviewDate == null ? "" : this.LastReviewDate!.Value.ToString("MMM dd, yyyy"); } }
    public DateTime? LastUpdatedDate { get; init; }
    public string LastUpdatedDateFormatted { get { return this.LastUpdatedDate == null ? "" : this.LastUpdatedDate!.Value.ToString("MMM dd, yyyy"); } }


}
