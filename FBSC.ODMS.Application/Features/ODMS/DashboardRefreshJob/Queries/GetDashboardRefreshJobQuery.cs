using FBSC.Common.Core.Queries;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using MediatR;
using FBSC.Common.Utility.Extensions;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Application.DTOs;
using LanguageExt;

namespace FBSC.ODMS.Application.Features.ODMS.DashboardRefreshJob.Queries;

public record GetDashboardRefreshJobQuery : BaseQuery, IRequest<PagedListResponse<DashboardRefreshJobListDto>>;

public class GetDashboardRefreshJobQueryHandler(ApplicationContext context) : BaseQueryHandler<ApplicationContext, DashboardRefreshJobListDto, GetDashboardRefreshJobQuery>(context), IRequestHandler<GetDashboardRefreshJobQuery, PagedListResponse<DashboardRefreshJobListDto>>
{
	public override Task<PagedListResponse<DashboardRefreshJobListDto>> Handle(GetDashboardRefreshJobQuery request, CancellationToken cancellationToken = default)
	{
		return Task.FromResult(Context.Set<DashboardRefreshJobState>().Include(l=>l.DashboardQuery)
			.AsNoTracking().Select(e => new DashboardRefreshJobListDto()
			{
				Id = e.Id,
				LastModifiedDate = e.LastModifiedDate,
				DashboardQueryId = e.DashboardQuery == null ? "" : e.DashboardQuery!.QueryHash,
				TriggerType = e.TriggerType,
				Status = e.Status,
				QueuedAt = e.QueuedAt,
				StartedAt = e.StartedAt,
				CompletedAt = e.CompletedAt,
				DurationMs = e.DurationMs,
				RowsCached = e.RowsCached,
				RetryCount = e.RetryCount,
				MaxRetries = e.MaxRetries,
				ErrorRemarks = e.ErrorRemarks,
			})
			.ToPagedResponse(request.SearchColumns, request.SearchValue,
				request.SortColumn, request.SortOrder,
				request.PageNumber, request.PageSize));
	}	
}
