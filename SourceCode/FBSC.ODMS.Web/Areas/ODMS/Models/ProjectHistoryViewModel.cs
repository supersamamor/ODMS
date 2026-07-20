using FBSC.Common.Web.Utility.Extensions;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using FBSC.Common.Web.Utility.Annotations;

namespace FBSC.ODMS.Web.Areas.ODMS.Models;

public record ProjectHistoryViewModel : BaseViewModel
{
    [Display(Name = "Name of the Project")]
    [Required]
    [StringLength(255, ErrorMessage = "{0} length can't be more than {1}.")]
    public string ProjectName { get; init; } = "";
    [Display(Name = "Assigned Business Unit")]
    [Required]
    public string BusinessUnitId { get; init; } = "";
    public string? ReferenceFieldBusinessUnitId { get; set; }
    [Display(Name = "Project Priority")]
    [Required]
    [StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
    public string Priority { get; init; } = "";
    [Display(Name = "Project Start Date")]
    [Required]
    public DateTime StartDate { get; init; } = DateTime.Now.Date;
    [Display(Name = "Target Completion Date")]
    [Required]
    public DateTime TargetEndDate { get; init; } = DateTime.Now.Date;
    [Display(Name = "Estimated Budget in Currency")]

    [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
    public decimal? EstimatedBudget { get; init; } = 0;
    [Display(Name = "Detailed Description of Project")]
    [StringLength(1000, ErrorMessage = "{0} length can't be more than {1}.")]
    public string? ProjectDescription { get; init; }
    [Display(Name = "Assigned Project Manager")]
    [Required]
    public string ProjectManagerId { get; init; } = "";
    public string? ReferenceFieldProjectManagerId { get; set; }
    [Display(Name = "RAG Status (Red, Amber, Green)")]
    [StringLength(20, ErrorMessage = "{0} length can't be more than {1}.")]
    public string? HealthStatus { get; init; }
    [Display(Name = "Current Phase (e.g. Development)")]
    [StringLength(100, ErrorMessage = "{0} length can't be more than {1}.")]
    public string? Phase { get; init; }
    [Display(Name = "Schedule Status (e.g. Delayed)")]
    [StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
    public string? ScheduleStatus { get; init; }
    [Display(Name = "Date of Last Review")]
    public DateTime? LastReviewDate { get; init; } = DateTime.Now.Date;
    [Display(Name = "Date of Last Update")]
    public DateTime? LastUpdatedDate { get; init; } = DateTime.Now.Date;

    public DateTime LastModifiedDate { get; set; }
    public string? ProjectNameReference { get; init; } 
    public ProjectViewModel? Project { get; init; }
    public BusinessUnitViewModel? BusinessUnit { get; init; }
    public EmployeeViewModel? Employee { get; init; }

    public IList<TeamMembersHistoryViewModel>? TeamMembersHistoryList { get; set; }

}
