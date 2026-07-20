using FBSC.Common.Web.Utility.Extensions;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using FBSC.Common.Web.Utility.Annotations;

namespace FBSC.ODMS.Web.Areas.ODMS.Models;

public record DataSourceViewModel : BaseViewModel
{
	[Display(Name = "Connection Name")]
	[Required]
	[StringLength(150, ErrorMessage = "{0} length can't be more than {1}.")]
	public string Name { get; init; } = "";
	[Display(Name = "Data Source Type")]
	[Required]
	public string DataSourceType { get; init; } = FBSC.ODMS.Core.Constants.DataSourceTypes.SqlServer;
	[Display(Name = "Uploaded File")]
	public IFormFile? UploadedFileForm { get; set; }
	public string? UploadedFilePath { get; init; }
	[Display(Name = "Generated Table Name")]
	public string? GeneratedTableName { get; init; }
	[Display(Name = "Import Status")]
	public string? ImportStatus { get; init; }
	[Display(Name = "Import Error")]
	public string? ImportErrorMessage { get; init; }
	[Display(Name = "Last Imported")]
	public DateTime? LastImportedDate { get; init; }
	[Display(Name = "MS SQL Server Host Or Instance")]
	[StringLength(200, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? ServerAddress { get; init; }
	[Display(Name = "Target Database Name")]
	[StringLength(150, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? DatabaseName { get; init; }
	[Display(Name = "SQL, WindowsIntegrated, or AzureAD")]
	[StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? AuthenticationType { get; init; }
	[Display(Name = "Connection Username")]
	[StringLength(100, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? Username { get; init; }
	[Display(Name = "Password")]
	[DataType(DataType.Password)]
	[StringLength(100, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? Password { get; init; }
			[Display(Name = "Description")]
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? Description { get; init; }
	[Display(Name = "Is Active")]
	public bool IsActive { get; init; } = false;
	
	public DateTime LastModifiedDate { get; set; }
		
	
}
