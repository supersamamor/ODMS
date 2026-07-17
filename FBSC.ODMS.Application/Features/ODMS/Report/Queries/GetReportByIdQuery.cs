using FBSC.Common.Core.Queries;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FBSC.ODMS.Application.Features.ODMS.Report.Queries;

public record GetReportByIdQuery(string Id) : BaseQueryById(Id), IRequest<Option<ReportState>>;

public class GetReportByIdQueryHandler(ApplicationContext context) : BaseQueryByIdHandler<ApplicationContext, ReportState, GetReportByIdQuery>(context), IRequestHandler<GetReportByIdQuery, Option<ReportState>>
{
    public override async Task<Option<ReportState>> Handle(GetReportByIdQuery request, CancellationToken cancellationToken = default)
    {
        return await Context.Report         
            .Include(l => l.ReportQueryFilterList)
            .Include(l => l.ReportRoleAssignmentList)
            .Where(e => e.Id == request.Id).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
    }

}
