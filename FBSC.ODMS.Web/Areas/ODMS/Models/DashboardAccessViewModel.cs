using FBSC.Common.Web.Utility.Extensions;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using FBSC.Common.Web.Utility.Annotations;

namespace FBSC.ODMS.Web.Areas.ODMS.Models;

public record DashboardAccessViewModel : BaseViewModel
{	
	[Display(Name = "Dashboard")]
	[Required]
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string DashboardId { get; init; } = "";
	public string?  ReferenceFieldDashboardId { get; set; }
	[Display(Name = "User Or Role")]
	[Required]
	[StringLength(20, ErrorMessage = "{0} length can't be more than {1}.")]
	public string GranteeType { get; init; } = "";
	[Display(Name = "User Or Role Identifier")]
	[Required]
	[StringLength(100, ErrorMessage = "{0} length can't be more than {1}.")]
	public string GranteeId { get; init; } = "";
	[Display(Name = "View, Edit, Or Owner")]
	[Required]
	[StringLength(20, ErrorMessage = "{0} length can't be more than {1}.")]
	public string AccessLevel { get; init; } = "";
	[Display(Name = "Granted At")]
	public DateTime? GrantedAt { get; init; } = DateTime.Now;
	
	public DateTime LastModifiedDate { get; set; }
	public DashboardViewModel? Dashboard { get; init; }
		
	
}
