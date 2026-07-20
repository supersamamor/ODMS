using FBSC.Common.Core.Queries;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using MediatR;
using FBSC.Common.Utility.Extensions;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Application.DTOs;
using LanguageExt;

namespace FBSC.ODMS.Application.Features.ODMS.TeamMembers.Queries;

public record GetTeamMembersQuery : BaseQuery, IRequest<PagedListResponse<TeamMembersListDto>>;

public class GetTeamMembersQueryHandler(ApplicationContext context) : BaseQueryHandler<ApplicationContext, TeamMembersListDto, GetTeamMembersQuery>(context), IRequestHandler<GetTeamMembersQuery, PagedListResponse<TeamMembersListDto>>
{
	public override Task<PagedListResponse<TeamMembersListDto>> Handle(GetTeamMembersQuery request, CancellationToken cancellationToken = default)
	{
		return Task.FromResult(Context.Set<TeamMembersState>().Include(l=>l.Project)
			.AsNoTracking().Select(e => new TeamMembersListDto()
			{
				Id = e.Id,
				LastModifiedDate = e.LastModifiedDate,
				MemberName = e.MemberName,
				Role = e.Role,
			})
			.ToPagedResponse(request.SearchColumns, request.SearchValue,
				request.SortColumn, request.SortOrder,
				request.PageNumber, request.PageSize));
	}	
}
