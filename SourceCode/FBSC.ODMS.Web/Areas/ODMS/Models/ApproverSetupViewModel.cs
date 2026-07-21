using FBSC.ODMS.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace FBSC.ODMS.Web.Areas.ODMS.Models;

public record ApproverSetupViewModel : BaseViewModel
{
    [Display(Name = "Table")]
    [Required]
    [StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
    public string TableName { get; init; } = "";
    // Optional routing discriminator for the Project module: blank = all towers.
    [Display(Name = "Delivery Tower")]
    [StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
    public string? DeliveryCategory { get; init; }
    [Display(Name = "Approval Type")]
    [Required]
    [StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
    public string ApprovalType { get; init; } = "";

    public DateTime LastModifiedDate { get; set; }
    [Display(Name = "Email Subject")]
    [Required]
    [StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
    public string EmailSubject { get; init; } = "";
    [Display(Name = "Email Body (Html)")]
    [Required]
    public string EmailBody { get; init; } = "";
    public IList<ApproverAssignmentViewModel>? ApproverAssignmentList { get; set; }
    public string SelectedApprovers
    {
        get
        {
            if (this.ApproverAssignmentList != null)
            {
                return string.Join(",", this.ApproverAssignmentList.Select(l => l.ApproverUserId).ToList());
            }
            else
            {
                return "";
            }
        }
    }
    public string ApprovalSetupType { get; init; } = "";
}
