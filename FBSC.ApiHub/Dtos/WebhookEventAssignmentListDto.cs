
namespace FBSC.ApiHub.Dtos;

public record WebhookEventAssignmentListDto : BaseWebhookDto
{
	public string WebhookApiId { get; init; } = "";
	public string EventName { get; init; } = "";
	public string Route { get; init; } = "";
	public string Method { get; init; } = "";
	
	
}
