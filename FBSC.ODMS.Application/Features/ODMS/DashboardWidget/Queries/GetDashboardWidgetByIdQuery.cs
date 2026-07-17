using FBSC.Common.Core.Queries;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FBSC.ODMS.Application.Features.ODMS.DashboardWidget.Queries;

public record GetDashboardWidgetByIdQuery(string Id) : BaseQueryById(Id), IRequest<Option<DashboardWidgetState>>;

public class GetDashboardWidgetByIdQueryHandler(ApplicationContext context) : BaseQueryByIdHandler<ApplicationContext, DashboardWidgetState, GetDashboardWidgetByIdQuery>(context), IRequestHandler<GetDashboardWidgetByIdQuery, Option<DashboardWidgetState>>
{
	
	public override async Task<Option<DashboardWidgetState>> Handle(GetDashboardWidgetByIdQuery request, CancellationToken cancellationToken = default)
	{
		return await Context.DashboardWidget.Include(l=>l.Dashboard).Include(l=>l.ReportType).Include(l=>l.Dashboard).Include(l=>l.DashboardQuery)
			.Where(e => e.Id == request.Id).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
	}
	
}
