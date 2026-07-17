using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Application.DTOs;

public record DashboardThemeListDto : BaseDto
{
	public string Code { get; init; } = "";
	public string Name { get; init; } = "";
	public bool IsDarkMode { get; init; }
	public string IsDarkModeFormatted { get { return this.IsDarkMode == true ? "Yes" : "No"; } }
	public string PrimaryColorHex { get; init; } = "";
		public string? GenerationAlgorithm { get; init; }
	public bool IsSystemDefault { get; init; }
	public string IsSystemDefaultFormatted { get { return this.IsSystemDefault == true ? "Yes" : "No"; } }
	
	
}
