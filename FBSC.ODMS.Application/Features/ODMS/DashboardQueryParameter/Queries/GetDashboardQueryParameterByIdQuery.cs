using FBSC.Common.Core.Queries;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FBSC.ODMS.Application.Features.ODMS.DashboardQueryParameter.Queries;

public record GetDashboardQueryParameterByIdQuery(string Id) : BaseQueryById(Id), IRequest<Option<DashboardQueryParameterState>>;

public class GetDashboardQueryParameterByIdQueryHandler(ApplicationContext context) : BaseQueryByIdHandler<ApplicationContext, DashboardQueryParameterState, GetDashboardQueryParameterByIdQuery>(context), IRequestHandler<GetDashboardQueryParameterByIdQuery, Option<DashboardQueryParameterState>>
{
	
	public override async Task<Option<DashboardQueryParameterState>> Handle(GetDashboardQueryParameterByIdQuery request, CancellationToken cancellationToken = default)
	{
		return await Context.DashboardQueryParameter.Include(l=>l.DashboardQuery)
			.Where(e => e.Id == request.Id).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
	}
	
}
