using FBSC.Common.Web.Utility.Extensions;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using FBSC.Common.Web.Utility.Annotations;

namespace FBSC.ODMS.Web.Areas.ODMS.Models;

public record EmployeeViewModel : BaseViewModel
{	
	[Display(Name = "Department")]
	
	public string? Department { get; init; }
	[Display(Name = "Email")]
	[Required]
	[StringLength(255, ErrorMessage = "{0} length can't be more than {1}.")]
	public string Email { get; init; } = "";
	[Display(Name = "Employee Code")]
	[Required]
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string EmployeeCode { get; init; } = "";
	[Display(Name = "Name")]
	[Required]
	[StringLength(255, ErrorMessage = "{0} length can't be more than {1}.")]
	public string Name { get; init; } = "";
	[Display(Name = "Position")]
	[Required]
	
	public string Position { get; init; } = "";
	[Display(Name = "User Id")]
	[StringLength(36, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? UserId { get; init; }
	
	public DateTime LastModifiedDate { get; set; }
		
	public IList<ProjectViewModel>? ProjectList { get; set; }
	public IList<ProjectHistoryViewModel>? ProjectHistoryList { get; set; }
	
}
