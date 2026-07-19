using System.ComponentModel.DataAnnotations;
namespace FBSC.ApiHub.ViewModels;
public record WebhookEventAssignmentViewModel : BaseWebhookViewModel
{
    [Display(Name = "Webhook Api")]
    [Required]
    public string WebhookApiId { get; init; } = "";
    public string? ReferenceFieldWebhookApiId { get; set; }
    [Display(Name = "Event Name")]
    [Required]
    [StringLength(255, ErrorMessage = "{0} length can't be more than {1}.")]
    public string EventName { get; init; } = "";
    [Display(Name = "Route")]
    [Required]
    [StringLength(255, ErrorMessage = "{0} length can't be more than {1}.")]
    public string Route { get; init; } = "";
    [Display(Name = "Method")]
    [Required]
    [StringLength(15, ErrorMessage = "{0} length can't be more than {1}.")]
    public string Method { get; init; } = "";
    public DateTime LastModifiedDate { get; set; }
    public WebhookApiViewModel? WebhookApi { get; init; }
    public IList<WebhookLogsViewModel>? WebhookLogsList { get; set; }
    public bool NewRecord { get; init; } = false;
    public bool Active { get; init; } = true;
}
