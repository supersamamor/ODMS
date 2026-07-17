using FBSC.Common.Utility.Extensions;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.Infrastructure.Data;
using FBSC.ODMS.Core.Oidc;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FBSC.Common.Core.Queries;

namespace FBSC.ODMS.Web.Areas.Admin.Queries.Applications;

public record GetApplicationsQuery : BaseQuery, IRequest<PagedListResponse<OidcApplication>>
{
}

public class GetApplicationsQueryHandler(IdentityContext context) : IRequestHandler<GetApplicationsQuery, PagedListResponse<OidcApplication>>
{
    public Task<PagedListResponse<OidcApplication>> Handle(GetApplicationsQuery request, CancellationToken cancellationToken) =>
        Task.FromResult(context.Set<OidcApplication>()
                      .AsNoTracking()
                      .ToPagedResponse(request.SearchColumns, request.SearchValue, request.SortColumn,
                                       request.SortOrder, request.PageNumber, request.PageSize));
}