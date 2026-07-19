using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using FBSC.Common.Core.Queries;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FBSC.ODMS.Application.Features.ODMS.Approval.Queries;

public record GetApproverSetupByIdQuery(string Id) : BaseQueryById(Id), IRequest<Option<ApproverSetupState>>;

public class GetApproverSetupByIdQueryHandler(ApplicationContext context) : BaseQueryByIdHandler<ApplicationContext, ApproverSetupState, GetApproverSetupByIdQuery>(context), IRequestHandler<GetApproverSetupByIdQuery, Option<ApproverSetupState>>
{
    public override async Task<Option<ApproverSetupState>> Handle(GetApproverSetupByIdQuery request, CancellationToken cancellationToken = default)
    {
        return await Context.ApproverSetup
            .Include(l => l.ApproverAssignmentList)
            .Where(e => e.Id == request.Id).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
    }

}