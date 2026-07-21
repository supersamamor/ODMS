using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Core.ODMS;

public record ProjectMilestoneState : BaseEntity
{
	public string ProjectId { get; init; } = "";
	public string MilestoneId { get; init; } = "";
	public DateTime StartDate { get; init; }
	public DateTime TargetEndDate { get; init; }
	public string Status { get; init; } = "";
	
	public ProjectState? Project { get; init; }
	public MilestoneState? Milestone { get; init; }
	
	
}
