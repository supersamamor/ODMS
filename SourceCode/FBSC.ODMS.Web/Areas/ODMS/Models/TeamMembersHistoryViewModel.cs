using FBSC.ODMS.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace FBSC.ODMS.Web.Areas.ODMS.Models;

public record TeamMembersHistoryViewModel : BaseViewModel
{
    [Display(Name = "Name")]
    [Required]
    public string EmployeeId { get; init; } = "";
    [Display(Name = "Level")]
    [StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
    public string? MemberLevel { get; init; }
    [Display(Name = "Role")]
    [Required]
    [StringLength(100, ErrorMessage = "{0} length can't be more than {1}.")]
    public string Role { get; init; } = "";
    public string ProjectHistoryId { get; init; } = "";
    public string? ProjectName { get; init; } = "";
    public DateTime LastModifiedDate { get; set; }
    public ProjectHistoryViewModel? ProjectHistory { get; init; }
    public EmployeeViewModel? Employee { get; init; }

}
