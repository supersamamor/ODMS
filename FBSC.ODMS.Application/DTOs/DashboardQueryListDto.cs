using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Application.DTOs;

public record DashboardQueryListDto : BaseDto
{
	public string DataSourceId { get; init; } = "";
	public string Name { get; init; } = "";
	public string? Description { get; init; }
			public bool IsParameterized { get; init; }
	public string IsParameterizedFormatted { get { return this.IsParameterized == true ? "Yes" : "No"; } }
	public bool GeneratedByAI { get; init; }
	public string GeneratedByAIFormatted { get { return this.GeneratedByAI == true ? "Yes" : "No"; } }
	public string? ValidationStatus { get; init; }
	public DateTime? LastValidatedAt { get; init; }
	public string LastValidatedAtFormatted { get { return this.LastValidatedAt == null ? "" : this.LastValidatedAt!.Value.ToString("MMM dd, yyyy HH:mm"); } }
	public int? LastExecutionDurationMs { get; init; }
	public string LastExecutionDurationMsFormatted { get { return this.LastExecutionDurationMs == null ? "" : this.LastExecutionDurationMs!.Value.ToString("#,##0"); } }
	public DateTime? LastExecutedAt { get; init; }
	public string LastExecutedAtFormatted { get { return this.LastExecutedAt == null ? "" : this.LastExecutedAt!.Value.ToString("MMM dd, yyyy HH:mm"); } }
	public string? LastExecutionErrorRemarks { get; init; }
	public bool IsActive { get; init; }
	public string IsActiveFormatted { get { return this.IsActive == true ? "Yes" : "No"; } }
	
	public string StatusBadge { get; set; } = "";
}
