using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Core.ODMS;

public record DashboardQueryState : BaseEntity
{
	public string DataSourceId { get; init; } = "";
	public string Name { get; init; } = "";
	public string? Description { get; init; }
	public string SqlQueryText { get; init; } = "";
	public string? QueryHash { get; init; }
	public bool IsParameterized { get; init; }
	public bool GeneratedByAI { get; init; }
	public string? ValidationStatus { get; init; }
	public DateTime? LastValidatedAt { get; init; }
	public int? LastExecutionDurationMs { get; init; }
	public DateTime? LastExecutedAt { get; init; }
	public string? LastExecutionErrorRemarks { get; init; }
	public bool IsActive { get; init; }
	
	public DataSourceState? DataSource { get; init; }
	
	public IList<DashboardQueryParameterState>? DashboardQueryParameterList { get; set; }
	public IList<DashboardQueryResultColumnState>? DashboardQueryResultColumnList { get; set; }
	public IList<DashboardQueryResultCacheState>? DashboardQueryResultCacheList { get; set; }
	public IList<DashboardWidgetState>? DashboardWidgetList { get; set; }
	public IList<AiSqlGenerationRequestState>? AiSqlGenerationRequestList { get; set; }
	public IList<DashboardRefreshJobState>? DashboardRefreshJobList { get; set; }
	
}
