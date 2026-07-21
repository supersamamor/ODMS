using FBSC.Common.Core.Base.Models;

namespace FBSC.ODMS.Core.ODMS;

/// <summary>
/// Columns shared by <see cref="ProjectState"/> and <see cref="ProjectHistoryState"/>,
/// per the final ODMS DatabaseStructure workbook. Single source of truth so the
/// history table can never drift from the main table.
/// </summary>
public abstract record ProjectBase : BaseEntity
{
    public string ProjectCode { get; init; } = "";
    public string ProjectName { get; init; } = "";
    public string DeliveryTower { get; init; } = "";
    public string DemandType { get; init; } = "";
    public string BusinessUnitId { get; init; } = "";
    public string? TechnologyBusinessPartnerId { get; init; }
    public string Priority { get; init; } = "";
    public DateTime BaselineStartDate { get; init; }
    public DateTime BaselineEndDate { get; init; }
    public decimal? ApprovedBudget { get; init; }
    public string? ProjectDescription { get; init; }
    public string ProjectManagerId { get; init; } = "";
    public string? DeputyProjectManagerId { get; init; }
    public string? ActiveStatus { get; init; }
    public string? SOWFileName { get; init; }
    public bool NoSOW { get; init; }
}
