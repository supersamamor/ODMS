using FBSC.Common.Core.Base.Models;
namespace FBSC.ApiHub.Models;
public record WebhookEventAssignmentState : BaseEntity
{
    public string WebhookApiId { get; init; } = "";
    public string EventName { get; init; } = "";
    public string Route { get; init; } = "";
    public string Method { get; init; } = "";
    public bool Active { get; init; } = true;
    public WebhookApiState? WebhookApi { get; init; }
    public IList<WebhookLogsState>? WebhookLogsList { get; set; }
  
}

