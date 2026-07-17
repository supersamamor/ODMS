using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Application.DTOs;

public record DataUploadColumnListDto : BaseDto
{
	public string DataUploadBatchId { get; init; } = "";
	public string ColumnName { get; init; } = "";
	public string? DetectedDataType { get; init; }
	public string? MappedSqlDataType { get; init; }
	public int? OrdinalPosition { get; init; }
	public string OrdinalPositionFormatted { get { return this.OrdinalPosition == null ? "" : this.OrdinalPosition!.Value.ToString("#,##0"); } }
	public string? SampleValue { get; init; }
	
	
}
