using FBSC.Common.Core.Queries;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using MediatR;
using FBSC.Common.Utility.Extensions;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Application.DTOs;
using LanguageExt;

namespace FBSC.ODMS.Application.Features.ODMS.DashboardAccess.Queries;

public record GetDashboardAccessQuery : BaseQuery, IRequest<PagedListResponse<DashboardAccessListDto>>;

public class GetDashboardAccessQueryHandler(ApplicationContext context) : BaseQueryHandler<ApplicationContext, DashboardAccessListDto, GetDashboardAccessQuery>(context), IRequestHandler<GetDashboardAccessQuery, PagedListResponse<DashboardAccessListDto>>
{
	public override Task<PagedListResponse<DashboardAccessListDto>> Handle(GetDashboardAccessQuery request, CancellationToken cancellationToken = default)
	{
		return Task.FromResult(Context.Set<DashboardAccessState>().Include(l=>l.Dashboard)
			.AsNoTracking().Select(e => new DashboardAccessListDto()
			{
				Id = e.Id,
				LastModifiedDate = e.LastModifiedDate,
				DashboardId = e.Dashboard == null ? "" : e.Dashboard!.Code,
				GranteeType = e.GranteeType,
				GranteeId = e.GranteeId,
				AccessLevel = e.AccessLevel,
				GrantedAt = e.GrantedAt,
			})
			.ToPagedResponse(request.SearchColumns, request.SearchValue,
				request.SortColumn, request.SortOrder,
				request.PageNumber, request.PageSize));
	}	
}
