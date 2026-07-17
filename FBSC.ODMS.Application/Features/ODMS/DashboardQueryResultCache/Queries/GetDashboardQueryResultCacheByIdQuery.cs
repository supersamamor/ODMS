using FBSC.Common.Core.Queries;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FBSC.ODMS.Application.Features.ODMS.DashboardQueryResultCache.Queries;

public record GetDashboardQueryResultCacheByIdQuery(string Id) : BaseQueryById(Id), IRequest<Option<DashboardQueryResultCacheState>>;

public class GetDashboardQueryResultCacheByIdQueryHandler(ApplicationContext context) : BaseQueryByIdHandler<ApplicationContext, DashboardQueryResultCacheState, GetDashboardQueryResultCacheByIdQuery>(context), IRequestHandler<GetDashboardQueryResultCacheByIdQuery, Option<DashboardQueryResultCacheState>>
{
	
	public override async Task<Option<DashboardQueryResultCacheState>> Handle(GetDashboardQueryResultCacheByIdQuery request, CancellationToken cancellationToken = default)
	{
		return await Context.DashboardQueryResultCache.Include(l=>l.DashboardQuery)
			.Where(e => e.Id == request.Id).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
	}
	
}
