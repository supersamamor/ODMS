using FBSC.Common.Core.Queries;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FBSC.ODMS.Application.Features.ODMS.Project.Queries;

public record GetProjectByIdQuery(string Id) : BaseQueryById(Id), IRequest<Option<ProjectState>>;

public class GetProjectByIdQueryHandler(ApplicationContext context) : BaseQueryByIdHandler<ApplicationContext, ProjectState, GetProjectByIdQuery>(context), IRequestHandler<GetProjectByIdQuery, Option<ProjectState>>
{
	
	public override async Task<Option<ProjectState>> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken = default)
	{
		return await Context.Project.Include(l=>l.Employee).Include(l=>l.BusinessUnit)
			.Include(l=>l.TeamMembersList)
			.Where(e => e.Id == request.Id).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
	}
	
}
