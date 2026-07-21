using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Core.ODMS;

public record StatusReportRiskIssueState : BaseEntity
{
	public string StatusReportId { get; init; } = "";
	public string Code { get; init; } = "";
	public string Type { get; init; } = "";
	public string Title { get; init; } = "";
	public string Severity { get; init; } = "";
	public string Status { get; init; } = "";
	public string OwnerId { get; init; } = "";
	public DateTime DateRaised { get; init; }
	public string? Notes { get; init; }
	
	public StatusReportState? StatusReport { get; init; }
	public EmployeeState? Owner { get; init; }
	
	
}
