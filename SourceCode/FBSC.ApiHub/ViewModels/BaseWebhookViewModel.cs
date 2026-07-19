namespace FBSC.ApiHub.ViewModels
{
    public record BaseWebhookViewModel
    {
        public string Id { get; init; } = Guid.NewGuid().ToString();
    }
}
