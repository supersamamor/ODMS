using FBSC.Common.Web.Utility.Extensions;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using FBSC.Common.Web.Utility.Annotations;

namespace FBSC.ODMS.Web.Areas.ODMS.Models;

public record AiSqlPromptTemplateViewModel : BaseViewModel
{	
	[Display(Name = "HRIS, Accounting, CRM, Or Generic")]
	[Required]
	[StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
	public string SystemType { get; init; } = "";
	[Display(Name = "Instructions Steering Natural-Language-To-Sql Generation")]
	[Required]
	
	public string PromptTemplate { get; init; } = "";
	[Display(Name = "Evaluation Order When Multiple Templates Apply")]
	public int? Sequence { get; init; } = 0;
	[Display(Name = "Is Active")]
	public bool IsActive { get; init; } = false;
	
	public DateTime LastModifiedDate { get; set; }
		
	
}
