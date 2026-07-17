using FBSC.Common.Web.Utility.Extensions;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using FBSC.Common.Web.Utility.Annotations;

namespace FBSC.ODMS.Web.Areas.ODMS.Models;

public record DashboardThemeViewModel : BaseViewModel
{	
	[Display(Name = "Code")]
	[Required]
	[StringLength(30, ErrorMessage = "{0} length can't be more than {1}.")]
	public string Code { get; init; } = "";
	[Display(Name = "Name")]
	[Required]
	[StringLength(100, ErrorMessage = "{0} length can't be more than {1}.")]
	public string Name { get; init; } = "";
	[Display(Name = "Is Dark Mode")]
	public bool IsDarkMode { get; init; } = false;
	[Display(Name = "Primary Seed Color")]
	[Required]
	[StringLength(10, ErrorMessage = "{0} length can't be more than {1}.")]
	public string PrimaryColorHex { get; init; } = "";
	[Display(Name = "Generated Palette As Json Array Of Hex Values")]
	
	public string? ColorPaletteJson { get; init; }
	[Display(Name = "Sequential, Categorical, Or Diverging")]
	[StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? GenerationAlgorithm { get; init; }
	[Display(Name = "Is System Default")]
	public bool IsSystemDefault { get; init; } = false;
	
	public DateTime LastModifiedDate { get; set; }
		
	public IList<DashboardViewModel>? DashboardList { get; set; }
	
}
