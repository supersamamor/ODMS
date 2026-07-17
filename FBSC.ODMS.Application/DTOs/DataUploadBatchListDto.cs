using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Application.DTOs;

public record DataUploadBatchListDto : BaseDto
{
	public string DataSourceId { get; init; } = "";
	public string FileName { get; init; } = "";
	public string SheetName { get; init; } = "";
	public string FileType { get; init; } = "";
	public string? UploadedBy { get; init; }
	public string? StagingTableName { get; init; }
	public int? RowCount { get; init; }
	public string RowCountFormatted { get { return this.RowCount == null ? "" : this.RowCount!.Value.ToString("#,##0"); } }
	public int? ColumnCount { get; init; }
	public string ColumnCountFormatted { get { return this.ColumnCount == null ? "" : this.ColumnCount!.Value.ToString("#,##0"); } }
	public string ImportStatus { get; init; } = "";
	public DateTime? ImportedAt { get; init; }
	public string ImportedAtFormatted { get { return this.ImportedAt == null ? "" : this.ImportedAt!.Value.ToString("MMM dd, yyyy HH:mm"); } }
	public string? ErrorRemarks { get; init; }
	
	
}
