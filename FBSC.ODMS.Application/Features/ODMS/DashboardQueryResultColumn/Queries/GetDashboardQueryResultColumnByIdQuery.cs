using FBSC.Common.Core.Queries;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FBSC.ODMS.Application.Features.ODMS.DashboardQueryResultColumn.Queries;

public record GetDashboardQueryResultColumnByIdQuery(string Id) : BaseQueryById(Id), IRequest<Option<DashboardQueryResultColumnState>>;

public class GetDashboardQueryResultColumnByIdQueryHandler(ApplicationContext context) : BaseQueryByIdHandler<ApplicationContext, DashboardQueryResultColumnState, GetDashboardQueryResultColumnByIdQuery>(context), IRequestHandler<GetDashboardQueryResultColumnByIdQuery, Option<DashboardQueryResultColumnState>>
{
	
	public override async Task<Option<DashboardQueryResultColumnState>> Handle(GetDashboardQueryResultColumnByIdQuery request, CancellationToken cancellationToken = default)
	{
		return await Context.DashboardQueryResultColumn.Include(l=>l.DashboardQuery)
			.Where(e => e.Id == request.Id).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
	}
	
}
