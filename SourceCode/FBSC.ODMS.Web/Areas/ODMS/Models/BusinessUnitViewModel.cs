using FBSC.Common.Web.Utility.Extensions;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using FBSC.Common.Web.Utility.Annotations;

namespace FBSC.ODMS.Web.Areas.ODMS.Models;

public record BusinessUnitViewModel : BaseViewModel
{
	[Display(Name = "Code")]
	[Required]
	[StringLength(20, ErrorMessage = "{0} length can't be more than {1}.")]
	public string Code { get; init; } = "";
	[Display(Name = "Name")]
	[Required]
	[StringLength(150, ErrorMessage = "{0} length can't be more than {1}.")]
	public string Name { get; init; } = "";

	public DateTime LastModifiedDate { get; set; }
		
	public IList<ProjectViewModel>? ProjectList { get; set; }
	public IList<ProjectHistoryViewModel>? ProjectHistoryList { get; set; }
	
}
