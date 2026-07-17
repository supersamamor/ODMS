using FBSC.Common.Web.Utility.Extensions;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using FBSC.Common.Web.Utility.Annotations;

namespace FBSC.ODMS.Web.Areas.ODMS.Models;

public record DataUploadColumnViewModel : BaseViewModel
{	
	[Display(Name = "Data Upload Batch")]
	[Required]
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string DataUploadBatchId { get; init; } = "";
	public string?  ReferenceFieldDataUploadBatchId { get; set; }
	[Display(Name = "Detected Column Name")]
	[Required]
	[StringLength(150, ErrorMessage = "{0} length can't be more than {1}.")]
	public string ColumnName { get; init; } = "";
	[Display(Name = "Auto-Detected Data Type")]
	[StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? DetectedDataType { get; init; }
	[Display(Name = "Sql Data Type Used In Staging Table")]
	[StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? MappedSqlDataType { get; init; }
	[Display(Name = "Column Order In File")]
	public int? OrdinalPosition { get; init; } = 0;
	[Display(Name = "First Non-Blank Sample Value")]
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? SampleValue { get; init; }
	
	public DateTime LastModifiedDate { get; set; }
	public DataUploadBatchViewModel? DataUploadBatch { get; init; }
		
	
}
