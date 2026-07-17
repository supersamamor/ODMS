using FBSC.Common.Core.Queries;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using MediatR;
using FBSC.Common.Utility.Extensions;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Application.DTOs;
using LanguageExt;

namespace FBSC.ODMS.Application.Features.ODMS.DataSourceSchemaCache.Queries;

public record GetDataSourceSchemaCacheQuery : BaseQuery, IRequest<PagedListResponse<DataSourceSchemaCacheListDto>>;

public class GetDataSourceSchemaCacheQueryHandler(ApplicationContext context) : BaseQueryHandler<ApplicationContext, DataSourceSchemaCacheListDto, GetDataSourceSchemaCacheQuery>(context), IRequestHandler<GetDataSourceSchemaCacheQuery, PagedListResponse<DataSourceSchemaCacheListDto>>
{
	public override Task<PagedListResponse<DataSourceSchemaCacheListDto>> Handle(GetDataSourceSchemaCacheQuery request, CancellationToken cancellationToken = default)
	{
		return Task.FromResult(Context.Set<DataSourceSchemaCacheState>().Include(l=>l.DataSource)
			.AsNoTracking().Select(e => new DataSourceSchemaCacheListDto()
			{
				Id = e.Id,
				LastModifiedDate = e.LastModifiedDate,
				DataSourceId = e.DataSource == null ? "" : e.DataSource!.Name,
				SchemaName = e.SchemaName,
				TableName = e.TableName,
				ColumnName = e.ColumnName,
				SqlDataType = e.SqlDataType,
				OrdinalPosition = e.OrdinalPosition,
				IsNullable = e.IsNullable,
				RefreshedAt = e.RefreshedAt,
			})
			.ToPagedResponse(request.SearchColumns, request.SearchValue,
				request.SortColumn, request.SortOrder,
				request.PageNumber, request.PageSize));
	}	
}
