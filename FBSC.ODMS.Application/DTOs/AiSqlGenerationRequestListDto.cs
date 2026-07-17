using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Application.DTOs;

public record AiSqlGenerationRequestListDto : BaseDto
{
	public string DataSourceId { get; init; } = "";
	public string NaturalLanguagePrompt { get; init; } = "";
		public string? DashboardQueryId { get; init; }
	public int? ConfidenceScore { get; init; }
	public string ConfidenceScoreFormatted { get { return this.ConfidenceScore == null ? "" : this.ConfidenceScore!.Value.ToString("#,##0"); } }
	public string? ValidationStatus { get; init; }
	public string? ErrorRemarks { get; init; }
	public string? RequestedBy { get; init; }
	public DateTime? GeneratedAt { get; init; }
	public string GeneratedAtFormatted { get { return this.GeneratedAt == null ? "" : this.GeneratedAt!.Value.ToString("MMM dd, yyyy HH:mm"); } }
	
	
}
