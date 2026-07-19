namespace FBSC.HTMLTemplate.Dtos;

public record HTMLTemplateListDto : BaseHTMLTemplateDto
{
	public string HTMLTemplateName { get; init; } = "";	
	public string StatusBadge { get; set; } = "";
}
