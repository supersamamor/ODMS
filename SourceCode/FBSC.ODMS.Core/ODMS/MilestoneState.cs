using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Core.ODMS;

public record MilestoneState : BaseEntity
{
	public string Name { get; init; } = "";
	
	
	public IList<ProjectMilestoneState>? ProjectMilestoneList { get; set; }
	
}
