using FBSC.Common.Core.Queries;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using MediatR;
using FBSC.Common.Utility.Extensions;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Application.DTOs;
using LanguageExt;

namespace FBSC.ODMS.Application.Features.ODMS.Dashboard.Queries;

public record GetDashboardQuery : BaseQuery, IRequest<PagedListResponse<DashboardListDto>>;

public class GetDashboardQueryHandler(ApplicationContext context) : BaseQueryHandler<ApplicationContext, DashboardListDto, GetDashboardQuery>(context), IRequestHandler<GetDashboardQuery, PagedListResponse<DashboardListDto>>
{
	public override async Task<PagedListResponse<DashboardListDto>> Handle(GetDashboardQuery request, CancellationToken cancellationToken = default)
	{
		var pagedList = Context.Set<DashboardState>().Include(l=>l.DashboardTheme)
			.AsNoTracking().Select(e => new DashboardListDto()
			{
				Id = e.Id,
				LastModifiedDate = e.LastModifiedDate,
				Code = e.Code,
				Name = e.Name,
				Description = e.Description,
				Category = e.Category,
				DashboardThemeId = e.DashboardTheme == null ? "" : e.DashboardTheme!.Code,
				OwnerUserId = e.OwnerUserId,
				IsPublic = e.IsPublic,
				IsTemplate = e.IsTemplate,
				RefreshIntervalSeconds = e.RefreshIntervalSeconds,
				LastPublishedAt = e.LastPublishedAt,
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
