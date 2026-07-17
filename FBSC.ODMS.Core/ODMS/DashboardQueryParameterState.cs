using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Core.ODMS;

public record DashboardQueryParameterState : BaseEntity
{
	public string DashboardQueryId { get; init; } = "";
	public string ParameterName { get; init; } = "";
	public string DataType { get; init; } = "";
	public string ControlType { get; init; } = "";
	public string? DefaultValue { get; init; }
	public bool IsRequired { get; init; }
	public string? LookupSourceQuery { get; init; }
	public int Sequence { get; init; }
	
	public DashboardQueryState? DashboardQuery { get; init; }
	
	
}
