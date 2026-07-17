using FBSC.Common.Utility.Extensions;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FBSC.Common.Core.Queries;
using FBSC.ODMS.Core.Identity;

namespace FBSC.ODMS.Web.Areas.Admin.Queries.Roles;

public record GetRolesQuery : BaseQuery, IRequest<PagedListResponse<ApplicationRole>>
{
}

public class GetRolesQueryHandler(IdentityContext context) : IRequestHandler<GetRolesQuery, PagedListResponse<ApplicationRole>>
{
    public Task<PagedListResponse<ApplicationRole>> Handle(GetRolesQuery request, CancellationToken cancellationToken) =>
        Task.FromResult(context.Roles.AsNoTracking().ToPagedResponse(request.SearchColumns, request.SearchValue,
                                                            request.SortColumn, request.SortOrder,
                                                            request.PageNumber, request.PageSize));
}
