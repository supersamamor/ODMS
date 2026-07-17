using FBSC.Common.Core.Queries;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FBSC.ODMS.Application.Features.ODMS.Dashboard.Queries;

public record GetDashboardByIdQuery(string Id) : BaseQueryById(Id), IRequest<Option<DashboardState>>;

public class GetDashboardByIdQueryHandler(ApplicationContext context) : BaseQueryByIdHandler<ApplicationContext, DashboardState, GetDashboardByIdQuery>(context), IRequestHandler<GetDashboardByIdQuery, Option<DashboardState>>
{
	
	public override async Task<Option<DashboardState>> Handle(GetDashboardByIdQuery request, CancellationToken cancellationToken = default)
	{
		return await Context.Dashboard.Include(l=>l.DashboardTheme)
			.Include(l=>l.DashboardWidgetList)
			.Include(l=>l.DashboardWidgetList)
			.Include(l=>l.DashboardAccessList)
			.Where(e => e.Id == request.Id).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
	}
	
}
