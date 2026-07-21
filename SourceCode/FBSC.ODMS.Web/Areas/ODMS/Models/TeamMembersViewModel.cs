using FBSC.ODMS.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace FBSC.ODMS.Web.Areas.ODMS.Models;

public record TeamMembersViewModel : BaseViewModel
{
    // Optional: the "(Unknown)" default in the Team Members grid maps to null.
    [Display(Name = "Name")]
    public string? EmployeeId { get; init; }
    [Display(Name = "Level")]
    [Required]
    [StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
    public string? MemberLevel { get; init; }
    [Display(Name = "Role")]
    [Required]
    [StringLength(100, ErrorMessage = "{0} length can't be more than {1}.")]
    public string Role { get; init; } = "";
    public string ProjectId { get; init; } = "";
    public string? ProjectName { get; init; } = "";
    public DateTime LastModifiedDate { get; set; }
    public ProjectViewModel? Project { get; init; }
    public EmployeeViewModel? Employee { get; init; }

}
