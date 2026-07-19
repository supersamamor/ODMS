using System.ComponentModel.DataAnnotations;
namespace FBSC.HTMLTemplate.ViewModels;
public record HTMLTemplateViewModel : BaseHTMLTemplateViewModel
{	
	[Display(Name = "Name")]
	[Required]
	[StringLength(255, ErrorMessage = "{0} length can't be more than {1}.")]
	public string HTMLTemplateName { get; init; } = "";
	[Display(Name = "Description")]
	[Required]
	
	public string Description { get; init; } = "";
	[Display(Name = "HTML Template")]
	[Required]
	
	public string? HTMLTemplate { get; init; } = "";
	public string? HTMLFooterTemplate { get; init; } = "";
    public string? CustomCss { get; init; }
    public string Orientation { get; init; } = nameof(Rotativa.AspNetCore.Options.Orientation.Portrait);
    public string PaperSize { get; init; } = nameof(Rotativa.AspNetCore.Options.Size.A4);
	public int MarginTop { get; init; } = 10;
    public int MarginBottom { get; init; } = 10;
    public int MarginLeft { get; init; } = 10;
    public int MarginRight { get; init; } = 10;
    public DateTime LastModifiedDate { get; set; }
	public string? CustomSwitch { get; init; }
    public string? CssLibraries { get; init; }
	
}
