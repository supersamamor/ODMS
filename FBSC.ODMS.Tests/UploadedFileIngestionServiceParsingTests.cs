using System.Text;
using FBSC.ODMS.Core.Constants;
using FBSC.ODMS.Infrastructure.Services.Dashboard;
using FBSC.ODMS.Tests.TestHelpers;
using OfficeOpenXml;

namespace FBSC.ODMS.Tests;

public class UploadedFileIngestionServiceParsingTests
{
    private static UploadedFileIngestionService CreateService() =>
        new(TestApplicationContextFactory.Create(), TestApplicationContextFactory.CreateConfiguration());

    [Fact]
    public void DetectFormat_MapsExtensionsToFormatConstants()
    {
        Assert.Equal(UploadedFileFormat.Csv, UploadedFileIngestionService.DetectFormat("employees.csv"));
        Assert.Equal(UploadedFileFormat.Xlsx, UploadedFileIngestionService.DetectFormat("employees.xlsx"));
        Assert.Equal(UploadedFileFormat.Xls, UploadedFileIngestionService.DetectFormat("employees.xls"));
    }

    [Fact]
    public void DetectFormat_RejectsUnsupportedExtension()
    {
        Assert.Throws<NotSupportedException>(() => UploadedFileIngestionService.DetectFormat("employees.txt"));
    }

    [Fact]
    public void Parse_Csv_SanitizesHeadersAndInfersTypes()
    {
        var csv = "Employee Name,Hire Date,Salary\nJane Doe,2026-01-15,85000\nJohn Smith,2026-02-01,72000.50\n";
        var bytes = Encoding.UTF8.GetBytes(csv);

        var parsed = CreateService().Parse(bytes, "employees.csv");

        var sheet = Assert.Single(parsed.Sheets);
        Assert.Equal(2, sheet.Rows.Count);
        Assert.Equal("Employee_Name", sheet.Columns[0].SanitizedName);
        Assert.Equal("Hire_Date", sheet.Columns[1].SanitizedName);
        Assert.Equal("Salary", sheet.Columns[2].SanitizedName);
        Assert.Equal("datetime2", sheet.Columns[1].InferredSqlType);
        Assert.Equal("decimal(18,4)", sheet.Columns[2].InferredSqlType);
    }

    [Fact]
    public void Parse_Csv_RespectsDelimiterOverride()
    {
        var csv = "Name;Amount\nWidget;10.5\n";
        var bytes = Encoding.UTF8.GetBytes(csv);

        var parsed = CreateService().Parse(bytes, "data.csv", csvDelimiterOverride: ";");

        var sheet = Assert.Single(parsed.Sheets);
        Assert.Equal(2, sheet.Columns.Count);
        Assert.Equal("Widget", sheet.Rows[0][0]);
    }

    /// <summary>
    /// Proves multi-sheet Excel support: each worksheet becomes its own ParsedSheet with its
    /// own independently-sanitized/typed columns - exactly the "one sheet maps 1:1 to one
    /// queryable table" contract UploadedFileIngestionService.IngestAsync relies on when it
    /// registers each sheet as a separate DataSourceSchemaCacheState set.
    /// </summary>
    [Fact]
    public void Parse_MultiSheetXlsx_ProducesOneParsedSheetPerWorksheet()
    {
        var bytes = BuildTwoSheetWorkbook();

        var parsed = CreateService().Parse(bytes, "workbook.xlsx");

        Assert.Equal(2, parsed.Sheets.Count);

        var employeesSheet = parsed.Sheets.Single(s => s.SheetName == "Employees");
        Assert.Equal(["Name", "Department"], employeesSheet.Columns.Select(c => c.SanitizedName));
        Assert.Equal(2, employeesSheet.Rows.Count);

        var revenueSheet = parsed.Sheets.Single(s => s.SheetName == "Revenue");
        Assert.Equal(["Region", "Amount"], revenueSheet.Columns.Select(c => c.SanitizedName));
        Assert.Equal("decimal(18,4)", revenueSheet.Columns[1].InferredSqlType);
    }

    private static byte[] BuildTwoSheetWorkbook()
    {
        using var package = new ExcelPackage();

        var employees = package.Workbook.Worksheets.Add("Employees");
        employees.Cells[1, 1].Value = "Name";
        employees.Cells[1, 2].Value = "Department";
        employees.Cells[2, 1].Value = "Alice";
        employees.Cells[2, 2].Value = "Engineering";
        employees.Cells[3, 1].Value = "Bob";
        employees.Cells[3, 2].Value = "Sales";

        var revenue = package.Workbook.Worksheets.Add("Revenue");
        revenue.Cells[1, 1].Value = "Region";
        revenue.Cells[1, 2].Value = "Amount";
        revenue.Cells[2, 1].Value = "APAC";
        revenue.Cells[2, 2].Value = "125000.50";

        return package.GetAsByteArray();
    }
}
