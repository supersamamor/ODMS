using FBSC.Common.Core.Queries;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FBSC.ODMS.Application.Features.ODMS.TeamMembersHistory.Queries;

public record GetTeamMembersHistoryByIdQuery(string Id) : BaseQueryById(Id), IRequest<Option<TeamMembersHistoryState>>;

public class GetTeamMembersHistoryByIdQueryHandler(ApplicationContext context) : BaseQueryByIdHandler<ApplicationContext, TeamMembersHistoryState, GetTeamMembersHistoryByIdQuery>(context), IRequestHandler<GetTeamMembersHistoryByIdQuery, Option<TeamMembersHistoryState>>
{
	
	public override async Task<Option<TeamMembersHistoryState>> Handle(GetTeamMembersHistoryByIdQuery request, CancellationToken cancellationToken = default)
	{
		return await Context.TeamMembersHistory.Include(l=>l.ProjectHistory)
			.Where(e => e.Id == request.Id).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
	}
	
}
