using FBSC.Common.Core.Queries;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using MediatR;
using FBSC.Common.Utility.Extensions;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Application.DTOs;
using LanguageExt;

namespace FBSC.ODMS.Application.Features.ODMS.DataUpload.Queries;

public record GetDataUploadQuery : BaseQuery, IRequest<PagedListResponse<DataUploadListDto>>;

public class GetDataUploadQueryHandler(ApplicationContext context) : BaseQueryHandler<ApplicationContext, DataUploadListDto, GetDataUploadQuery>(context), IRequestHandler<GetDataUploadQuery, PagedListResponse<DataUploadListDto>>
{
	public override Task<PagedListResponse<DataUploadListDto>> Handle(GetDataUploadQuery request, CancellationToken cancellationToken = default)
	{
		return Task.FromResult(Context.Set<DataUploadState>()
			.AsNoTracking().Select(e => new DataUploadListDto()
			{
				Id = e.Id,
				LastModifiedDate = e.LastModifiedDate,
				Description = e.Description,
				FilePath = e.FilePath,
				FileType = e.FileType,
			})
			.ToPagedResponse(request.SearchColumns, request.SearchValue,
				request.SortColumn, request.SortOrder,
				request.PageNumber, request.PageSize));
	}	
}
