using FBSC.ODMS.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace FBSC.ODMS.Web.Areas.ODMS.Models;

public record ProjectHistoryViewModel : BaseViewModel
{
    public string ProjectId { get; init; } = "";
    [Display(Name = "Project ID")]
    [Required]
    [StringLength(12, ErrorMessage = "{0} length can't be more than {1}.")]
    public string ProjectCode { get; init; } = "";
    [Display(Name = "Project Name")]
    [Required]
    [StringLength(255, ErrorMessage = "{0} length can't be more than {1}.")]
    public string ProjectName { get; init; } = "";
    [Display(Name = "Delivery Tower")]
    [Required]
    [StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
    public string DeliveryTower { get; init; } = "";
    [Display(Name = "Demand Type")]
    [Required]
    [StringLength(100, ErrorMessage = "{0} length can't be more than {1}.")]
    public string DemandType { get; init; } = "";
    [Display(Name = "Business Unit")]
    [Required]
    public string BusinessUnitId { get; init; } = "";
    public string? ReferenceFieldBusinessUnitId { get; set; }
    [Display(Name = "Technology Business Partner")]
    public string? TechnologyBusinessPartnerId { get; init; }
    [Display(Name = "Priority")]
    [Required]
    [StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
    public string Priority { get; init; } = "";
    [Display(Name = "Start Date")]
    [Required]
    public DateTime BaselineStartDate { get; init; } = DateTime.Now.Date;
    [Display(Name = "Target End Date")]
    [Required]
    public DateTime BaselineEndDate { get; init; } = DateTime.Now.Date;
    [Display(Name = "Approved Budget")]
    [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
    public decimal? ApprovedBudget { get; init; }
    [Display(Name = "Project Description")]
    [StringLength(1000, ErrorMessage = "{0} length can't be more than {1}.")]
    public string? ProjectDescription { get; init; }
    [Display(Name = "Project Manager")]
    [Required]
    public string ProjectManagerId { get; init; } = "";
    public string? ReferenceFieldProjectManagerId { get; set; }
    [Display(Name = "Deputy Project Manager")]
    public string? DeputyProjectManagerId { get; init; }
    [Display(Name = "Status")]
    [StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
    public string? ActiveStatus { get; init; }
    [Display(Name = "Statement of Work")]
    public string? SOWFileName { get; init; }
    [Display(Name = "No SOW yet")]
    public bool NoSOW { get; init; }

    public DateTime LastModifiedDate { get; set; }
    public string? ProjectNameReference { get; init; }
    public ProjectViewModel? Project { get; init; }
    public BusinessUnitViewModel? BusinessUnit { get; init; }
    public EmployeeViewModel? Employee { get; init; }

    public IList<TeamMembersHistoryViewModel>? TeamMembersHistoryList { get; set; }

}
