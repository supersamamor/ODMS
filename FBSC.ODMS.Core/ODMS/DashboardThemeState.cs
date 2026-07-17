using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Core.ODMS;

public record DashboardThemeState : BaseEntity
{
	public string Code { get; init; } = "";
	public string Name { get; init; } = "";
	public bool IsDarkMode { get; init; }
	public string PrimaryColorHex { get; init; } = "";
	public string? ColorPaletteJson { get; init; }
	public string? GenerationAlgorithm { get; init; }
	public bool IsSystemDefault { get; init; }
	
	
	public IList<DashboardState>? DashboardList { get; set; }
	
}
