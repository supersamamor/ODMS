using FBSC.Common.Core.Base.Models;

namespace FBSC.ODMS.Core.ODMS;

/// <summary>
/// Columns shared by <see cref="ProjectState"/> and its derived state records,
/// per the final ODMS DatabaseStructure workbook. Single source of truth for the
/// core Project column set.
/// </summary>
public abstract record ProjectBase : BaseEntity
{
    public string ProjectCode { get; init; } = "";
    public string ProjectName { get; init; } = "";
    public string DeliveryCategory { get; init; } = "";
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
    // Statement-of-Work documents are now multi-file: see ProjectAttachmentState.
    // NoSOW records that a project legitimately has no SOW yet.
    public bool NoSOW { get; init; }
}
