using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FBSC.ODMS.Web.Areas.Admin.Models;

public record ApplicationViewModel
{
    [Display(Name = "Client Id")]
    public string ClientId { get; set; } = "";
    [Display(Name = "Client Secret")]
    public string ClientSecret { get; set; } = "";
    [Required]
    [Display(Name = "Name")]
    public string DisplayName { get; set; } = "";
    [Required]
    [Display(Name = "Redirect URI")]
    public string RedirectUri { get; set; } = "";
    [Required]
    [Display(Name = "Scopes")]
    public string Scopes { get; set; } = "";
    [Required]
    [Display(Name = "Entity")]
    public string Entity { get; set; } = "";
	[Required]
    [Display(Name = "Redirect URI")]
    public string PostLogoutRedirectUris { get; set; } = "";
    [JsonIgnore]
    public SelectList Entities { get; set; } = new(new List<SelectListItem>());
	public string CompleteScopes { get; set; } = "";
	public string? WebhookHmacSecret { get; set; }
}
