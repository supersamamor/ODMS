
namespace FBSC.ODMS.Application.DTOs
{
    public class ReportResultModel
    {
        public string? ReportId { get; set; }
		public string? ReportName { get; set; }
		public string? Results { get; set; }
		public string? ColumnHeaders { get; set; }
		public string ReportOrChartType { get; set; } = "";
		public bool DisplayLegend { get; set; }
		public string? DrillDownReportId { get; set; }
		public object? PdfReportData { get; set; }
		public string? HTMLTemplate { get; set; } = "";
		public string? HTMLFooterTemplate { get; init; } = "";
		public string Orientation { get; init; } = string.Empty;
		public string PaperSize { get; init; } = string.Empty;
		public int MarginTop { get; init; } 
		public int MarginBottom { get; init; }
		public int MarginLeft { get; init; } 
		public int MarginRight { get; init; }
        public int SpanWidth { get; init; }
        public IList<ReportQueryFilterModel> Filters { get; set; } = [];
    }
    public class ReportQueryFilterModel
    {
        public string FieldName { get; set; } = "";
		public string FieldDescription { get; set; } = "";
		public string DataType { get; set; } = "";
		public string FieldValue { get; set; } = "";
		public string ReportId { get; init; } = "";
		public string? CustomDropdownValues { get; init; }
		public string? DropdownTableKeyAndValue { get; init; }
		public string? DropdownFilter { get; init; }
		public int Sequence { get; init; }
    }
}
