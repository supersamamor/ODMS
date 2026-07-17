using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Application.DTOs;

public record DashboardListDto : BaseDto
{
	public string Code { get; init; } = "";
	public string Name { get; init; } = "";
	public string? Description { get; init; }
	public string? Category { get; init; }
	public string? DashboardThemeId { get; init; }
	public string OwnerUserId { get; init; } = "";
	public bool IsPublic { get; init; }
	public string IsPublicFormatted { get { return this.IsPublic == true ? "Yes" : "No"; } }
	public bool IsTemplate { get; init; }
	public string IsTemplateFormatted { get { return this.IsTemplate == true ? "Yes" : "No"; } }
	public int? RefreshIntervalSeconds { get; init; }
	public string RefreshIntervalSecondsFormatted { get { return this.RefreshIntervalSeconds == null ? "" : this.RefreshIntervalSeconds!.Value.ToString("#,##0"); } }
	public DateTime? LastPublishedAt { get; init; }
	public string LastPublishedAtFormatted { get { return this.LastPublishedAt == null ? "" : this.LastPublishedAt!.Value.ToString("MMM dd, yyyy HH:mm"); } }
	public bool IsActive { get; init; }
	public string IsActiveFormatted { get { return this.IsActive == true ? "Yes" : "No"; } }
	
	public string StatusBadge { get; set; } = "";
}
