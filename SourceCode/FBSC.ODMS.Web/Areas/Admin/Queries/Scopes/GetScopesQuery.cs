using FBSC.Common.Utility.Extensions;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.Infrastructure.Data;
using FBSC.ODMS.Core.Oidc;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FBSC.Common.Core.Queries;

namespace FBSC.ODMS.Web.Areas.Admin.Queries.Scopes;

public record GetScopesQuery : BaseQuery, IRequest<PagedListResponse<OidcScope>>
{
}

public class GetScopesQueryHandler(IdentityContext context) : IRequestHandler<GetScopesQuery, PagedListResponse<OidcScope>>
{
    public Task<PagedListResponse<OidcScope>> Handle(GetScopesQuery request, CancellationToken cancellationToken) =>
        Task.FromResult(context.Set<OidcScope>()
                      .AsNoTracking()
                      .ToPagedResponse(request.SearchColumns, request.SearchValue, request.SortColumn,
                                       request.SortOrder, request.PageNumber, request.PageSize));
}