using FBSC.Common.Web.Utility.Extensions;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using FBSC.Common.Web.Utility.Annotations;

namespace FBSC.ODMS.Web.Areas.ODMS.Models;

public record AccomplishmentViewModel : BaseViewModel
{	
	[Display(Name = "StatusReport")]
	
	public string? StatusReportId { get; init; }
	public string?  ReferenceFieldStatusReportId { get; set; }
	[Display(Name = "Description")]
	[StringLength(255, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? Description { get; init; }
	
	public DateTime LastModifiedDate { get; set; }
	public StatusReportViewModel? StatusReport { get; init; }
		
	
}
