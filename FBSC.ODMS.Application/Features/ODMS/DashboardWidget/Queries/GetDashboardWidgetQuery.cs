using FBSC.Common.Core.Queries;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using MediatR;
using FBSC.Common.Utility.Extensions;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Application.DTOs;
using LanguageExt;

namespace FBSC.ODMS.Application.Features.ODMS.DashboardWidget.Queries;

public record GetDashboardWidgetQuery : BaseQuery, IRequest<PagedListResponse<DashboardWidgetListDto>>;

public class GetDashboardWidgetQueryHandler(ApplicationContext context) : BaseQueryHandler<ApplicationContext, DashboardWidgetListDto, GetDashboardWidgetQuery>(context), IRequestHandler<GetDashboardWidgetQuery, PagedListResponse<DashboardWidgetListDto>>
{
	public override Task<PagedListResponse<DashboardWidgetListDto>> Handle(GetDashboardWidgetQuery request, CancellationToken cancellationToken = default)
	{
		return Task.FromResult(Context.Set<DashboardWidgetState>().Include(l=>l.Dashboard).Include(l=>l.ReportType).Include(l=>l.Dashboard).Include(l=>l.DashboardQuery)
			.AsNoTracking().Select(e => new DashboardWidgetListDto()
			{
				Id = e.Id,
				LastModifiedDate = e.LastModifiedDate,
				DashboardId = e.Dashboard == null ? "" : e.Dashboard!.Code,
				DashboardQueryId = e.DashboardQuery == null ? "" : e.DashboardQuery!.QueryHash,
				ReportTypeId = e.ReportType == null ? "" : e.ReportType!.Code,
				Title = e.Title,
				XAxisColumnName = e.XAxisColumnName,
				YAxisColumnsJson = e.YAxisColumnsJson,
				SeriesColumnName = e.SeriesColumnName,
				AggregationOverride = e.AggregationOverride,
				DrillDownDashboardId = e.Dashboard == null ? "" : e.Dashboard!.Code,
				GridPositionX = e.GridPositionX,
				GridPositionY = e.GridPositionY,
				GridWidth = e.GridWidth,
				GridHeight = e.GridHeight,
				RefreshIntervalOverrideSeconds = e.RefreshIntervalOverrideSeconds,
				Sequence = e.Sequence,
			})
			.ToPagedResponse(request.SearchColumns, request.SearchValue,
				request.SortColumn, request.SortOrder,
				request.PageNumber, request.PageSize));
	}	
}
