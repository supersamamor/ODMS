using FBSC.Common.Identity.Abstractions;
using FBSC.ODMS.Application.DTOs;
using FBSC.ODMS.Infrastructure.Data;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace FBSC.ODMS.Application.Features.ODMS.ReportBuilder.Queries;

public record GetReportSetupAndResultByIdQuery(string Id) : IRequest<Option<ReportResultModel>>
{
    public IList<ReportQueryFilterModel> Filters { get; set; } = [];
}

public class GetReportBuilderByIdQueryHandler(ApplicationContext context, IConfiguration configuration, IdentityContext identityContext, IAuthenticatedUser authenticatedUser) : IRequestHandler<GetReportSetupAndResultByIdQuery, Option<ReportResultModel>>
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
		var connectionString = await Helpers.ReportDataHelper.ResolveConnectionStringAsync(
			  context, configuration, report?.DataSourceId, cancellationToken);

		var resultsAndLabels = await Helpers.ReportDataHelper.ConvertSQLQueryToJsonAsync(
			  authenticatedUser,
			  connectionString,
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
			DrillDownReportId = report.DrillDownReportId,
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
}