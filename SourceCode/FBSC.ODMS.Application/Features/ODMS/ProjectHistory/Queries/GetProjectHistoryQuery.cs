using FBSC.Common.Core.Queries;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using MediatR;
using FBSC.Common.Utility.Extensions;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Application.DTOs;
using LanguageExt;

namespace FBSC.ODMS.Application.Features.ODMS.ProjectHistory.Queries;

public record GetProjectHistoryQuery : BaseQuery, IRequest<PagedListResponse<ProjectHistoryListDto>>;

public class GetProjectHistoryQueryHandler(ApplicationContext context) : BaseQueryHandler<ApplicationContext, ProjectHistoryListDto, GetProjectHistoryQuery>(context), IRequestHandler<GetProjectHistoryQuery, PagedListResponse<ProjectHistoryListDto>>
{
	public override Task<PagedListResponse<ProjectHistoryListDto>> Handle(GetProjectHistoryQuery request, CancellationToken cancellationToken = default)
	{
		return Task.FromResult(Context.Set<ProjectHistoryState>().Include(l=>l.Employee).Include(l=>l.BusinessUnit).Include(l=>l.Project)
			.AsNoTracking().Select(e => new ProjectHistoryListDto()
			{
				Id = e.Id,
				LastModifiedDate = e.LastModifiedDate,
				ProjectCode = e.ProjectCode,
				ProjectName = e.ProjectName,
				DeliveryTower = e.DeliveryTower,
				DemandType = e.DemandType,
				BusinessUnitId = e.BusinessUnit == null ? "" : e.BusinessUnit!.Name,
				ProjectManagerId = e.Employee == null ? "" : e.Employee!.Name,
				Priority = e.Priority,
				ActiveStatus = e.ActiveStatus,
                BaselineStartDate = e.BaselineStartDate,
                BaselineEndDate = e.BaselineEndDate,
                ApprovedBudget = e.ApprovedBudget,
			})
			.ToPagedResponse(request.SearchColumns, request.SearchValue,
				request.SortColumn, request.SortOrder,
				request.PageNumber, request.PageSize));
	}	
}
