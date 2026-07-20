using FBSC.Common.Web.Utility.Extensions;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using FBSC.Common.Web.Utility.Annotations;

namespace FBSC.ODMS.Web.Areas.ODMS.Models;

public record TeamMembersHistoryViewModel : BaseViewModel
{
    [Display(Name = "Full Name of the Member")]
    [Required]
    [StringLength(255, ErrorMessage = "{0} length can't be more than {1}.")]
    public string MemberName { get; init; } = "";
    [Display(Name = "Role of the Member in Project")]
    [Required]
    [StringLength(100, ErrorMessage = "{0} length can't be more than {1}.")]
    public string Role { get; init; } = "";
    public string? ProjectName { get; init; } = "";
    public DateTime LastModifiedDate { get; set; }
    public ProjectHistoryViewModel? ProjectHistory { get; init; }


}
