using FBSC.ODMS.Application.DTOs;

namespace FBSC.ODMS.Web.Areas.ODMS.Models
{
    public class ReportResultViewModel
	{
		public string? ReportId { get; set; }
		public string? ReportName { get; set; }
		public string? Results { get; set; }
		public string? ColumnHeaders { get; set; }
		public string ReportOrChartType { get; set; } = "";
		public bool DisplayLegend { get; set; }
		public string PdfCompleteFilePath { get; set; } = "";
        public string? CustomHtmlContent { get; set; } = "";
        public int SpanWidth { get; init; }
        public bool IsPercentage => ReportName != null && ReportName.Contains('%');
		public IList<ReportQueryFilterViewModel> Filters { get; set; } = [];
		public string? DomId
		{
			get { return this.ReportId?.Replace("-",""); }
		}
	}
}
