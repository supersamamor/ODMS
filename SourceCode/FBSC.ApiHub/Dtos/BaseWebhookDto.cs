namespace FBSC.ApiHub.Dtos
{
    public record BaseWebhookDto
    {
        public string Id { get; init; } = "";
        public DateTime LastModifiedDate { get; set; }
    }
}
