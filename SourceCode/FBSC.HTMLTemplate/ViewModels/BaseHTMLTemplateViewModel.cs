namespace FBSC.HTMLTemplate.ViewModels
{
    public record BaseHTMLTemplateViewModel
    {
        public string Id { get; init; } = Guid.NewGuid().ToString();
    }
}
