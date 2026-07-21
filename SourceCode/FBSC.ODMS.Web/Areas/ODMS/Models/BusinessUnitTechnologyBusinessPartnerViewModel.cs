using FBSC.ODMS.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace FBSC.ODMS.Web.Areas.ODMS.Models;

/// <summary>
/// One Technology Business Partner (an Employee) assigned to a Business Unit.
/// A Business Unit can have many; rendered as an add/remove sub-collection grid
/// on the BusinessUnit Add/Edit pages.
/// </summary>
public record BusinessUnitTechnologyBusinessPartnerViewModel : BaseViewModel
{
    public string BusinessUnitId { get; init; } = "";

    [Display(Name = "Technology Business Partner")]
    [Required]
    public string EmployeeId { get; init; } = "";
}
