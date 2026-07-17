namespace FBSC.ODMS.ExcelProcessor.Models
{
    public class ExcelImportResultModel<TableObject>
    {
        public bool IsSuccess { get; set; }
        public List<ExcelRecord> FailedRecords { get; set; } = [];
        public List<TableObject> SuccessRecords { get; set; } = [];
    }
    public class ExcelRecord
    {
        public int RowNumber { get; set; }
        public Dictionary<string, object?> Data { get; set; } = [];
    }
}
