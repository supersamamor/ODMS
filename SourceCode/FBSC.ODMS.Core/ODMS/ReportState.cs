using FBSC.Common.Core.Base.Models;
namespace FBSC.ODMS.Core.ODMS;
public record ReportState : BaseEntity
{
    public string ReportName { get; set; } = "";
    public string? ReportDescription { get; init; } = "";
    public string QueryType { get; init; } = "";
    public string ReportOrChartType { get; init; } = "";
    public bool IsDistinct { get; init; }
    public string? QueryString { get; set; }
    public bool DisplayOnDashboard { get; init; }
    public bool DisplayOnReportModule { get; init; }
    public int Sequence { get; init; }
    public string? HtmlTemplate { get; set; }
	public string? HTMLFooterTemplate { get; init; } = "";
    public string Orientation { get; init; } = "";
    public string PaperSize { get; init; } = "";
    public int MarginTop { get; init; }
    public int MarginBottom { get; init; }
    public int MarginLeft { get; init; }
    public int MarginRight { get; init; }
    public int SpanWidth { get; init; }
    public string? DrillDownReportId { get; set; }
    public string? DataSourceId { get; init; }
    public IList<ReportQueryFilterState>? ReportQueryFilterList { get; set; }
    public IList<ReportRoleAssignmentState>? ReportRoleAssignmentList { get; set; }
    public IList<ReportAIIntegrationState>? ReportAIIntegrationList { get; set; }
    public void SetQueryType(string queryString)
    {
        QueryString = queryString;
    }
    public void IncrementReportName()
    {
        //if there is ({number}) at the end of the report name, increment it to ({number + 1}) otherwise add (1)
        if (ReportName.Contains('('))
        {
            var lastBracketIndex = ReportName.LastIndexOf('(');
            var lastBracket = ReportName[lastBracketIndex..];
            if (int.TryParse(lastBracket.Replace("(", "").Replace(")", ""), out int number))
            {
                ReportName = string.Concat(ReportName.AsSpan(0, lastBracketIndex), $"({number + 1})");
            }
            else
            {
                ReportName += "(1)";
            }
        }
        else
        {
            ReportName += "(1)";
        }     
    }
}
public record ReportQueryFilterState : BaseEntity
{
    public string ReportId { get; init; } = "";
    public string FieldName { get; init; } = "";
    public string? FieldDescription { get; init; }
    public string DataType { get; init; } = "";
    public string? CustomDropdownValues { get; init; }
    public string? DropdownTableKeyAndValue { get; init; }
    public string? DropdownFilter { get; init; }
    public int Sequence { get; init; }
    public ReportState? Report { get; init; }
}
public record ReportRoleAssignmentState : BaseEntity
{
    public new string Id { get; set; } = Guid.NewGuid().ToString();
    public string ReportId { get; init; } = "";
    public string RoleName { get; init; } = "";
    public ReportState? Report { get; init; }
}

public record ReportAIIntegrationState : BaseEntity
{
    public new string Id { get; set; } = Guid.NewGuid().ToString();
    public string ReportId { get; init; } = "";
    public string Input { get; init; } = "";
    public string Output { get; init; } = "";
    public ReportState? Report { get; init; }
}

