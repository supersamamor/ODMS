using FBSC.Common.Utility.Extensions;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FBSC.Common.Core.Queries;
using FBSC.ODMS.Core.ODMS;

namespace FBSC.ODMS.Web.Areas.Admin.Queries.BatchUploadJobs;

public record GetBatchUploadJobsQuery : BaseQuery, IRequest<PagedListResponse<UploadProcessorState>>
{
}

public class GetBatchUploadJobsQueryHandler(ApplicationContext context) : IRequestHandler<GetBatchUploadJobsQuery, PagedListResponse<UploadProcessorState>>
{

    public Task<PagedListResponse<UploadProcessorState>> Handle(GetBatchUploadJobsQuery request, CancellationToken cancellationToken) =>
        Task.FromResult(context.UploadProcessor.AsNoTracking().ToPagedResponse(request.SearchColumns, request.SearchValue,
                                                               request.SortColumn, request.SortOrder,
                                                               request.PageNumber, request.PageSize));
}
