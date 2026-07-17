using FBSC.Common.Web.Utility.Extensions;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using FBSC.Common.Web.Utility.Annotations;

namespace FBSC.ODMS.Web.Areas.ODMS.Models;

public record DashboardQueryResultCacheViewModel : BaseViewModel
{	
	[Display(Name = "Dashboard Query")]
	[Required]
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string DashboardQueryId { get; init; } = "";
	public string?  ReferenceFieldDashboardQueryId { get; set; }
	[Display(Name = "Hash Of The Resolved Parameter Values For This Cache Entry")]
	[Required]
	[StringLength(64, ErrorMessage = "{0} length can't be more than {1}.")]
	public string ParameterSetHash { get; init; } = "";
		[Display(Name = "Rows Cached")]
	public int? RowCount { get; init; } = 0;
	[Display(Name = "Approximate Payload Size In Bytes")]
	public int? CacheSizeBytes { get; init; } = 0;
	[Display(Name = "Cached At")]
	public DateTime? CachedAt { get; init; } = DateTime.Now;
	[Display(Name = "Expires At")]
	public DateTime? ExpiresAt { get; init; } = DateTime.Now;
	[Display(Name = "Flagged Stale By A Failed Or Overdue Refresh")]
	public bool IsStale { get; init; } = false;
	
	public DateTime LastModifiedDate { get; set; }
	public DashboardQueryViewModel? DashboardQuery { get; init; }
		
	
}
