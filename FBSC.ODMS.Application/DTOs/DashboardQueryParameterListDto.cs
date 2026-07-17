using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Application.DTOs;

public record DashboardQueryParameterListDto : BaseDto
{
	public string DashboardQueryId { get; init; } = "";
	public string ParameterName { get; init; } = "";
	public string DataType { get; init; } = "";
	public string ControlType { get; init; } = "";
	public string? DefaultValue { get; init; }
	public bool IsRequired { get; init; }
	public string IsRequiredFormatted { get { return this.IsRequired == true ? "Yes" : "No"; } }
		public int Sequence { get; init; }
	public string SequenceFormatted { get { return this.Sequence.ToString("#,##0"); } }
	
	
}
