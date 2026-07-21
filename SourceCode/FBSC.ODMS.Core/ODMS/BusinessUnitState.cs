using FBSC.Common.Core.Base.Models;

namespace FBSC.ODMS.Core.ODMS;

public record BusinessUnitState : BaseEntity
{
	public string Name { get; init; } = "";
	/// <summary>Short unique code (e.g. FLI) used as the ProjectCode prefix. Immutable after creation.</summary>
	public string Code { get; init; } = "";


	public IList<ProjectState>? ProjectList { get; set; }
	public IList<BusinessUnitTechnologyBusinessPartnerState>? TechnologyBusinessPartnerList { get; set; }

}
