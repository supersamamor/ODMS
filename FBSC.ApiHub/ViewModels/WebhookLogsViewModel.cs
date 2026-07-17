using System.ComponentModel.DataAnnotations;
namespace FBSC.ApiHub.ViewModels;
public record WebhookLogsViewModel : BaseWebhookViewModel
{
    [Display(Name = "Webhook Event Assignment")]
    [Required]
    public string WebhookEventAssignmentId { get; init; } = "";
    public string? WebhookApiName { get; set; }
    public string? ReferenceFieldWebhookEventAssignmentId { get; set; }
    [Display(Name = "Payload")]
    public string? Payload { get; init; }
    public DateTime LastModifiedDate { get; set; }
    public WebhookEventAssignmentViewModel? WebhookEventAssignment { get; init; }
    [Display(Name = "Date/Time Started")]
    public DateTime? DateTimeStarted { get; init; } = DateTime.Now;
    [Display(Name = "Date/Time Ended")]
    public DateTime? DateTimeEnded { get; init; } = DateTime.Now;
    [Display(Name = "Status")]
    [StringLength(10, ErrorMessage = "{0} length can't be more than {1}.")]
    public string? Status { get; init; }
    public string? DataId { get; init; }
    public string? Error { get; private set; }
    public string? Response { get; private set; }
    public bool ProcessResponse { get; init; }
    public string? ProcessResponseStatus { get; set; }
    public string? ProcessResponseStatusError { get; private set; }
    public string ParametarizedRoute { get; set; } = string.Empty;
    public string? LockedByInstance { get; set; }
}
