using FBSC.Common.Core.Queries;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using MediatR;
using FBSC.Common.Utility.Extensions;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Application.DTOs;
using LanguageExt;

namespace FBSC.ODMS.Application.Features.ODMS.Milestone.Queries;

public record GetMilestoneQuery : BaseQuery, IRequest<PagedListResponse<MilestoneListDto>>;

public class GetMilestoneQueryHandler(ApplicationContext context) : BaseQueryHandler<ApplicationContext, MilestoneListDto, GetMilestoneQuery>(context), IRequestHandler<GetMilestoneQuery, PagedListResponse<MilestoneListDto>>
{
	public override Task<PagedListResponse<MilestoneListDto>> Handle(GetMilestoneQuery request, CancellationToken cancellationToken = default)
	{
		return Task.FromResult(Context.Set<MilestoneState>()
			.AsNoTracking().Select(e => new MilestoneListDto()
			{
				Id = e.Id,
				LastModifiedDate = e.LastModifiedDate,
				Name = e.Name,
			})
			.ToPagedResponse(request.SearchColumns, request.SearchValue,
				request.SortColumn, request.SortOrder,
				request.PageNumber, request.PageSize));
	}	
}
