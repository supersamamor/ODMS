using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Core.ODMS;

public record DashboardRefreshJobState : BaseEntity
{
	public string DashboardQueryId { get; init; } = "";
	public string TriggerType { get; init; } = "";
	public string Status { get; init; } = "";
	public DateTime? QueuedAt { get; init; }
	public DateTime? StartedAt { get; init; }
	public DateTime? CompletedAt { get; init; }
	public int? DurationMs { get; init; }
	public int? RowsCached { get; init; }
	public int? RetryCount { get; init; }
	public int? MaxRetries { get; init; }
	public string? ErrorRemarks { get; init; }
	
	public DashboardQueryState? DashboardQuery { get; init; }
	
	
}
