using FBSC.Common.Web.Utility.Extensions;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using FBSC.Common.Web.Utility.Annotations;

namespace FBSC.ODMS.Web.Areas.ODMS.Models;

public record AiSqlGenerationRequestViewModel : BaseViewModel
{	
	[Display(Name = "Data Source")]
	[Required]
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string DataSourceId { get; init; } = "";
	public string?  ReferenceFieldDataSourceId { get; set; }
	[Display(Name = "User`s Plain-Language Question")]
	[Required]
	
	public string NaturalLanguagePrompt { get; init; } = "";
	[Display(Name = "AI-Generated Sql")]
	
	public string? GeneratedSqlQueryText { get; init; }
	[Display(Name = "Dashboard Query Created On Acceptance")]
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? DashboardQueryId { get; init; }
	public string?  ReferenceFieldDashboardQueryId { get; set; }
	[Display(Name = "Model Confidence 0-100")]
	public int? ConfidenceScore { get; init; } = 0;
	[Display(Name = "Status")]
	[StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? ValidationStatus { get; init; }
	[Display(Name = "Generation Or Validation Error")]
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? ErrorRemarks { get; init; }
	[Display(Name = "Requested By")]
	[StringLength(100, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? RequestedBy { get; init; }
	[Display(Name = "Generated At")]
	public DateTime? GeneratedAt { get; init; } = DateTime.Now;
	
	public DateTime LastModifiedDate { get; set; }
	public DataSourceViewModel? DataSource { get; init; }
	public DashboardQueryViewModel? DashboardQuery { get; init; }
		
	
}
