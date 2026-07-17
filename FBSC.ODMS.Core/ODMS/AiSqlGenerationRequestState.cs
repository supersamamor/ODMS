using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Core.ODMS;

public record AiSqlGenerationRequestState : BaseEntity
{
	public string DataSourceId { get; init; } = "";
	public string NaturalLanguagePrompt { get; init; } = "";
	public string? GeneratedSqlQueryText { get; init; }
	public string? DashboardQueryId { get; init; }
	public int? ConfidenceScore { get; init; }
	public string? ValidationStatus { get; init; }
	public string? ErrorRemarks { get; init; }
	public string? RequestedBy { get; init; }
	public DateTime? GeneratedAt { get; init; }
	
	public DataSourceState? DataSource { get; init; }
	public DashboardQueryState? DashboardQuery { get; init; }
	
	
}
