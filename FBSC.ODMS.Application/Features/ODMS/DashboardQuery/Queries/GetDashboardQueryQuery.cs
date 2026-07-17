using FBSC.Common.Core.Queries;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using MediatR;
using FBSC.Common.Utility.Extensions;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Application.DTOs;
using LanguageExt;

namespace FBSC.ODMS.Application.Features.ODMS.DashboardQuery.Queries;

public record GetDashboardQueryQuery : BaseQuery, IRequest<PagedListResponse<DashboardQueryListDto>>;

public class GetDashboardQueryQueryHandler(ApplicationContext context) : BaseQueryHandler<ApplicationContext, DashboardQueryListDto, GetDashboardQueryQuery>(context), IRequestHandler<GetDashboardQueryQuery, PagedListResponse<DashboardQueryListDto>>
{
	public override async Task<PagedListResponse<DashboardQueryListDto>> Handle(GetDashboardQueryQuery request, CancellationToken cancellationToken = default)
	{
		var pagedList = Context.Set<DashboardQueryState>().Include(l=>l.DataSource)
			.AsNoTracking().Select(e => new DashboardQueryListDto()
			{
				Id = e.Id,
				LastModifiedDate = e.LastModifiedDate,
				DataSourceId = e.DataSource == null ? "" : e.DataSource!.Name,
				Name = e.Name,
				Description = e.Description,
				IsParameterized = e.IsParameterized,
				GeneratedByAI = e.GeneratedByAI,
				ValidationStatus = e.ValidationStatus,
				LastValidatedAt = e.LastValidatedAt,
				LastExecutionDurationMs = e.LastExecutionDurationMs,
				LastExecutedAt = e.LastExecutedAt,
				LastExecutionErrorRemarks = e.LastExecutionErrorRemarks,
				IsActive = e.IsActive,
			})
			.ToPagedResponse(request.SearchColumns, request.SearchValue,
				request.SortColumn, request.SortOrder,
				request.PageNumber, request.PageSize);
		foreach (var item in pagedList.Data)
		{
			item.StatusBadge = await Helpers.ApprovalHelper.GetApprovalStatus(Context, item.Id, cancellationToken);
		}
		return pagedList;
	}	
}
