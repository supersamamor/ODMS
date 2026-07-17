using FBSC.Common.Web.Utility.Extensions;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using FBSC.Common.Web.Utility.Annotations;

namespace FBSC.ODMS.Web.Areas.ODMS.Models;

public record DashboardQueryViewModel : BaseViewModel
{	
	[Display(Name = "Data Source")]
	[Required]
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string DataSourceId { get; init; } = "";
	public string?  ReferenceFieldDataSourceId { get; set; }
	[Display(Name = "Name")]
	[Required]
	[StringLength(150, ErrorMessage = "{0} length can't be more than {1}.")]
	public string Name { get; init; } = "";
	[Display(Name = "Description")]
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? Description { get; init; }
	[Display(Name = "MS SQL Select Statement")]
	[Required]
	
	public string SqlQueryText { get; init; } = "";
	[Display(Name = "Hash Of Normalized Sql Text, Used For Cache Keying And Dedupe")]
	[StringLength(64, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? QueryHash { get; init; }
	[Display(Name = "Has One Or More DashboardQueryParameter Rows")]
	public bool IsParameterized { get; init; } = false;
	[Display(Name = "Originated From An AiSqlGenerationRequest")]
	public bool GeneratedByAI { get; init; } = false;
	[Display(Name = "Valid, Invalid, Or NotYetValidated")]
	[StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? ValidationStatus { get; init; }
	[Display(Name = "Last Validated At")]
	public DateTime? LastValidatedAt { get; init; } = DateTime.Now;
	[Display(Name = "Last Execution Duration In Milliseconds")]
	public int? LastExecutionDurationMs { get; init; } = 0;
	[Display(Name = "Last Executed At")]
	public DateTime? LastExecutedAt { get; init; } = DateTime.Now;
	[Display(Name = "Last Execution Error")]
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? LastExecutionErrorRemarks { get; init; }
	[Display(Name = "Is Active")]
	public bool IsActive { get; init; } = false;
	
	public DateTime LastModifiedDate { get; set; }
	public DataSourceViewModel? DataSource { get; init; }
		
	public IList<DashboardQueryParameterViewModel>? DashboardQueryParameterList { get; set; }
	public IList<DashboardQueryResultColumnViewModel>? DashboardQueryResultColumnList { get; set; }
	public IList<DashboardQueryResultCacheViewModel>? DashboardQueryResultCacheList { get; set; }
	public IList<DashboardWidgetViewModel>? DashboardWidgetList { get; set; }
	public IList<AiSqlGenerationRequestViewModel>? AiSqlGenerationRequestList { get; set; }
	public IList<DashboardRefreshJobViewModel>? DashboardRefreshJobList { get; set; }
	
}
