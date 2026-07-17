using FBSC.Common.Core.Queries;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FBSC.ODMS.Application.Features.ODMS.DashboardAccess.Queries;

public record GetDashboardAccessByIdQuery(string Id) : BaseQueryById(Id), IRequest<Option<DashboardAccessState>>;

public class GetDashboardAccessByIdQueryHandler(ApplicationContext context) : BaseQueryByIdHandler<ApplicationContext, DashboardAccessState, GetDashboardAccessByIdQuery>(context), IRequestHandler<GetDashboardAccessByIdQuery, Option<DashboardAccessState>>
{
	
	public override async Task<Option<DashboardAccessState>> Handle(GetDashboardAccessByIdQuery request, CancellationToken cancellationToken = default)
	{
		return await Context.DashboardAccess.Include(l=>l.Dashboard)
			.Where(e => e.Id == request.Id).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
	}
	
}
