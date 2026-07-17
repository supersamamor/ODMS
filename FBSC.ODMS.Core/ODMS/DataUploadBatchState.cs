using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Core.ODMS;

public record DataUploadBatchState : BaseEntity
{
	public string DataSourceId { get; init; } = "";
	public string FileName { get; init; } = "";
	/// <summary>
	/// Worksheet this batch represents. A CSV upload gets exactly one implicit batch with
	/// SheetName == FileName's base name; a multi-sheet XLSX upload gets one
	/// DataUploadBatchState per included sheet (all sharing DataSourceId + FileName), so each
	/// sheet maps 1:1 to a staging table / DataSourceSchemaCacheState set, exactly like a
	/// table/view does for a live database connection.
	/// </summary>
	public string SheetName { get; init; } = "";
	public string FileType { get; init; } = "";
	public string? UploadedBy { get; init; }
	public string? StagingTableName { get; init; }
	public int? RowCount { get; init; }
	public int? ColumnCount { get; init; }
	public string ImportStatus { get; init; } = "";
	public DateTime? ImportedAt { get; init; }
	public string? ErrorRemarks { get; init; }
	
	public DataSourceState? DataSource { get; init; }
	
	public IList<DataUploadColumnState>? DataUploadColumnList { get; set; }
	
}
