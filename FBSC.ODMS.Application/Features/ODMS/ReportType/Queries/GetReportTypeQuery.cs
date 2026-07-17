using FBSC.Common.Core.Queries;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using MediatR;
using FBSC.Common.Utility.Extensions;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Application.DTOs;
using LanguageExt;

namespace FBSC.ODMS.Application.Features.ODMS.ReportType.Queries;

public record GetReportTypeQuery : BaseQuery, IRequest<PagedListResponse<ReportTypeListDto>>;

public class GetReportTypeQueryHandler(ApplicationContext context) : BaseQueryHandler<ApplicationContext, ReportTypeListDto, GetReportTypeQuery>(context), IRequestHandler<GetReportTypeQuery, PagedListResponse<ReportTypeListDto>>
{
	public override async Task<PagedListResponse<ReportTypeListDto>> Handle(GetReportTypeQuery request, CancellationToken cancellationToken = default)
	{
		var pagedList = Context.Set<ReportTypeState>()
			.AsNoTracking().Select(e => new ReportTypeListDto()
			{
				Id = e.Id,
				LastModifiedDate = e.LastModifiedDate,
				Code = e.Code,
				Name = e.Name,
				ChartRenderer = e.ChartRenderer,
				MinColumnsRequired = e.MinColumnsRequired,
				MaxColumnsRequired = e.MaxColumnsRequired,
				RequiresXAxis = e.RequiresXAxis,
				RequiresYAxis = e.RequiresYAxis,
				RequiresSeriesGrouping = e.RequiresSeriesGrouping,
				IconClass = e.IconClass,
				IsActive = e.IsActive,
			})
			.ToPagedResponse(request.SearchColumns, request.SearchValue,
				request.SortColumn, request.SortOrder,
				request.PageNumber, request.PageSize);
		foreach (var item in pagedList.Data)
		{
			item.StatusBadge = await Helpers.ApprovalHelper.GetApprovalStatus(Context, item.Id, cancellationToken);
		}
		return pagedList;
	}	
}
