using FBSC.Common.Core.Base.Models;

namespace FBSC.ODMS.Core.ODMS;

/// <summary>
/// Mapping table linking a BusinessUnit to the Employee(s) who serve as its
/// Technology Business Partner. Exposed on BusinessUnit as
/// <c>TechnologyBusinessPartnerList</c>.
/// </summary>
public record BusinessUnitTechnologyBusinessPartnerState : BaseEntity
{
	public string BusinessUnitId { get; init; } = "";
	public string EmployeeId { get; init; } = "";

	public BusinessUnitState? BusinessUnit { get; init; }
	public EmployeeState? Employee { get; init; }
}
