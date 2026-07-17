using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.HTMLTemplate.Models;

public record HTMLTemplateState : BaseEntity
{
    public string HTMLTemplateName { get; init; } = "";
    public string Description { get; init; } = "";
    public string HTMLTemplate { get; private set; } = "";
    public string HTMLFooterTemplate { get; init; } = "";
    public string? CustomCss { get; init; }
    public string Orientation { get; init; } = "";
    public string PaperSize { get; init; } = "";
    public int MarginTop { get; init; }
    public int MarginBottom { get; init; }
    public int MarginLeft { get; init; }
    public int MarginRight { get; init; }
    public string? CustomSwitch { get; init; }
    public string? CssLibraries { get; init; }
    public void SetHTMLTemplate(string htmlTemplate)
    {
        this.HTMLTemplate = htmlTemplate;
    }
}
