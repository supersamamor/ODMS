using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Core.ODMS;

public record BusinessUnitState : BaseEntity
{
	public string Name { get; init; } = "";
	
	
	public IList<ProjectState>? ProjectList { get; set; }
	public IList<ProjectHistoryState>? ProjectHistoryList { get; set; }
	
}
