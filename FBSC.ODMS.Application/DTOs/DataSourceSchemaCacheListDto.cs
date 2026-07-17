using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Application.DTOs;

public record DataSourceSchemaCacheListDto : BaseDto
{
	public string DataSourceId { get; init; } = "";
	public string SchemaName { get; init; } = "";
	public string TableName { get; init; } = "";
	public string ColumnName { get; init; } = "";
	public string? SqlDataType { get; init; }
	public int? OrdinalPosition { get; init; }
	public string OrdinalPositionFormatted { get { return this.OrdinalPosition == null ? "" : this.OrdinalPosition!.Value.ToString("#,##0"); } }
	public bool IsNullable { get; init; }
	public string IsNullableFormatted { get { return this.IsNullable == true ? "Yes" : "No"; } }
	public DateTime? RefreshedAt { get; init; }
	public string RefreshedAtFormatted { get { return this.RefreshedAt == null ? "" : this.RefreshedAt!.Value.ToString("MMM dd, yyyy HH:mm"); } }
	
	
}
