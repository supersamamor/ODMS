using FBSC.Common.Core.Queries;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using MediatR;
using FBSC.Common.Utility.Extensions;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Application.DTOs;
using LanguageExt;

namespace FBSC.ODMS.Application.Features.ODMS.DashboardQueryResultCache.Queries;

public record GetDashboardQueryResultCacheQuery : BaseQuery, IRequest<PagedListResponse<DashboardQueryResultCacheListDto>>;

public class GetDashboardQueryResultCacheQueryHandler(ApplicationContext context) : BaseQueryHandler<ApplicationContext, DashboardQueryResultCacheListDto, GetDashboardQueryResultCacheQuery>(context), IRequestHandler<GetDashboardQueryResultCacheQuery, PagedListResponse<DashboardQueryResultCacheListDto>>
{
	public override Task<PagedListResponse<DashboardQueryResultCacheListDto>> Handle(GetDashboardQueryResultCacheQuery request, CancellationToken cancellationToken = default)
	{
		return Task.FromResult(Context.Set<DashboardQueryResultCacheState>().Include(l=>l.DashboardQuery)
			.AsNoTracking().Select(e => new DashboardQueryResultCacheListDto()
			{
				Id = e.Id,
				LastModifiedDate = e.LastModifiedDate,
				DashboardQueryId = e.DashboardQuery == null ? "" : e.DashboardQuery!.QueryHash,
				RowCount = e.RowCount,
				CacheSizeBytes = e.CacheSizeBytes,
				CachedAt = e.CachedAt,
				ExpiresAt = e.ExpiresAt,
				IsStale = e.IsStale,
			})
			.ToPagedResponse(request.SearchColumns, request.SearchValue,
				request.SortColumn, request.SortOrder,
				request.PageNumber, request.PageSize));
	}	
}
