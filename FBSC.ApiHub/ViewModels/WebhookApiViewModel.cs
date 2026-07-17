using FBSC.Common.Web.Utility.Annotations;
using System.ComponentModel.DataAnnotations;
namespace FBSC.ApiHub.ViewModels;

public record WebhookApiViewModel : BaseWebhookViewModel
{
    [Display(Name = "Client Id")]
    [Required]
    [StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
    public string ClientId { get; init; } = "";
    [Display(Name = "Name")]
    [Required]
    [StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
    public string Name { get; init; } = "";
    [Display(Name = "Grant Type")]
    [Required]
    [StringLength(20, ErrorMessage = "{0} length can't be more than {1}.")]
    public string GrantType { get; init; } = "";
    [Display(Name = "Within Private Network")]
    public bool WithinPrivateNetwork { get; init; } = false;
    [Display(Name = "Client Secret")]
    [RequiredIf(nameof(GrantType), Models.GrantType.ClientCredentials)]
    [StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
    public string ClientSecret { get; init; } = "";
    [Display(Name = "Scope")]
    [RequiredIf(nameof(GrantType), Models.GrantType.ClientCredentials)]
    public string? Scope { get; init; }
    [Display(Name = "Base Url")]
    [Required]
    [StringLength(255, ErrorMessage = "{0} length can't be more than {1}.")]
    public string BaseUrl { get; init; } = "";
    [Display(Name = "Authentication Url")]
    [StringLength(255, ErrorMessage = "{0} length can't be more than {1}.")]
    [RequiredIf(nameof(GrantType), Models.GrantType.ClientCredentials)]
    public string? AuthenticationUrl { get; init; }
    public DateTime LastModifiedDate { get; set; }
    public IList<WebhookEventAssignmentViewModel>? WebhookEventAssignmentList { get; set; }
    public bool IsEdit { get; init; }
    [Display(Name = "HMAC")]
    [StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
    public string? HMAC { get; init; } 
    public bool Active { get; init; } = true;
    [Display(Name = "Additional Configuration")]
    public string? AdditionalConfigurationJson { get; init; }
}
