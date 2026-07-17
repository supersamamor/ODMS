using FBSC.Common.Utility.Extensions;
using FBSC.Common.Utility.Models;
using Microsoft.EntityFrameworkCore;

namespace FBSC.Common.Core.Queries;

/// <summary>
/// A base class for query handlers that retrieve
/// records from a database.
/// </summary>
/// <typeparam name="TContext"></typeparam>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TQuery"></typeparam>
/// <remarks>
/// Initializes an instance of <see cref="BaseQueryHandler{TContext, TEntity, TQuery}"/>.
/// </remarks>
/// <param name="context"></param>
public abstract class BaseQueryHandler<TContext, TEntity, TQuery>(TContext context)
    where TContext : DbContext
    where TEntity : class
    where TQuery : BaseQuery
{
    /// <summary>
    /// An instance of TContext.
    /// </summary>
    protected readonly TContext Context = context;

    /// <summary>
    /// Retrieves records from the database based on the
    /// specified <paramref name="request"/>.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual Task<PagedListResponse<TEntity>> Handle(TQuery request, CancellationToken cancellationToken = default) =>
        Task.FromResult(Context.Set<TEntity>().AsNoTracking().ToPagedResponse(request.SearchColumns, request.SearchValue,
                                                                     request.SortColumn, request.SortOrder,
                                                                     request.PageNumber, request.PageSize));
}
