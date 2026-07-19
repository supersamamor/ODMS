
namespace FBSC.ApiHub.Dtos;

public record WebhookApiListDto : BaseWebhookDto
{
    public string ClientId { get; init; } = "";
    public string Name { get; init; } = "";
	public string GrantType { get; init; } = "";
	public bool WithinPrivateNetwork { get; init; }
	public string WithinPrivateNetworkFormatted { get { return this.WithinPrivateNetwork == true ? "Yes" : "No"; } }
	public string ClientSecret { get; init; } = "";
	public string? Scope { get; init; }
	public string BaseUrl { get; init; } = "";
	public string? AuthenticationUrl { get; init; }	

}
