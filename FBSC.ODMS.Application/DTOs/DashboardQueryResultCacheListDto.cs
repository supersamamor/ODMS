using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Application.DTOs;

public record DashboardQueryResultCacheListDto : BaseDto
{
	public string DashboardQueryId { get; init; } = "";
			public int? RowCount { get; init; }
	public string RowCountFormatted { get { return this.RowCount == null ? "" : this.RowCount!.Value.ToString("#,##0"); } }
	public int? CacheSizeBytes { get; init; }
	public string CacheSizeBytesFormatted { get { return this.CacheSizeBytes == null ? "" : this.CacheSizeBytes!.Value.ToString("#,##0"); } }
	public DateTime? CachedAt { get; init; }
	public string CachedAtFormatted { get { return this.CachedAt == null ? "" : this.CachedAt!.Value.ToString("MMM dd, yyyy HH:mm"); } }
	public DateTime? ExpiresAt { get; init; }
	public string ExpiresAtFormatted { get { return this.ExpiresAt == null ? "" : this.ExpiresAt!.Value.ToString("MMM dd, yyyy HH:mm"); } }
	public bool IsStale { get; init; }
	public string IsStaleFormatted { get { return this.IsStale == true ? "Yes" : "No"; } }
	
	
}
