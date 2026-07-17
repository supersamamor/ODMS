using FBSC.Common.Core.Queries;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FBSC.ODMS.Application.Features.ODMS.DashboardQuery.Queries;

public record GetDashboardQueryByIdQuery(string Id) : BaseQueryById(Id), IRequest<Option<DashboardQueryState>>;

public class GetDashboardQueryByIdQueryHandler(ApplicationContext context) : BaseQueryByIdHandler<ApplicationContext, DashboardQueryState, GetDashboardQueryByIdQuery>(context), IRequestHandler<GetDashboardQueryByIdQuery, Option<DashboardQueryState>>
{
	
	public override async Task<Option<DashboardQueryState>> Handle(GetDashboardQueryByIdQuery request, CancellationToken cancellationToken = default)
	{
		return await Context.DashboardQuery.Include(l=>l.DataSource)
			.Include(l=>l.DashboardQueryParameterList)
			.Include(l=>l.DashboardQueryResultColumnList)
			.Include(l=>l.DashboardQueryResultCacheList)
			.Include(l=>l.DashboardWidgetList)
			.Include(l=>l.DashboardRefreshJobList)
			.Where(e => e.Id == request.Id).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
	}
	
}
