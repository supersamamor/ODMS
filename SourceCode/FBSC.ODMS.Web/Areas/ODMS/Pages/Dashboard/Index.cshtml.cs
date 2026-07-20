using FBSC.Common.Utility.Helpers;
using FBSC.HTMLTemplate.ViewModels;
using FBSC.ODMS.Application.DTOs;
using FBSC.ODMS.Application.Features.ODMS.Report.Commands;
using FBSC.ODMS.Application.Features.ODMS.ReportBuilder.Queries;
using FBSC.ODMS.Web.Areas.ODMS.Models;
using FBSC.ODMS.Web.Models;
using FBSC.ODMS.Web.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using OpenTelemetry;
using static FBSC.ODMS.Web.Permission;
namespace FBSC.ODMS.Web.Areas.ODMS.Pages.Dashboard
{
    public class IndexModel(AIDataAnalyticsServices aiDataAnalyticsServices, FBSC.HTMLTemplate.Services.RotativaService rotativaService) : BasePageModel<IndexModel>
    {
        [BindProperty]
        public IList<ReportResultViewModel> ReportList { get; set; } = [];
        [BindProperty]
        public string SelectedReportId { get; set; } = "";
        public async Task<IActionResult> OnGet()
        {
            var reportListDto = await Mediatr.Send(new GetDashboardReportsQuery());
            foreach (var report in reportListDto)
            {
                var reportViewModel = Mapper.Map<ReportResultViewModel>(report);
                if (report.ReportOrChartType == Core.Constants.ReportChartType.PDF)
                {
                    var rotativaDocument = (await rotativaService.GeneratePDFFromTemplateAsync($"{report.ReportName}_{DateTime.Now:MMddyyyy}.pdf",
                                    WebConstants.SecureUploadFilePath, "Dashboard", new HTMLTemplateViewModel()
                                    {
                                        HTMLTemplate = report.HTMLTemplate,
										HTMLFooterTemplate = report.HTMLFooterTemplate,
                                        Orientation = report.Orientation,
                                        PaperSize = report.PaperSize,
                                        MarginTop = report.MarginTop,
                                        MarginBottom = report.MarginBottom,
                                        MarginLeft = report.MarginLeft,
                                        MarginRight = report.MarginRight,
                                    }, PageContext, report.PdfReportData!));
                    reportViewModel.PdfCompleteFilePath = rotativaDocument.CompleteFilePath;
                }
                else if (report.ReportOrChartType == Core.Constants.ReportChartType.CustomHtml)
                {
                    reportViewModel.CustomHtmlContent = HTMLTemplateHelper.ProcessTemplate(report.HTMLTemplate, report!.PdfReportData!);
                }
                ReportList.Add(reportViewModel);
            }
            return Page();
        }
        public async Task<PartialViewResult> OnPostDataAnalytics()
        {
            ModelState.Clear();
            var query = new GetReportSetupAndResultByIdQuery(SelectedReportId);
            var report = new ReportResultModel();
            // Execute the MediatR pipeline which routes through ReportDataHelper
            _ = (await Mediatr.Send(query)).Select(l => report = l);
            var chatGPTResult = await aiDataAnalyticsServices.AIDrivenAnalysis(report!.ReportName!, report.Results!, report.ColumnHeaders!, token: new CancellationToken());
            await Mediatr.Send(new AddReportAnalyticsCommand()
            {
                ReportId = report.ReportId!,
                Input = $"Report Data : {report.Results} / Report Column Headers : {report.ColumnHeaders}",
                Output = chatGPTResult == null ? "" : chatGPTResult!,
            });
            JObject chatGPTJson = JObject.Parse(chatGPTResult!);
            return Partial("_DataAnalytics", chatGPTJson);
        }
        // Add this inside your Dashboard IndexModel class (Pages.Dashboard.IndexModel)
        public async Task<JsonResult> OnPostRefreshDashboardWidgetAsync(
            [FromQuery] string reportId,
            [FromForm(Name = "Filters")] IList<ReportQueryFilterViewModel> filters,
            [FromForm(Name = "GlobalProject")] string? globalProject,
            [FromForm(Name = "GlobalWeekNumber")] int? globalWeekNumber,
            [FromForm(Name = "GlobalBusinessUnitId")] string? globalBusinessUnitId)
        {
            var query = new GetReportSetupAndResultByIdQuery(reportId);

            // Map the incoming UI view models to the DTO expected by MediatR
            Mapper.Map(filters, query.Filters);

            // Append the global filter bar's values as @GlobalProject /
            // @GlobalWeekNumber (+precomputed @GlobalWeekStartDate/@GlobalWeekEndDate)
            // / @GlobalBusinessUnitId parameters for the report SQL to opt into.
            Application.Helpers.ReportDataHelper.AppendGlobalFilters(
                query.Filters, globalProject, globalWeekNumber, globalBusinessUnitId);

            var reportResult = new ReportResultModel();

            // Execute the MediatR pipeline which routes through ReportDataHelper
            _ = (await Mediatr.Send(query)).Select(l => reportResult = l);

            string generatedPdfUrl = "";
            string customHtmlContent = "";
            // GENERATE PDF / HTML IF APPLICABLE
            if (reportResult.ReportOrChartType == Core.Constants.ReportChartType.PDF)
            {
                var rotativaDocument = await rotativaService.GeneratePDFFromTemplateAsync(
                    $"{reportResult.ReportName}_{DateTime.Now:MMddyyyyHHmmss}.pdf", // Added HHmmss to avoid browser caching
                    WebConstants.SecureUploadFilePath, "Dashboard", new HTMLTemplateViewModel()
                    {
                        HTMLTemplate = reportResult.HTMLTemplate,
                        HTMLFooterTemplate = reportResult.HTMLFooterTemplate,
                        Orientation = reportResult.Orientation,
                        PaperSize = reportResult.PaperSize,
                        MarginTop = reportResult.MarginTop,
                        MarginBottom = reportResult.MarginBottom,
                        MarginLeft = reportResult.MarginLeft,
                        MarginRight = reportResult.MarginRight,
                    }, PageContext, reportResult.PdfReportData!);

                // Generate the exact URL needed for the iframe src
                generatedPdfUrl = Url.Page("/ViewAttachment/Index", new
                {
                    area = "ODMS",
                    handler = "Preview",
                    completeFilePath = rotativaDocument.CompleteFilePath,
                    timestamp = DateTime.Now.Ticks
                })!;
            }
            else if (reportResult.ReportOrChartType == Core.Constants.ReportChartType.CustomHtml)
            {
                customHtmlContent = HTMLTemplateHelper.ProcessTemplate(reportResult.HTMLTemplate, reportResult!.PdfReportData!);             
            }
            return new JsonResult(new
            {
                success = true,
                results = reportResult.Results,
                columnHeaders = reportResult.ColumnHeaders,
                reportOrChartType = reportResult.ReportOrChartType,
                displayLegend = reportResult.DisplayLegend,
                pdfUrl = generatedPdfUrl,
                customHtmlContent,
            });
        }
    }
}
