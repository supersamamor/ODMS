using FBSC.Common.Core.Queries;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FBSC.ODMS.Application.Features.ODMS.TeamMembers.Queries;

public record GetTeamMembersByIdQuery(string Id) : BaseQueryById(Id), IRequest<Option<TeamMembersState>>;

public class GetTeamMembersByIdQueryHandler(ApplicationContext context) : BaseQueryByIdHandler<ApplicationContext, TeamMembersState, GetTeamMembersByIdQuery>(context), IRequestHandler<GetTeamMembersByIdQuery, Option<TeamMembersState>>
{
	
	public override async Task<Option<TeamMembersState>> Handle(GetTeamMembersByIdQuery request, CancellationToken cancellationToken = default)
	{
		return await Context.TeamMembers.Include(l=>l.Project)
			.Where(e => e.Id == request.Id).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
	}
	
}
