using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Core.ODMS;

public record DataSourceSchemaCacheState : BaseEntity
{
	public string DataSourceId { get; init; } = "";
	public string SchemaName { get; init; } = "";
	public string TableName { get; init; } = "";
	public string ColumnName { get; init; } = "";
	public string? SqlDataType { get; init; }
	public int? OrdinalPosition { get; init; }
	public bool IsNullable { get; init; }
	public string? InferredSemanticType { get; init; }
	public DateTime? RefreshedAt { get; init; }

	public DataSourceState? DataSource { get; init; }
	
	
}
