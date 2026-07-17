using FBSC.Common.Core.Queries;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using MediatR;
using FBSC.Common.Utility.Extensions;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Application.DTOs;
using LanguageExt;

namespace FBSC.ODMS.Application.Features.ODMS.DashboardQueryResultColumn.Queries;

public record GetDashboardQueryResultColumnQuery : BaseQuery, IRequest<PagedListResponse<DashboardQueryResultColumnListDto>>;

public class GetDashboardQueryResultColumnQueryHandler(ApplicationContext context) : BaseQueryHandler<ApplicationContext, DashboardQueryResultColumnListDto, GetDashboardQueryResultColumnQuery>(context), IRequestHandler<GetDashboardQueryResultColumnQuery, PagedListResponse<DashboardQueryResultColumnListDto>>
{
	public override Task<PagedListResponse<DashboardQueryResultColumnListDto>> Handle(GetDashboardQueryResultColumnQuery request, CancellationToken cancellationToken = default)
	{
		return Task.FromResult(Context.Set<DashboardQueryResultColumnState>().Include(l=>l.DashboardQuery)
			.AsNoTracking().Select(e => new DashboardQueryResultColumnListDto()
			{
				Id = e.Id,
				LastModifiedDate = e.LastModifiedDate,
				DashboardQueryId = e.DashboardQuery == null ? "" : e.DashboardQuery!.QueryHash,
				ColumnName = e.ColumnName,
				OrdinalPosition = e.OrdinalPosition,
				SqlDataType = e.SqlDataType,
				InferredRole = e.InferredRole,
				IsAggregatable = e.IsAggregatable,
				DefaultAggregation = e.DefaultAggregation,
				FormatString = e.FormatString,
				Sequence = e.Sequence,
			})
			.ToPagedResponse(request.SearchColumns, request.SearchValue,
				request.SortColumn, request.SortOrder,
				request.PageNumber, request.PageSize));
	}	
}
