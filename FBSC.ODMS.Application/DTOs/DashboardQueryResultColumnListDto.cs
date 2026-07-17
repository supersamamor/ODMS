using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Application.DTOs;

public record DashboardQueryResultColumnListDto : BaseDto
{
	public string DashboardQueryId { get; init; } = "";
	public string ColumnName { get; init; } = "";
	public int OrdinalPosition { get; init; }
	public string OrdinalPositionFormatted { get { return this.OrdinalPosition.ToString("#,##0"); } }
	public string? SqlDataType { get; init; }
	public string InferredRole { get; init; } = "";
	public bool IsAggregatable { get; init; }
	public string IsAggregatableFormatted { get { return this.IsAggregatable == true ? "Yes" : "No"; } }
	public string? DefaultAggregation { get; init; }
	public string? FormatString { get; init; }
	public int Sequence { get; init; }
	public string SequenceFormatted { get { return this.Sequence.ToString("#,##0"); } }
	
	
}
