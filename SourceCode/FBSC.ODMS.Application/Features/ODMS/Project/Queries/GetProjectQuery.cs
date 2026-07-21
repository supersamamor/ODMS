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

public record GetProjectQuery : BaseQuery, IRequest<PagedListResponse<ProjectListDto>>
{
	/// <summary>Optional Delivery Tower/Category filter; null/empty = all.</summary>
	public string? DeliveryCategory { get; set; }
}

public class GetProjectQueryHandler(ApplicationContext context) : BaseQueryHandler<ApplicationContext, ProjectListDto, GetProjectQuery>(context), IRequestHandler<GetProjectQuery, PagedListResponse<ProjectListDto>>
{
	public override Task<PagedListResponse<ProjectListDto>> Handle(GetProjectQuery request, CancellationToken cancellationToken = default)
	{
		var projects = Context.Set<ProjectState>().Include(l=>l.Employee).Include(l=>l.BusinessUnit)
			.AsNoTracking();
		if (!string.IsNullOrEmpty(request.DeliveryCategory))
		{
			projects = projects.Where(e => e.DeliveryCategory == request.DeliveryCategory);
		}
		return Task.FromResult(projects
			.Select(e => new ProjectListDto()
			{
				Id = e.Id,
				LastModifiedDate = e.LastModifiedDate,
				ProjectCode = e.ProjectCode,
				ProjectName = e.ProjectName,
				DeliveryCategory = e.DeliveryCategory,
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
