using X.PagedList;

namespace FBSC.Common.Utility.Models;

/// <summary>
/// A class representing a paged list.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <remarks>
/// Creates an instance of <see cref="PagedListResponse{T}"/>.
/// </remarks>
/// <param name="data">An instance of <see cref="IPagedList{T}"/></param>
/// <param name="totalCount">The total no of records in the list.</param>
public class PagedListResponse<T>(IPagedList<T> data, int totalCount)
{
    /// <summary>
    /// The data.
    /// </summary>
    public IPagedList<T> Data { get; private set; } = data;

    /// <summary>
    /// Total no of records in the list.
    /// </summary>
    public int TotalCount { get; private set; } = totalCount;
}