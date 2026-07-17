using AutoMapper;
using FBSC.ODMS.Infrastructure.Data;
using FBSC.Common.Data;
using LanguageExt;
using LanguageExt.Common;
using MediatR;
using static LanguageExt.Prelude;

namespace FBSC.ODMS.Web.Areas.Admin.Commands.AuditTrail;

public class AddAuditLogCommand : Audit, IRequest<Validation<Error, Audit>>
{
}

public class AddAuditLogCommandHandler(ApplicationContext context, IMapper mapper) : IRequestHandler<AddAuditLogCommand, Validation<Error, Audit>>
{
    public async Task<Validation<Error, Audit>> Handle(AddAuditLogCommand request, CancellationToken cancellationToken)
    {
        var log = mapper.Map<Audit>(request);
        log.DateTime = log.DateTime == DateTime.MinValue ? DateTime.UtcNow : log.DateTime;
        context.Add(log);
        _ = await context.SaveChangesAsync(cancellationToken);
        return Success<Error, Audit>(log);
    }
}
