using FBSC.ODMS.Core.Constants;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Services.Dashboard;

namespace FBSC.ODMS.Tests;

public class ChartConfigurationServiceTests
{
    private readonly ChartConfigurationService _service = new();

    private static ReportTypeState BarReportType(bool requiresSeries = false) => new()
    {
        Code = "Bar",
        Name = "Bar Chart",
        ChartRenderer = "bar",
        RequiresXAxis = true,
        RequiresYAxis = true,
        RequiresSeriesGrouping = requiresSeries,
    };

    [Fact]
    public void ResolveMapping_UsesExplicitWidgetMappingWhenPresent()
    {
        var widget = new DashboardWidgetState { XAxisColumnName = "Region", YAxisColumnsJson = "[\"Revenue\"]" };
        var columns = new[]
        {
            new DashboardQueryResultColumnInfo("Region", 0, "nvarchar"),
            new DashboardQueryResultColumnInfo("Revenue", 1, "decimal"),
        };

        var mapping = _service.ResolveMapping(widget, BarReportType(), columns, catalogColumns: []);

        Assert.Equal("Region", mapping.XAxisColumnName);
        Assert.Equal(["Revenue"], mapping.YAxisColumnNames);
    }

    [Fact]
    public void ResolveMapping_AutoSuggestsFromCatalogedInferredRole_WhenWidgetHasNoMapping()
    {
        var widget = new DashboardWidgetState();
        var columns = new[]
        {
            new DashboardQueryResultColumnInfo("DepartmentName", 0, "nvarchar"),
            new DashboardQueryResultColumnInfo("TotalHeadcount", 1, "int"),
        };
        var catalog = new[]
        {
            new DashboardQueryResultColumnState { ColumnName = "DepartmentName", InferredRole = SemanticType.Dimension },
            new DashboardQueryResultColumnState { ColumnName = "TotalHeadcount", InferredRole = SemanticType.Measure },
        };

        var mapping = _service.ResolveMapping(widget, BarReportType(), columns, catalog);

        Assert.Equal("DepartmentName", mapping.XAxisColumnName);
        Assert.Equal(["TotalHeadcount"], mapping.YAxisColumnNames);
    }

    [Fact]
    public void ResolveMapping_FallsBackToNameTypeHeuristic_WhenColumnIsNotCataloged()
    {
        // No DataSourceSchemaCache entry exists for this column (e.g. an ad-hoc computed
        // column in the SELECT list) - must still infer a reasonable role from name + SQL type.
        var widget = new DashboardWidgetState();
        var columns = new[]
        {
            new DashboardQueryResultColumnInfo("OrderDate", 0, "datetime"),
            new DashboardQueryResultColumnInfo("TotalAmount", 1, "decimal"),
        };

        var mapping = _service.ResolveMapping(widget, BarReportType(), columns, catalogColumns: []);

        Assert.Equal("OrderDate", mapping.XAxisColumnName);
        Assert.Equal(["TotalAmount"], mapping.YAxisColumnNames);
    }

    [Fact]
    public void BuildConfig_FailsWithGuidance_WhenRequiredRoleCannotBeResolved()
    {
        var widget = new DashboardWidgetState();
        // Only measure-shaped columns - a Bar chart needs a label/X-axis too.
        var columns = new[]
        {
            new DashboardQueryResultColumnInfo("Amount1", 0, "decimal"),
            new DashboardQueryResultColumnInfo("Amount2", 1, "decimal"),
        };

        var result = _service.BuildConfig(widget, BarReportType(), columns, rows: [], catalogColumns: []);

        Assert.False(result.Success);
        Assert.Contains("X-axis", result.ErrorMessage);
    }

    [Fact]
    public void BuildConfig_TableRendererReturnsRawColumnsAndRows_RegardlessOfSchema()
    {
        var widget = new DashboardWidgetState();
        var tableType = new ReportTypeState { Code = "Table", Name = "Table", ChartRenderer = "table" };
        var columns = new[] { new DashboardQueryResultColumnInfo("Anything", 0, "nvarchar") };
        var rows = new List<Dictionary<string, object?>> { new() { ["Anything"] = "value" } };

        var result = _service.BuildConfig(widget, tableType, columns, rows, catalogColumns: []);

        Assert.True(result.Success);
    }

    /// <summary>
    /// The core schema-agnosticism claim: the SAME ReportType + the SAME ChartConfigurationService
    /// produce a correctly-shaped Bar chart config for two completely differently-shaped result
    /// sets (an HRIS-style schema and an unrelated accounting-style schema) with zero
    /// per-schema code - only the InferredRole/heuristic-driven mapping differs.
    /// </summary>
    [Theory]
    [MemberData(nameof(DifferentlyShapedSchemas))]
    public void BuildConfig_RendersSuccessfully_AcrossDifferentlyShapedSchemas(
        DashboardQueryResultColumnInfo[] columns,
        List<Dictionary<string, object?>> rows,
        string expectedXAxisColumn,
        string expectedYAxisColumn)
    {
        var widget = new DashboardWidgetState();

        var result = _service.BuildConfig(widget, BarReportType(), columns, rows, catalogColumns: []);

        Assert.True(result.Success, result.ErrorMessage);
        var mapping = _service.ResolveMapping(widget, BarReportType(), columns, catalogColumns: []);
        Assert.Equal(expectedXAxisColumn, mapping.XAxisColumnName);
        Assert.Contains(expectedYAxisColumn, mapping.YAxisColumnNames);
    }

    public static IEnumerable<object[]> DifferentlyShapedSchemas()
    {
        // Schema A: HRIS headcount-by-department
        yield return
        [
            new[]
            {
                new DashboardQueryResultColumnInfo("DepartmentName", 0, "nvarchar"),
                new DashboardQueryResultColumnInfo("HeadcountTotal", 1, "int"),
            },
            new List<Dictionary<string, object?>>
            {
                new() { ["DepartmentName"] = "Engineering", ["HeadcountTotal"] = 42 },
                new() { ["DepartmentName"] = "Sales", ["HeadcountTotal"] = 18 },
            },
            "DepartmentName",
            "HeadcountTotal",
        ];

        // Schema B: unrelated accounting revenue-by-region - no column names in common with A
        yield return
        [
            new[]
            {
                new DashboardQueryResultColumnInfo("Region", 0, "nvarchar"),
                new DashboardQueryResultColumnInfo("NetRevenueAmount", 1, "decimal"),
            },
            new List<Dictionary<string, object?>>
            {
                new() { ["Region"] = "APAC", ["NetRevenueAmount"] = 125000.50m },
                new() { ["Region"] = "EMEA", ["NetRevenueAmount"] = 98250.00m },
            },
            "Region",
            "NetRevenueAmount",
        ];
    }
}
