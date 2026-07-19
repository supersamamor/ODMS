using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using FBSC.Common.Core.Queries;
using FBSC.Common.Utility.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FBSC.Common.Utility.Extensions;

namespace FBSC.ODMS.Application.Features.ODMS.Approval.Queries;

public record GetApproverSetupQuery : BaseQuery, IRequest<PagedListResponse<ApproverSetupState>>;

public class GetApproverSetupQueryHandler(ApplicationContext context) : BaseQueryHandler<ApplicationContext, ApproverSetupState, GetApproverSetupQuery>(context), IRequestHandler<GetApproverSetupQuery, PagedListResponse<ApproverSetupState>>
{
    public override Task<PagedListResponse<ApproverSetupState>> Handle(GetApproverSetupQuery request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Context.ApproverSetup.Where(l=>l.ApprovalSetupType == ApprovalSetupTypes.Modular).AsNoTracking().ToPagedResponse(request.SearchColumns, request.SearchValue,
                                                                 request.SortColumn, request.SortOrder,
                                                                 request.PageNumber, request.PageSize));
    }

}
