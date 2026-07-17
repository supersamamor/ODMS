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
using static LanguageExt.Prelude;

namespace FBSC.ODMS.Application.Features.ODMS.ReportBuilder.Queries;

public record GetReportSetupAndResultByIdQuery(string Id) : IRequest<Option<ReportResultModel>>
{
    public IList<ReportQueryFilterModel> Filters { get; set; } = [];
}

public class GetReportBuilderByIdQueryHandler(
    ApplicationContext context,
    IConfiguration configuration,
    IdentityContext identityContext,
    IAuthenticatedUser authenticatedUser,
    DashboardQueryExecutionService dashboardQueryExecutionService,
    ChartConfigurationService chartConfigurationService) : IRequestHandler<GetReportSetupAndResultByIdQuery, Option<ReportResultModel>>
{
    public async Task<Option<ReportResultModel>> Handle(GetReportSetupAndResultByIdQuery request, CancellationToken cancellationToken = default)
	{
		var roleList = await (from ur in identityContext.UserRoles
							  join r in identityContext.Roles on ur.RoleId equals r.Id
							  where ur.UserId == authenticatedUser.UserId
							  select r.Name).Distinct().ToListAsync(cancellationToken);
		var report = await context.Report
			.Include(l => l.ReportQueryFilterList)
			.Where(e => e.Id == request.Id)
			.Where(l => l.ReportRoleAssignmentList!.Any(ra => roleList.Contains(ra.RoleName)))
			.AsNoTracking().FirstOrDefaultAsync(cancellationToken);

		if (report is null)
		{
			return await TryGetDashboardWidgetResultAsync(request, roleList, cancellationToken);
		}

		if (request.Filters == null || request.Filters.Count == 0)
		{
			request.Filters = [];
			if (report?.ReportQueryFilterList?.Count > 0)
			{
				foreach (var parameter in report.ReportQueryFilterList)
				{
					request.Filters.Add(new ReportQueryFilterModel()
					{
						FieldName = parameter.FieldName!,
						FieldDescription = parameter.FieldDescription!,
						DataType = parameter.DataType!,
						CustomDropdownValues = parameter.CustomDropdownValues!,
						DropdownTableKeyAndValue = parameter.DropdownTableKeyAndValue!,
						DropdownFilter = parameter.DropdownFilter!,
						Sequence = parameter.Sequence!,
					});
				}
			}
		}
		var resultsAndLabels = await Helpers.ReportDataHelper.ConvertSQLQueryToJsonAsync(
			  authenticatedUser,
			  configuration.GetConnectionString("ReportContext")!,
			  report!,
			  request.Filters);
		return new ReportResultModel()
		{
			ReportId = request.Id,
			ReportName = report!.ReportName,
			Results = resultsAndLabels.Results,
			ColumnHeaders = resultsAndLabels.ColumnHeaders,
			ReportOrChartType = report!.ReportOrChartType,
			Filters = request.Filters,
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
		};
	}

    /// <summary>
    /// Same by-id lookup, but for a Phase 2/4 DashboardWidget instead of a legacy Report -
    /// used both by Report/Index's standalone view and DashboardRenderer's AJAX
    /// RefreshDashboardWidget handler. Returns the exact same ReportResultModel shape either
    /// way (see DashboardWidgetRenderHelper), so neither caller needs to know which engine
    /// actually produced the result.
    /// </summary>
    private async Task<Option<ReportResultModel>> TryGetDashboardWidgetResultAsync(GetReportSetupAndResultByIdQuery request, IReadOnlyList<string> roleList, CancellationToken cancellationToken)
    {
        var userId = authenticatedUser.UserId;
        var widget = await context.DashboardWidget
            .Include(w => w.Dashboard)
            .Include(w => w.ReportType)
            .Include(w => w.DashboardQuery!).ThenInclude(q => q!.DashboardQueryParameterList)
            .Include(w => w.DashboardQuery!).ThenInclude(q => q!.DashboardQueryResultColumnList)
            .Include(w => w.DashboardQuery!).ThenInclude(q => q!.DataSource)
            .AsNoTracking()
            .Where(w => w.Id == request.Id && w.Dashboard != null && w.DashboardQuery != null && w.ReportType != null)
            .Where(w => w.Dashboard!.IsPublic
                || w.Dashboard!.OwnerUserId == userId
                || w.Dashboard!.DashboardAccessList!.Any(a =>
                    (a.GranteeType == DashboardGranteeType.User && a.GranteeId == userId)
                    || (a.GranteeType == DashboardGranteeType.Role && roleList.Contains(a.GranteeId))))
            .FirstOrDefaultAsync(cancellationToken);

        if (widget is null)
        {
            return None;
        }

        var declaredParameters = widget.DashboardQuery!.DashboardQueryParameterList ?? [];
        var suppliedByFieldName = (request.Filters ?? []).ToDictionary(f => f.FieldName, f => f.FieldValue, StringComparer.OrdinalIgnoreCase);
        var parameterValues = declaredParameters.ToDictionary(
            p => p.ParameterName,
            p => suppliedByFieldName.TryGetValue(p.ParameterName, out var supplied) && !string.IsNullOrEmpty(supplied) ? supplied : p.DefaultValue);

        var result = await DashboardWidgetRenderHelper.RenderAsync(
            widget, dashboardQueryExecutionService, chartConfigurationService,
            parameterValues, userId, authenticatedUser.TraceId, cancellationToken);
        result.Filters = request.Filters ?? [];
        return result;
    }
}
