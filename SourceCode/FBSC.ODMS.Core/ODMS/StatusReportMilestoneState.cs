using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Core.ODMS;

public record StatusReportMilestoneState : BaseEntity
{
	public string StatusReportId { get; init; } = "";
	public string Name { get; init; } = "";
	public DateTime StartDate { get; init; }
	public DateTime TargetEndDate { get; init; }
	public string Status { get; init; } = "";
	
	public StatusReportState? StatusReport { get; init; }
	
	
}
