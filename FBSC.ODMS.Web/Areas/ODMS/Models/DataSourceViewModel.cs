using FBSC.Common.Web.Utility.Extensions;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using FBSC.Common.Web.Utility.Annotations;

namespace FBSC.ODMS.Web.Areas.ODMS.Models;

public record DataSourceViewModel : BaseViewModel
{	
	[Display(Name = "Friendly Label For This Connection")]
	[Required]
	[StringLength(150, ErrorMessage = "{0} length can't be more than {1}.")]
	public string Name { get; init; } = "";
	[Display(Name = "HRIS, Accounting, CRM, or Custom — free-text category for grouping/filtering")]
	[Required]
	[StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
	public string SystemType { get; init; } = "";
	[Display(Name = "ExternalDatabase (live connection) or UploadedFile (CSV/XLSX)")]
	[Required]
	[StringLength(30, ErrorMessage = "{0} length can't be more than {1}.")]
	public string ConnectionKind { get; init; } = "";
	[Display(Name = "ConnectionString or ServerCredentials — how to reach the server (ExternalDatabase only)")]
	[StringLength(30, ErrorMessage = "{0} length can't be more than {1}.")]
	public string ConnectionMode { get; init; } = "";
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
	[Display(Name = "Connection Password")]
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	[DataType(DataType.Password)]
	public string? PasswordEncrypted { get; init; }
	[Display(Name = "Full Connection String")]
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	[DataType(DataType.Password)]
	public string? ConnectionStringEncrypted { get; init; }
			[Display(Name = "Description")]
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? Description { get; init; }
	[Display(Name = "Is Active")]
	public bool IsActive { get; init; } = false;
	[Display(Name = "Last Connection Test At")]
	public DateTime? LastTestedAt { get; init; } = DateTime.Now;
	[Display(Name = "Last Connection Test Result")]
	[StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? LastTestStatus { get; init; }
	
	public DateTime LastModifiedDate { get; set; }
		
	public IList<DataSourceSchemaCacheViewModel>? DataSourceSchemaCacheList { get; set; }
	public IList<DataUploadBatchViewModel>? DataUploadBatchList { get; set; }
	public IList<DashboardQueryViewModel>? DashboardQueryList { get; set; }
	public IList<AiSqlGenerationRequestViewModel>? AiSqlGenerationRequestList { get; set; }
	
}
