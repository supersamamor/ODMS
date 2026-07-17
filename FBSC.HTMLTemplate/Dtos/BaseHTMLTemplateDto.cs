namespace FBSC.HTMLTemplate.Dtos
{
    public record BaseHTMLTemplateDto
    {
        public string Id { get; init; } = "";
        public DateTime LastModifiedDate { get; set; }
    }
}
