using FBSC.Common.Core.Queries;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using MediatR;
using FBSC.Common.Utility.Extensions;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Application.DTOs;
using LanguageExt;

namespace FBSC.ODMS.Application.Features.ODMS.TeamMembersHistory.Queries;

public record GetTeamMembersHistoryQuery : BaseQuery, IRequest<PagedListResponse<TeamMembersHistoryListDto>>;

public class GetTeamMembersHistoryQueryHandler(ApplicationContext context) : BaseQueryHandler<ApplicationContext, TeamMembersHistoryListDto, GetTeamMembersHistoryQuery>(context), IRequestHandler<GetTeamMembersHistoryQuery, PagedListResponse<TeamMembersHistoryListDto>>
{
	public override Task<PagedListResponse<TeamMembersHistoryListDto>> Handle(GetTeamMembersHistoryQuery request, CancellationToken cancellationToken = default)
	{
		return Task.FromResult(Context.Set<TeamMembersHistoryState>().Include(l=>l.ProjectHistory)
			.Include(l=>l.Employee)
			.AsNoTracking().Select(e => new TeamMembersHistoryListDto()
			{
                Id = e.Id,
                LastModifiedDate = e.LastModifiedDate,
                MemberLevel = e.MemberLevel,
                Name = e.Employee != null ? e.Employee.Name : string.Empty,
                Role = e.Role,
            })
			.ToPagedResponse(request.SearchColumns, request.SearchValue,
				request.SortColumn, request.SortOrder,
				request.PageNumber, request.PageSize));
	}	
}
