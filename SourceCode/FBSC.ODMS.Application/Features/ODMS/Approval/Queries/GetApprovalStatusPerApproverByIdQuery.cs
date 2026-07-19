using FBSC.ODMS.Infrastructure.Data;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FBSC.Common.Identity.Abstractions;
using FBSC.ODMS.Core.ODMS;

namespace FBSC.ODMS.Application.Features.ODMS.Approval.Queries;

public record GetApprovalStatusPerApproverByIdQuery(string DataId, string Module) : IRequest<Option<string>>;

public class GetApprovalStatusPerApproverByIdQueryHandler(ApplicationContext context, IAuthenticatedUser authenticatedUser) : IRequestHandler<GetApprovalStatusPerApproverByIdQuery, Option<string>>
{
    private readonly ApplicationContext _context = context;
    private readonly IAuthenticatedUser _authenticatedUser = authenticatedUser;

    public async Task<Option<string>> Handle(GetApprovalStatusPerApproverByIdQuery request, CancellationToken cancellationToken = default)
    {
        return await (from a in _context.ApprovalRecord
                     join b in _context.Approval on a.Id equals b.ApprovalRecordId
                     join c in _context.ApproverSetup on a.ApproverSetupId equals c.Id
                     where b.ApproverUserId == _authenticatedUser.UserId && a.DataId == request.DataId
                     && c.TableName == request.Module
                     select b.Status).FirstOrDefaultAsync(cancellationToken);
    }
}
