using FBSC.Common.Core.Queries;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using MediatR;
using FBSC.Common.Utility.Extensions;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Application.DTOs;
using LanguageExt;

namespace FBSC.ODMS.Application.Features.ODMS.DataUploadBatch.Queries;

public record GetDataUploadBatchQuery : BaseQuery, IRequest<PagedListResponse<DataUploadBatchListDto>>;

public class GetDataUploadBatchQueryHandler(ApplicationContext context) : BaseQueryHandler<ApplicationContext, DataUploadBatchListDto, GetDataUploadBatchQuery>(context), IRequestHandler<GetDataUploadBatchQuery, PagedListResponse<DataUploadBatchListDto>>
{
	public override Task<PagedListResponse<DataUploadBatchListDto>> Handle(GetDataUploadBatchQuery request, CancellationToken cancellationToken = default)
	{
		return Task.FromResult(Context.Set<DataUploadBatchState>().Include(l=>l.DataSource)
			.AsNoTracking().Select(e => new DataUploadBatchListDto()
			{
				Id = e.Id,
				LastModifiedDate = e.LastModifiedDate,
				DataSourceId = e.DataSource == null ? "" : e.DataSource!.Name,
				FileName = e.FileName,
				FileType = e.FileType,
				UploadedBy = e.UploadedBy,
				StagingTableName = e.StagingTableName,
				RowCount = e.RowCount,
				ColumnCount = e.ColumnCount,
				ImportStatus = e.ImportStatus,
				ImportedAt = e.ImportedAt,
				ErrorRemarks = e.ErrorRemarks,
			})
			.ToPagedResponse(request.SearchColumns, request.SearchValue,
				request.SortColumn, request.SortOrder,
				request.PageNumber, request.PageSize));
	}	
}
