using FBSC.Common.Data;
using FBSC.Common.Web.Utility.Helpers;
using FBSC.ODMS.Core.Constants;
using FBSC.ODMS.Infrastructure.Data;
using FBSC.ODMS.Infrastructure.Services.Dashboard;
using FBSC.ODMS.Web.Areas.ODMS.Models;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.DataSource;

/// <summary>
/// Upload -> preview (detected sheets/columns/types/sample rows, adjustable delimiter and
/// sheet inclusion) -> confirm flow for an UploadedFile-kind DataSource. The uploaded file is
/// saved to a temp path between the Preview and Confirm posts (a browser can't resubmit a
/// file input's bytes on the second POST) using the same FileUploadHelper save mechanism as
/// every other validated upload in this app - re-read and deleted once ingestion finishes.
/// </summary>
[Authorize(Policy = Permission.DataSource.Upload)]
public class UploadFileModel(UploadedFileIngestionService ingestionService, ApplicationContext context) : BasePageModel<UploadFileModel>
{
    [BindProperty]
    public string DataSourceId { get; set; } = "";
    [BindProperty]
    public IFormFile? UploadFormFile { get; set; }
    [BindProperty]
    public string? TempFilePath { get; set; }
    [BindProperty]
    public string? OriginalFileName { get; set; }
    [BindProperty]
    public string? CsvDelimiterOverride { get; set; }
    [BindProperty]
    public List<string> IncludedSheetNames { get; set; } = [];

    public string DataSourceName { get; set; } = "";
    public UploadFilePreviewViewModel? Preview { get; set; }

