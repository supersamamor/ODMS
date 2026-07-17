using FBSC.Common.Identity.Abstractions;
using FBSC.ODMS.Application.DTOs;
using FBSC.ODMS.Infrastructure.Data;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace FBSC.ODMS.Application.Features.ODMS.ReportBuilder.Queries;

public record GetDashboardReportsQuery() : IRequest<IList<ReportResultModel>>;
public class GetDashboardReportsQueryHandler(ApplicationContext context, IConfiguration configuration, IdentityContext identityContext, IAuthenticatedUser authenticatedUser) : IRequestHandler<GetDashboardReportsQuery, IList<ReportResultModel>>
{
    public async Task<IList<ReportResultModel>> Handle(GetDashboardReportsQuery request, CancellationToken cancellationToken = default)
    {
        var roleList = await (from ur in identityContext.UserRoles
                              join r in identityContext.Roles on ur.RoleId equals r.Id
                              where ur.UserId == authenticatedUser.UserId
                              select r.Name).Distinct().ToListAsync(cancellationToken);

        var reportList = await context.Report
            .Include(l => l.ReportQueryFilterList)
            .Where(e => e.DisplayOnDashboard == true).AsNoTracking()
            .Where(l => l.ReportRoleAssignmentList!.Any(ra => roleList.Contains(ra.RoleName)))
            .OrderBy(l => l.Sequence).ToListAsync(cancellationToken);

        IList<ReportResultModel> reportResult = [];

        foreach (var report in reportList)
        {
            var filters = new List<ReportQueryFilterModel>();

            if (report?.ReportQueryFilterList?.Count > 0)
            {
                foreach (var parameter in report.ReportQueryFilterList)
                {
                    // UPDATE 1: Map all required UI properties so the dashboard knows how to render the input (e.g., Dropdown, Date, etc.)
                    filters.Add(new ReportQueryFilterModel()
                    {
                        FieldName = parameter.FieldName!,
                        FieldDescription = parameter.FieldDescription!,
                        DataType = parameter.DataType!,
                        CustomDropdownValues = parameter.CustomDropdownValues,
                        DropdownTableKeyAndValue = parameter.DropdownTableKeyAndValue,
                        DropdownFilter = parameter.DropdownFilter,
                        Sequence = parameter.Sequence
                    });
                }
            }

            var resultsAndLabels = await Helpers.ReportDataHelper.ConvertSQLQueryToJsonAsync(authenticatedUser, configuration.GetConnectionString("ReportContext")!, report!, filters);

            reportResult.Add(new ReportResultModel()
            {
                ReportId = report?.Id,
                ReportName = report!.ReportName,
                Results = resultsAndLabels.Results,
                ColumnHeaders = resultsAndLabels.ColumnHeaders,
                ReportOrChartType = report!.ReportOrChartType,
                DisplayLegend = resultsAndLabels.DisplayLegend,
                PdfReportData = resultsAndLabels?.PdfReportData,
                HTMLTemplate = resultsAndLabels?.HTMLTemplate,
                HTMLFooterTemplate = resultsAndLabels?.HTMLFooterTemplate,
                Orientation = report.Orientation,
                PaperSize = report.PaperSize,
                MarginTop = report.MarginTop,
                MarginBottom = report.MarginBottom,
                MarginLeft = report.MarginLeft,
                MarginRight = report.MarginRight,
                Filters = filters,
                SpanWidth = report.SpanWidth
            });
        }
        return reportResult;
    }
}