using FBSC.Common.Core.Base.Models;

namespace FBSC.ODMS.Application.DTOs;

public record ProjectListDto : BaseDto
{
	public string ProjectCode { get; init; } = "";
	public string ProjectName { get; init; } = "";
	public string DeliveryTower { get; init; } = "";
	public string DemandType { get; init; } = "";
	public string BusinessUnitId { get; init; } = "";
	public string ProjectManagerId { get; init; } = "";
	public string Priority { get; init; } = "";
	public string? ActiveStatus { get; init; }
	public DateTime BaselineStartDate { get; init; }
	public string BaselineStartDateFormatted { get { return this.BaselineStartDate.ToString("MMM dd, yyyy"); } }
	public DateTime BaselineEndDate { get; init; }
	public string BaselineEndDateFormatted { get { return this.BaselineEndDate.ToString("MMM dd, yyyy"); } }
	public decimal? ApprovedBudget { get; init; }
	public string ApprovedBudgetFormatted { get { return this.ApprovedBudget == null ? "" : this.ApprovedBudget!.Value.ToString("N2"); } }

}
