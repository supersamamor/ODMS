using System.Text;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Services.Dashboard;
using FBSC.ODMS.Tests.TestHelpers;

namespace FBSC.ODMS.Tests;

/// <summary>
/// Proves an uploaded-file sheet's columns flow through the exact same field-mapping
/// pipeline (ChartConfigurationService) built for live-database widgets in the original
/// Phase 4 - no per-ConnectionKind branching anywhere in the mapping/rendering layer, per the
/// constraint that a DashboardWidget must not need to know or care where its data came from.
/// </summary>
public class UploadedFileWidgetRenderingTests
{
    [Fact]
    public void UploadedSheetColumns_ResolveThroughChartConfigurationService_LikeAnyOtherSource()
    {
        var csv = "Department,Headcount\nEngineering,42\nSales,18\nSupport,9\n";
        var ingestionService = new UploadedFileIngestionService(TestApplicationContextFactory.Create(), TestApplicationContextFactory.CreateConfiguration());

        var parsed = ingestionService.Parse(Encoding.UTF8.GetBytes(csv), "headcount.csv");
        var sheet = Assert.Single(parsed.Sheets);

        var resultColumns = sheet.Columns
            .Select((c, i) => new DashboardQueryResultColumnInfo(c.SanitizedName, i, c.InferredSqlType))
            .ToList();

        var widget = new DashboardWidgetState();
        var reportType = new ReportTypeState { Code = "Bar", Name = "Bar Chart", ChartRenderer = "bar", RequiresXAxis = true, RequiresYAxis = true };
        var chartConfigurationService = new ChartConfigurationService();

        // No DataSourceSchemaCacheState catalog is supplied here - exactly the "column isn't
        // cataloged yet" path a brand-new upload hits before schema discovery/caching runs,
        // so this also proves the name/type fallback heuristic works for uploaded columns.
        var mapping = chartConfigurationService.ResolveMapping(widget, reportType, resultColumns, catalogColumns: []);

        Assert.Equal("Department", mapping.XAxisColumnName);
        Assert.Equal(["Headcount"], mapping.YAxisColumnNames);
    }
}
