using FBSC.Common.Core.Queries;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using MediatR;
using FBSC.Common.Utility.Extensions;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Application.DTOs;
using LanguageExt;

namespace FBSC.ODMS.Application.Features.ODMS.DataUploadColumn.Queries;

public record GetDataUploadColumnQuery : BaseQuery, IRequest<PagedListResponse<DataUploadColumnListDto>>;

public class GetDataUploadColumnQueryHandler(ApplicationContext context) : BaseQueryHandler<ApplicationContext, DataUploadColumnListDto, GetDataUploadColumnQuery>(context), IRequestHandler<GetDataUploadColumnQuery, PagedListResponse<DataUploadColumnListDto>>
{
	public override Task<PagedListResponse<DataUploadColumnListDto>> Handle(GetDataUploadColumnQuery request, CancellationToken cancellationToken = default)
	{
		return Task.FromResult(Context.Set<DataUploadColumnState>().Include(l=>l.DataUploadBatch)
			.AsNoTracking().Select(e => new DataUploadColumnListDto()
			{
				Id = e.Id,
				LastModifiedDate = e.LastModifiedDate,
				DataUploadBatchId = e.DataUploadBatch == null ? "" : e.DataUploadBatch!.Id,
				ColumnName = e.ColumnName,
				DetectedDataType = e.DetectedDataType,
				MappedSqlDataType = e.MappedSqlDataType,
				OrdinalPosition = e.OrdinalPosition,
				SampleValue = e.SampleValue,
			})
			.ToPagedResponse(request.SearchColumns, request.SearchValue,
				request.SortColumn, request.SortOrder,
				request.PageNumber, request.PageSize));
	}	
}
