using FBSC.Common.Identity.Abstractions;
using FBSC.ODMS.Application.DTOs;
using FBSC.ODMS.Application.Helpers;
using FBSC.ODMS.Core.Constants;
using FBSC.ODMS.Infrastructure.Data;
using FBSC.ODMS.Infrastructure.Services.Dashboard;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace FBSC.ODMS.Application.Features.ODMS.ReportBuilder.Queries;

public record GetDashboardReportsQuery() : IRequest<IList<ReportResultModel>>;
public class GetDashboardReportsQueryHandler(
    ApplicationContext context,
    IConfiguration configuration,
    IdentityContext identityContext,
    IAuthenticatedUser authenticatedUser,
    DashboardQueryExecutionService dashboardQueryExecutionService,
    ChartConfigurationService chartConfigurationService) : IRequestHandler<GetDashboardReportsQuery, IList<ReportResultModel>>
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

        foreach (var widgetResult in await GetDashboardWidgetResultsAsync(roleList, cancellationToken))
        {
            reportResult.Add(widgetResult);
        }

        return reportResult;
    }

    /// <summary>
    /// Dashboard/DashboardWidget rows (the Phase 2/4 dynamic-data-source engine) rendered
    /// through the exact same ReportResultModel contract as a legacy Report - see
    /// DashboardWidgetRenderHelper. Visible dashboards: owned by the current user, explicitly
    /// shared with them or one of their roles (DashboardAccessState), or public - mirrors
    /// DashboardAccessAuthorizationService's own view-access rule, applied set-based here to
    /// avoid an N+1 per-dashboard permission check.
    /// </summary>
    private async Task<IList<ReportResultModel>> GetDashboardWidgetResultsAsync(IReadOnlyList<string> roleList, CancellationToken cancellationToken)
    {
        var userId = authenticatedUser.UserId;
        var widgets = await context.DashboardWidget
            .Include(w => w.Dashboard)
            .Include(w => w.ReportType)
            .Include(w => w.DashboardQuery!).ThenInclude(q => q!.DashboardQueryParameterList)
            .Include(w => w.DashboardQuery!).ThenInclude(q => q!.DashboardQueryResultColumnList)
            .Include(w => w.DashboardQuery!).ThenInclude(q => q!.DataSource)
            .AsNoTracking()
            .Where(w => w.Dashboard != null && w.Dashboard!.IsActive
                && (w.Dashboard!.IsPublic
                    || w.Dashboard!.OwnerUserId == userId
                    || w.Dashboard!.DashboardAccessList!.Any(a =>
                        (a.GranteeType == DashboardGranteeType.User && a.GranteeId == userId)
                        || (a.GranteeType == DashboardGranteeType.Role && roleList.Contains(a.GranteeId)))))
            .Where(w => w.DashboardQuery != null && w.ReportType != null)
            .OrderBy(w => w.Sequence)
            .ToListAsync(cancellationToken);

        var results = new List<ReportResultModel>(widgets.Count);
        foreach (var widget in widgets)
        {
            var parameterValues = (widget.DashboardQuery!.DashboardQueryParameterList ?? [])
                .ToDictionary(p => p.ParameterName, p => p.DefaultValue);

            var result = await DashboardWidgetRenderHelper.RenderAsync(
                widget, dashboardQueryExecutionService, chartConfigurationService,
                parameterValues, userId, authenticatedUser.TraceId, cancellationToken);

            result.Filters = (widget.DashboardQuery.DashboardQueryParameterList ?? [])
                .OrderBy(p => p.Sequence)
                .Select(p => new ReportQueryFilterModel
                {
                    FieldName = p.ParameterName,
                    FieldDescription = p.ParameterName,
                    DataType = p.DataType,
                    Sequence = p.Sequence,
                }).ToList();

            results.Add(result);
        }
        return results;
    }
}
