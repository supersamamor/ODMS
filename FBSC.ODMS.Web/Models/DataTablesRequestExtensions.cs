using FBSC.Common.Core.Queries;
using FBSC.Common.Utility.Models;
using LanguageExt;

namespace FBSC.ODMS.Web.Models;

public static class DataTablesRequestExtensions
{
    /// <summary>
    /// Converts <see cref="DataTablesRequest"/> to <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dataTablesRequest"></param>
    /// <returns></returns>
    public static T ToQuery<T>(this DataTablesRequest dataTablesRequest) where T : BaseQuery, new()
    {
        var columns = dataTablesRequest.Columns ?? [];
        var search = dataTablesRequest.Search;
        var orders = dataTablesRequest.Orders ?? [];
        var sort = orders.FirstOrDefault();     
        T query = new()
        {
            PageNumber = (dataTablesRequest.Start / dataTablesRequest.Length) + 1,
            PageSize = dataTablesRequest.Length,
            SearchColumns = columns?.Where(c => c.Searchable)?.Select(c => c.Name)?.ToArray(),
            SearchValue = search?.Value,
            SortColumn = sort != null && sort.Column < columns?.Count() ? columns.ElementAt(sort.Column).Name : null,
            SortOrder = sort?.Dir
        };
        return query;
    }
    public static DataTablesResponse<T> ToDataTablesResponse<T>(this IEnumerable<T> collection, DataTablesRequest request)
    {
        return new DataTablesResponse<T>
        {
            Draw = request.Draw,
            RecordsTotal = collection.Count(),
            RecordsFiltered = collection.Count(),
            Data = collection
        };
    }
    /// <summary>
    /// Converts <see cref="PagedListResponse{T}"/> to <see cref="DataTablesResponse{T}"/>.
    /// Data in <see cref="PagedListResponse{T}"/> is converted to <typeparamref name="TModel"/>
    /// using the provided mapper function.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TModel"></typeparam>
    /// <param name="result"></param>
    /// <param name="dataTablesRequest"></param>
    /// <param name="mapper">The function to convert <typeparamref name="TEntity"/> to <typeparamref name="TModel"/></param>
    /// <returns></returns>
    public static DataTablesResponse<TModel> ToDataTablesResponse<TEntity, TModel>(this PagedListResponse<TEntity> result, DataTablesRequest? dataTablesRequest, Func<TEntity, TModel> mapper) =>
        result.Data.Map(r => mapper(r)).ToDataTablesResponse(dataTablesRequest, result.TotalCount, result.Data.TotalItemCount);

    public static DataTablesResponse<T> ToDataTablesResponse<T>(this IEnumerable<T> collection, DataTablesRequest request, int recordsTotal)
    {
        return new DataTablesResponse<T>
        {
            Draw = request.Draw,
            RecordsTotal = recordsTotal,
            RecordsFiltered = recordsTotal,
            Data = collection
        };
    }
    public static DataTablesResponse<T> ToDataTablesResponse<T>(this IEnumerable<T> collection, DataTablesRequest? request, int recordsTotal, int recordsFiltered)
    {
        return new DataTablesResponse<T>
        {
            Draw = request?.Draw == null ? 0 : request!.Draw,
            RecordsTotal = recordsTotal,
            RecordsFiltered = recordsFiltered,
            Data = collection
        };
    }
}
