using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Core.ODMS;

public record DashboardQueryResultCacheState : BaseEntity
{
	public string DashboardQueryId { get; init; } = "";
	public string ParameterSetHash { get; init; } = "";
	public string? ResultJson { get; init; }
	public int? RowCount { get; init; }
	public int? CacheSizeBytes { get; init; }
	public DateTime? CachedAt { get; init; }
	public DateTime? ExpiresAt { get; init; }
	public bool IsStale { get; init; }
	
	public DashboardQueryState? DashboardQuery { get; init; }
	
	
}
