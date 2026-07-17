using FBSC.Common.Utility.Helpers;
using FBSC.HTMLTemplate.Services;
using FBSC.HTMLTemplate.ViewModels;
using FBSC.ODMS.Application.DTOs;
using FBSC.ODMS.Application.Features.ODMS.ReportBuilder.Queries;
using FBSC.ODMS.Web.Areas.ODMS.Models;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBSC.ODMS.Web.Areas.ODMS.Pages.Report
{
    [Authorize(Policy = Permission.Report.View)]
    public class IndexModel(RotativaService rotativaService) : BasePageModel<IndexModel>
    {
        [BindProperty]
        public ReportResultViewModel Report { get; set; } = new ReportResultViewModel();
        [BindProperty]
        public IList<ReportQueryFilterViewModel> Filters { get; set; } = [];
        [BindProperty]
        public string ReportId { get; set; } = "";
        public async Task<IActionResult> OnGet(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ReportId = id;
            var reportResult = new ReportResultModel();
            _ = (await Mediatr.Send(new GetReportSetupAndResultByIdQuery(id))).Select(l => reportResult = l);
            Mapper.Map(reportResult, Report);
            if (Report.ReportOrChartType == Core.Constants.ReportChartType.PDF)
            {
                var rotativaDocument = (await rotativaService.GeneratePDFFromTemplateAsync($"{Report.ReportName}_{DateTime.Now:MMddyyyy}.pdf",
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
                                     }, PageContext, reportResult!.PdfReportData!));
                Report.PdfCompleteFilePath = rotativaDocument.CompleteFilePath;
            }
            else if (Report.ReportOrChartType == Core.Constants.ReportChartType.CustomHtml)
            {
                Report.CustomHtmlContent = HTMLTemplateHelper.ProcessTemplate(reportResult.HTMLTemplate, reportResult!.PdfReportData!);
            }
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            var reportResult = new ReportResultModel();
            var query = new GetReportSetupAndResultByIdQuery(ReportId);
            Mapper.Map(Filters, query.Filters);
            _ = (await Mediatr.Send(query)).Select(l => reportResult = l);
            Mapper.Map(reportResult, Report);
            if (Report.ReportOrChartType == Core.Constants.ReportChartType.PDF)
            {
                var rotativaDocument = (await rotativaService.GeneratePDFFromTemplateAsync($"{Report.ReportName}_{DateTime.Now:MMddyyyy}.pdf",
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
                    }, PageContext, reportResult!.PdfReportData!));
                Report.PdfCompleteFilePath = rotativaDocument.CompleteFilePath;
            }
            return Page();
        }
    }
}
