using FBSC.Common.Core.Queries;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using MediatR;
using FBSC.Common.Utility.Extensions;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Application.DTOs;
using LanguageExt;

namespace FBSC.ODMS.Application.Features.ODMS.Project.Queries;

public record GetProjectQuery : BaseQuery, IRequest<PagedListResponse<ProjectListDto>>;

public class GetProjectQueryHandler(ApplicationContext context) : BaseQueryHandler<ApplicationContext, ProjectListDto, GetProjectQuery>(context), IRequestHandler<GetProjectQuery, PagedListResponse<ProjectListDto>>
{
	public override Task<PagedListResponse<ProjectListDto>> Handle(GetProjectQuery request, CancellationToken cancellationToken = default)
	{
		return Task.FromResult(Context.Set<ProjectState>().Include(l=>l.Employee).Include(l=>l.BusinessUnit)
			.AsNoTracking().Select(e => new ProjectListDto()
			{
				Id = e.Id,
				LastModifiedDate = e.LastModifiedDate,
				ProjectName = e.ProjectName,
				BusinessUnitId = e.BusinessUnit == null ? "" : e.BusinessUnit!.Name,
				//Priority = e.Priority,
				//StartDate = e.StartDate,
				//TargetEndDate = e.TargetEndDate,
				//EstimatedBudget = e.EstimatedBudget,
				//ProjectManagerId = e.Employee == null ? "" : e.Employee!.Id,
				//HealthStatus = e.HealthStatus,
				//Phase = e.Phase,
				//ScheduleStatus = e.ScheduleStatus,
				//LastReviewDate = e.LastReviewDate,
				//LastUpdatedDate = e.LastUpdatedDate,
			})
			.ToPagedResponse(request.SearchColumns, request.SearchValue,
				request.SortColumn, request.SortOrder,
				request.PageNumber, request.PageSize));
	}	
}
