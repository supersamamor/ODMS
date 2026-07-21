using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Core.ODMS;

public record StatusReportHealthIndicatorState : BaseEntity
{
	public string StatusReportId { get; init; } = "";
	public string Area { get; init; } = "";
	public string Status { get; init; } = "";
	public string? Comment { get; init; }
	
	public StatusReportState? StatusReport { get; init; }
	
	
}
