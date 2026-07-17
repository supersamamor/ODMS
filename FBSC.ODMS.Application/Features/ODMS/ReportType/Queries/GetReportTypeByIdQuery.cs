using FBSC.Common.Core.Queries;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FBSC.ODMS.Application.Features.ODMS.ReportType.Queries;

public record GetReportTypeByIdQuery(string Id) : BaseQueryById(Id), IRequest<Option<ReportTypeState>>;

public class GetReportTypeByIdQueryHandler(ApplicationContext context) : BaseQueryByIdHandler<ApplicationContext, ReportTypeState, GetReportTypeByIdQuery>(context), IRequestHandler<GetReportTypeByIdQuery, Option<ReportTypeState>>
{
	
	public override async Task<Option<ReportTypeState>> Handle(GetReportTypeByIdQuery request, CancellationToken cancellationToken = default)
	{
		return await Context.ReportType
			.Include(l=>l.DashboardWidgetList)
			.Where(e => e.Id == request.Id).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
	}
	
}
