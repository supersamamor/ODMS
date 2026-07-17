using FBSC.Common.Core.Queries;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using MediatR;
using FBSC.Common.Utility.Extensions;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Application.DTOs;
using LanguageExt;

namespace FBSC.ODMS.Application.Features.ODMS.DashboardQueryParameter.Queries;

public record GetDashboardQueryParameterQuery : BaseQuery, IRequest<PagedListResponse<DashboardQueryParameterListDto>>;

public class GetDashboardQueryParameterQueryHandler(ApplicationContext context) : BaseQueryHandler<ApplicationContext, DashboardQueryParameterListDto, GetDashboardQueryParameterQuery>(context), IRequestHandler<GetDashboardQueryParameterQuery, PagedListResponse<DashboardQueryParameterListDto>>
{
	public override Task<PagedListResponse<DashboardQueryParameterListDto>> Handle(GetDashboardQueryParameterQuery request, CancellationToken cancellationToken = default)
	{
		return Task.FromResult(Context.Set<DashboardQueryParameterState>().Include(l=>l.DashboardQuery)
			.AsNoTracking().Select(e => new DashboardQueryParameterListDto()
			{
				Id = e.Id,
				LastModifiedDate = e.LastModifiedDate,
				DashboardQueryId = e.DashboardQuery == null ? "" : e.DashboardQuery!.QueryHash,
				ParameterName = e.ParameterName,
				DataType = e.DataType,
				ControlType = e.ControlType,
				DefaultValue = e.DefaultValue,
				IsRequired = e.IsRequired,
				Sequence = e.Sequence,
			})
			.ToPagedResponse(request.SearchColumns, request.SearchValue,
				request.SortColumn, request.SortOrder,
				request.PageNumber, request.PageSize));
	}	
}