    public async Task<IActionResult> OnGet(string dataSourceId)
    {
        DataSourceId = dataSourceId;
        await LoadDataSourceNameAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostPreview()
    {
        await LoadDataSourceNameAsync();

        if (UploadFormFile is null || UploadFormFile.Length == 0)
        {
            NotyfService.Warning(Localizer["Choose a file to upload."]);
            return Page();
        }

        var permittedExtensions = (Configuration.GetValue<string>("UsersUpload:DataUploadPermitedExtensions") ?? ".csv,.xlsx,.xls").Split(',');
        var sizeLimit = Configuration.GetValue<long?>("DashboardEngine:UploadMaxFileSizeBytes") ?? 26_214_400;
        var uploadDir = Path.Combine(Configuration.GetValue<string>("UsersUpload:SecureUploadFilePath")!, "DataUpload", DataSourceId);

        var originalFileName = UploadFormFile.FileName;
        var saveResult = await FileUploadHelper.ProcessFormFile<UploadFileModel>(UploadFormFile, permittedExtensions, sizeLimit, uploadDir, cancellationToken: HttpContext.RequestAborted);

        string? savedPath = null;
        saveResult.Match(
            Succ: path => savedPath = path,
            Fail: errors => { AddModelError(errors, notifyError: true); });

        if (savedPath is null)
        {
            return Page();
        }

        try
        {
            var fileBytes = await System.IO.File.ReadAllBytesAsync(savedPath, HttpContext.RequestAborted);
            var parsed = ingestionService.Parse(fileBytes, originalFileName, string.IsNullOrWhiteSpace(CsvDelimiterOverride) ? null : CsvDelimiterOverride);

            var existingBatches = await context.DataUploadBatch
                .Include(b => b.DataUploadColumnList)
                .Where(b => b.DataSourceId == DataSourceId)
                .ToListAsync(HttpContext.RequestAborted);
            var existingColumnsBySheet = existingBatches.ToDictionary(
                b => b.SheetName,
                b => (b.DataUploadColumnList ?? []).Select(c => c.ColumnName).ToHashSet(StringComparer.OrdinalIgnoreCase));

            TempFilePath = savedPath;
            OriginalFileName = originalFileName;
            Preview = new UploadFilePreviewViewModel
            {
                OriginalFileName = originalFileName,
                Format = parsed.Format,
                Sheets = [.. parsed.Sheets.Select(sheet =>
                {
                    var newColumnNames = sheet.Columns.Select(c => c.SanitizedName).ToHashSet(StringComparer.OrdinalIgnoreCase);
                    var hadPreviousUpload = existingColumnsBySheet.TryGetValue(sheet.SheetName, out var previousColumnNames);
                    return new UploadFileSheetPreviewViewModel
                    {
                        SheetName = sheet.SheetName,
                        AlreadyExists = hadPreviousUpload,
                        IsTruncated = sheet.IsTruncated,
                        RowCount = sheet.Rows.Count,
                        RemovedColumns = hadPreviousUpload ? [.. previousColumnNames!.Except(newColumnNames, StringComparer.OrdinalIgnoreCase)] : [],
                        AddedColumns = hadPreviousUpload ? [.. newColumnNames.Except(previousColumnNames!, StringComparer.OrdinalIgnoreCase)] : [],
                        Columns = [.. sheet.Columns.Select(c => new UploadFileColumnPreviewViewModel
                        {
                            OriginalHeader = c.OriginalHeader,
                            SanitizedName = c.SanitizedName,
                            InferredSqlType = c.InferredSqlType,
                            SampleValue = c.SampleValue,
                        })],
                        SampleRows = [.. sheet.Rows.Take(10).Select(row =>
                        {
                            var dict = new Dictionary<string, object?>();
                            for (var i = 0; i < sheet.Columns.Count; i++)
                            {
                                dict[sheet.Columns[i].SanitizedName] = row[i] is DBNull ? null : row[i];
                            }
                            return dict;
                        })],
                    };
                })],
            };
            IncludedSheetNames = [.. Preview.Sheets.Select(s => s.SheetName)];
        }
        catch (NotSupportedException ex)
        {
            NotyfService.Error(ex.Message);
            TryDeleteFile(savedPath);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error parsing uploaded file for preview");
            NotyfService.Error(Localizer["Could not read that file - it may be corrupted or not a valid file of the selected type."]);
            TryDeleteFile(savedPath);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostConfirm()
    {
        if (string.IsNullOrEmpty(TempFilePath) || !System.IO.File.Exists(TempFilePath))
        {
            NotyfService.Error(Localizer["The uploaded file is no longer available - please upload it again."]);
            return RedirectToPage(new { dataSourceId = DataSourceId });
        }

        try
        {
            var fileBytes = await System.IO.File.ReadAllBytesAsync(TempFilePath, HttpContext.RequestAborted);
            var results = await ingestionService.IngestAsync(
                DataSourceId,
                fileBytes,
                OriginalFileName ?? "upload",
                User.Identity?.Name,
                IncludedSheetNames.Count > 0 ? IncludedSheetNames.ToHashSet() : null,
                string.IsNullOrWhiteSpace(CsvDelimiterOverride) ? null : CsvDelimiterOverride,
                HttpContext.RequestAborted);

            await AuditIngestionAsync(results);

            var succeeded = results.Count(r => r.Success);
            var failed = results.Count(r => !r.Success);
            if (failed == 0)
            {
                NotyfService.Success(Localizer["Uploaded and staged {0} sheet(s) successfully.", succeeded]);
            }
            else if (succeeded == 0)
            {
                NotyfService.Error(Localizer["All {0} sheet(s) failed to import - see Data Upload Batches for details.", failed]);
            }
            else
            {
                NotyfService.Warning(Localizer["{0} sheet(s) imported, {1} failed - see Data Upload Batches for details.", succeeded, failed]);
            }
        }
        finally
        {
            TryDeleteFile(TempFilePath);
        }

        return RedirectToPage("Details", new { id = DataSourceId });
    }

    private async Task AuditIngestionAsync(IReadOnlyList<IngestedSheetResult> results)
    {
        foreach (var result in results)
        {
            context.AuditLogs.Add(new Audit
            {
                UserId = User.Identity?.Name,
                TraceId = HttpContext.TraceIdentifier,
                Type = result.Success ? "FileUploadIngest" : "FileUploadIngestFailed",
                TableName = "DataUploadBatch",
                PrimaryKey = result.DataUploadBatchId,
                DateTime = DateTime.UtcNow,
                NewValues = System.Text.Json.JsonSerializer.Serialize(new
                {
                    SheetName = result.SheetName,
                    Status = result.Success ? "Success" : "Failed",
                    RowCount = result.RowCount,
                    ColumnCount = result.ColumnCount,
                    ErrorRemarks = result.ErrorRemarks,
                }),
            });
        }
        await context.SaveChangesAsync(HttpContext.RequestAborted);
    }

    private async Task LoadDataSourceNameAsync()
    {
        DataSourceName = await context.DataSource
            .Where(d => d.Id == DataSourceId)
            .Select(d => d.Name)
            .FirstOrDefaultAsync(HttpContext.RequestAborted) ?? "";
    }

    private static void TryDeleteFile(string path)
    {
        try
        {
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }
        catch
        {
            // Best-effort cleanup only - a leftover temp file isn't worth failing the request over.
        }
    }
}
