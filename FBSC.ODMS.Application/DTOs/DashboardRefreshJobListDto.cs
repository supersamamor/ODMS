using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Application.DTOs;

public record DashboardRefreshJobListDto : BaseDto
{
	public string DashboardQueryId { get; init; } = "";
	public string TriggerType { get; init; } = "";
	public string Status { get; init; } = "";
	public DateTime? QueuedAt { get; init; }
	public string QueuedAtFormatted { get { return this.QueuedAt == null ? "" : this.QueuedAt!.Value.ToString("MMM dd, yyyy HH:mm"); } }
	public DateTime? StartedAt { get; init; }
	public string StartedAtFormatted { get { return this.StartedAt == null ? "" : this.StartedAt!.Value.ToString("MMM dd, yyyy HH:mm"); } }
	public DateTime? CompletedAt { get; init; }
	public string CompletedAtFormatted { get { return this.CompletedAt == null ? "" : this.CompletedAt!.Value.ToString("MMM dd, yyyy HH:mm"); } }
	public int? DurationMs { get; init; }
	public string DurationMsFormatted { get { return this.DurationMs == null ? "" : this.DurationMs!.Value.ToString("#,##0"); } }
	public int? RowsCached { get; init; }
	public string RowsCachedFormatted { get { return this.RowsCached == null ? "" : this.RowsCached!.Value.ToString("#,##0"); } }
	public int? RetryCount { get; init; }
	public string RetryCountFormatted { get { return this.RetryCount == null ? "" : this.RetryCount!.Value.ToString("#,##0"); } }
	public int? MaxRetries { get; init; }
	public string MaxRetriesFormatted { get { return this.MaxRetries == null ? "" : this.MaxRetries!.Value.ToString("#,##0"); } }
	public string? ErrorRemarks { get; init; }
	
	
}
