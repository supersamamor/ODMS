using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Application.DTOs;

public record DataSourceListDto : BaseDto
{
	public string Name { get; init; } = "";
	public string SystemType { get; init; } = "";
	public string ConnectionMode { get; init; } = "";
	public string? ServerAddress { get; init; }
	public string? DatabaseName { get; init; }
	public string? AuthenticationType { get; init; }
	public string? Username { get; init; }
			public string? Description { get; init; }
	public bool IsActive { get; init; }
	public string IsActiveFormatted { get { return this.IsActive == true ? "Yes" : "No"; } }
	public DateTime? LastTestedAt { get; init; }
	public string LastTestedAtFormatted { get { return this.LastTestedAt == null ? "" : this.LastTestedAt!.Value.ToString("MMM dd, yyyy HH:mm"); } }
	public string? LastTestStatus { get; init; }
	
	public string StatusBadge { get; set; } = "";
}
