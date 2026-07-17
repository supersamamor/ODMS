using FBSC.Common.Web.Utility.Extensions;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using FBSC.Common.Web.Utility.Annotations;

namespace FBSC.ODMS.Web.Areas.ODMS.Models;

public record DataUploadBatchViewModel : BaseViewModel
{	
	[Display(Name = "Data Source")]
	[Required]
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string DataSourceId { get; init; } = "";
	public string?  ReferenceFieldDataSourceId { get; set; }
	[Display(Name = "Uploaded File Name")]
	[Required]
	[StringLength(260, ErrorMessage = "{0} length can't be more than {1}.")]
	public string FileName { get; init; } = "";
	[Display(Name = "CSV, XLSX, or JSON")]
	[Required]
	[StringLength(20, ErrorMessage = "{0} length can't be more than {1}.")]
	public string FileType { get; init; } = "";
	[Display(Name = "Uploaded By")]
	[StringLength(100, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? UploadedBy { get; init; }
	[Display(Name = "Generated Internal Staging Table Name")]
	[StringLength(150, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? StagingTableName { get; init; }
	[Display(Name = "Rows Imported")]
	public int? RowCount { get; init; } = 0;
	[Display(Name = "Columns Detected")]
	public int? ColumnCount { get; init; } = 0;
	[Display(Name = "Status")]
	[Required]
	[StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
	public string ImportStatus { get; init; } = "";
	[Display(Name = "Imported At")]
	public DateTime? ImportedAt { get; init; } = DateTime.Now;
	[Display(Name = "Import Error")]
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? ErrorRemarks { get; init; }
	
	public DateTime LastModifiedDate { get; set; }
	public DataSourceViewModel? DataSource { get; init; }
		
	public IList<DataUploadColumnViewModel>? DataUploadColumnList { get; set; }
	
}
