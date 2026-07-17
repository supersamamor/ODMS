using FBSC.ODMS.Infrastructure.Data;
using FBSC.Common.Data;
using LanguageExt;
using MediatR;

namespace FBSC.ODMS.Web.Areas.Admin.Queries.AuditTrail;

public record GetAuditLogByIdQuery(int Id) : IRequest<Option<Audit>>;

public class GetAuditLogByIdQueryHandler(ApplicationContext context) : IRequestHandler<GetAuditLogByIdQuery, Option<Audit>>
{
    public async Task<Option<Audit>> Handle(GetAuditLogByIdQuery request, CancellationToken cancellationToken) =>
        await context.GetSingle<Audit>(e => e.Id == request.Id, cancellationToken: cancellationToken);
}
