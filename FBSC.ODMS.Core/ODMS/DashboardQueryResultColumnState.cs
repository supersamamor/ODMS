using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Core.ODMS;

public record DashboardQueryResultColumnState : BaseEntity
{
	public string DashboardQueryId { get; init; } = "";
	public string ColumnName { get; init; } = "";
	public int OrdinalPosition { get; init; }
	public string? SqlDataType { get; init; }
	public string InferredRole { get; init; } = "";
	public bool IsAggregatable { get; init; }
	public string? DefaultAggregation { get; init; }
	public string? FormatString { get; init; }
	public int Sequence { get; init; }
	
	public DashboardQueryState? DashboardQuery { get; init; }
	
	
}
