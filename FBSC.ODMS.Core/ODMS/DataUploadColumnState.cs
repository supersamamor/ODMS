using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Core.ODMS;

public record DataUploadColumnState : BaseEntity
{
	public string DataUploadBatchId { get; init; } = "";
	public string ColumnName { get; init; } = "";
	public string? DetectedDataType { get; init; }
	public string? MappedSqlDataType { get; init; }
	public int? OrdinalPosition { get; init; }
	public string? SampleValue { get; init; }
	
	public DataUploadBatchState? DataUploadBatch { get; init; }
	
	
}
