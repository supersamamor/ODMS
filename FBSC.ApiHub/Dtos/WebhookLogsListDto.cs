using FBSC.ApiHub.Models;
namespace FBSC.ApiHub.Dtos;

public record WebhookLogsListDto : BaseWebhookDto
{
    public string WebhookApiName { get; init; } = "";
    public string WebhookEventAssignmentId { get; init; } = "";
    public string? Event { get; init; } = "";
    public string? Error { get; init; } = "";
    public string? Status { get; init; } = "";
    public string? DataId { get; init; } = "";    
    public string WebhookApiId { get; init; } = "";
    public string StatusBadge
    {
        get
        {
            return this.Status switch
            {
                WebhookStatus.Done => @"<span class=""badge bg-success"">" + this.Status + "</span>",
                WebhookStatus.Failed => @"<span class=""badge bg-danger"">" + this.Status + "</span>",
                WebhookStatus.Pending => @"<span class=""badge bg-primary"">" + this.Status + "</span>",
                WebhookStatus.CompletedWithError => @"<span class=""badge bg-danger"">" + this.Status + "</span>",
                _ => @"<span class=""badge bg-secondary"">" + this.Status + "</span>",
            };
        }
    } 
    public DateTime? DateTimeStarted { get; set; }
	public string LastModifiedDateFormatted { get { return this.LastModifiedDate!.ToString("MMM dd, yyyy HH:mm 'UTC'"); } }
    public DateTime? DateTimeEnded { get; set; }
}
