using FBSC.ODMS.Infrastructure.Data;
using MediatR;
using FBSC.Common.Utility.Extensions;
using Microsoft.EntityFrameworkCore;
using LanguageExt;
using FBSC.Common.Data;
using FBSC.ODMS.Web.Areas.Admin.Models;
using FBSC.Common.Utility.Models;
using FBSC.Common.Core.Queries;
namespace FBSC.ODMS.Web.Areas.Admin.Queries.AuditTrail;
public record GetAuditLogsQuery : BaseQuery, IRequest<PagedListResponse<AuditLogViewModel>>;
public class GetAuditLogsQueryHandler(ApplicationContext context) : BaseQueryHandler<ApplicationContext, AuditLogViewModel, GetAuditLogsQuery>(context), IRequestHandler<GetAuditLogsQuery, PagedListResponse<AuditLogViewModel>>
{
    public override Task<PagedListResponse<AuditLogViewModel>> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Context.Set<Audit>()
            .AsNoTracking().Select(e => new AuditLogViewModel()
            {
                Id = e.Id,
                UserId = e.UserId,
                Type = e.Type,
                TableName = e.TableName,
                DateTime = e.DateTime,
                PrimaryKey = e.PrimaryKey,
                TraceId = e.TraceId,
            })
            .ToPagedResponse(request.SearchColumns, request.SearchValue,
                request.SortColumn, request.SortOrder,
                request.PageNumber, request.PageSize));
    }
}
