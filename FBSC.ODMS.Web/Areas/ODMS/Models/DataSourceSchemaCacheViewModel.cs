using FBSC.Common.Web.Utility.Extensions;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using FBSC.Common.Web.Utility.Annotations;

namespace FBSC.ODMS.Web.Areas.ODMS.Models;

public record DataSourceSchemaCacheViewModel : BaseViewModel
{	
	[Display(Name = "Data Source")]
	[Required]
	[StringLength(450, ErrorMessage = "{0} length can't be more than {1}.")]
	public string DataSourceId { get; init; } = "";
	public string?  ReferenceFieldDataSourceId { get; set; }
	[Display(Name = "Sql Schema Name")]
	[Required]
	[StringLength(100, ErrorMessage = "{0} length can't be more than {1}.")]
	public string SchemaName { get; init; } = "";
	[Display(Name = "Sql Table Name")]
	[Required]
	[StringLength(150, ErrorMessage = "{0} length can't be more than {1}.")]
	public string TableName { get; init; } = "";
	[Display(Name = "Sql Column Name")]
	[Required]
	[StringLength(150, ErrorMessage = "{0} length can't be more than {1}.")]
	public string ColumnName { get; init; } = "";
	[Display(Name = "Native Sql Data Type")]
	[StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
	public string? SqlDataType { get; init; }
	[Display(Name = "Column Order In Source Table")]
	public int? OrdinalPosition { get; init; } = 0;
	[Display(Name = "Is Nullable")]
	public bool IsNullable { get; init; } = false;
	[Display(Name = "Last Discovered At")]
	public DateTime? RefreshedAt { get; init; } = DateTime.Now;
	
	public DateTime LastModifiedDate { get; set; }
	public DataSourceViewModel? DataSource { get; init; }
		
	
}
