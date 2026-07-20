using FBSC.Common.Core.Queries;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FBSC.ODMS.Application.Features.ODMS.ProjectHistory.Queries;

public record GetProjectHistoryByIdQuery(string Id) : BaseQueryById(Id), IRequest<Option<ProjectHistoryState>>;

public class GetProjectHistoryByIdQueryHandler(ApplicationContext context) : BaseQueryByIdHandler<ApplicationContext, ProjectHistoryState, GetProjectHistoryByIdQuery>(context), IRequestHandler<GetProjectHistoryByIdQuery, Option<ProjectHistoryState>>
{
	
	public override async Task<Option<ProjectHistoryState>> Handle(GetProjectHistoryByIdQuery request, CancellationToken cancellationToken = default)
	{
		return await Context.ProjectHistory.Include(l=>l.Employee).Include(l=>l.BusinessUnit).Include(l=>l.Project)
			.Include(l=>l.TeamMembersHistoryList)
			.Where(e => e.Id == request.Id).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
	}
	
}
