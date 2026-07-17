namespace FBSC.ODMS.Web.Areas.ODMS.Models;

public class UploadFileColumnPreviewViewModel
{
    public string OriginalHeader { get; set; } = "";
    public string SanitizedName { get; set; } = "";
    public string InferredSqlType { get; set; } = "";
    public string? SampleValue { get; set; }
}

public class UploadFileSheetPreviewViewModel
{
    public string SheetName { get; set; } = "";
    public bool AlreadyExists { get; set; }
    public bool IsTruncated { get; set; }
    public int RowCount { get; set; }
    public List<UploadFileColumnPreviewViewModel> Columns { get; set; } = [];
    public List<Dictionary<string, object?>> SampleRows { get; set; } = [];

    /// <summary>
    /// Columns the previous upload for this sheet had that this new upload doesn't - any
    /// DashboardWidget/DashboardQuery mapped to one of these will break once confirmed.
    /// </summary>
    public List<string> RemovedColumns { get; set; } = [];
    public List<string> AddedColumns { get; set; } = [];
}

public class UploadFilePreviewViewModel
{
    public string OriginalFileName { get; set; } = "";
    public string Format { get; set; } = "";
    public List<UploadFileSheetPreviewViewModel> Sheets { get; set; } = [];
}
