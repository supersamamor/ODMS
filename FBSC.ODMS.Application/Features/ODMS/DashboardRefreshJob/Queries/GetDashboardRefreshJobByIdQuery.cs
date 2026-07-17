using FBSC.Common.Core.Queries;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FBSC.ODMS.Application.Features.ODMS.DashboardRefreshJob.Queries;

public record GetDashboardRefreshJobByIdQuery(string Id) : BaseQueryById(Id), IRequest<Option<DashboardRefreshJobState>>;

public class GetDashboardRefreshJobByIdQueryHandler(ApplicationContext context) : BaseQueryByIdHandler<ApplicationContext, DashboardRefreshJobState, GetDashboardRefreshJobByIdQuery>(context), IRequestHandler<GetDashboardRefreshJobByIdQuery, Option<DashboardRefreshJobState>>
{
	
	public override async Task<Option<DashboardRefreshJobState>> Handle(GetDashboardRefreshJobByIdQuery request, CancellationToken cancellationToken = default)
	{
		return await Context.DashboardRefreshJob.Include(l=>l.DashboardQuery)
			.Where(e => e.Id == request.Id).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
	}
	
}
