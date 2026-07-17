using FBSC.Common.Utility.Extensions;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FBSC.Common.Core.Queries;
using FBSC.ODMS.Core.Identity;

namespace FBSC.ODMS.Web.Areas.Admin.Queries.Users;

public record GetUsersQuery : BaseQuery, IRequest<PagedListResponse<ApplicationUser>>
{
}

public class GetUsersQueryHandler(IdentityContext context) : IRequestHandler<GetUsersQuery, PagedListResponse<ApplicationUser>>
{
    public Task<PagedListResponse<ApplicationUser>> Handle(GetUsersQuery request, CancellationToken cancellationToken) =>
        Task.FromResult(context.Users.AsNoTracking().ToPagedResponse(request.SearchColumns, request.SearchValue,
                                                            request.SortColumn, request.SortOrder,
                                                            request.PageNumber, request.PageSize));
}
