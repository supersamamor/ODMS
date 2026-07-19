using FBSC.Common.Core.Queries;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using MediatR;
using FBSC.Common.Utility.Extensions;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Application.DTOs;
using LanguageExt;

namespace FBSC.ODMS.Application.Features.ODMS.DataSource.Queries;

public record GetDataSourceQuery : BaseQuery, IRequest<PagedListResponse<DataSourceListDto>>;

public class GetDataSourceQueryHandler(ApplicationContext context) : BaseQueryHandler<ApplicationContext, DataSourceListDto, GetDataSourceQuery>(context), IRequestHandler<GetDataSourceQuery, PagedListResponse<DataSourceListDto>>
{
	public override async Task<PagedListResponse<DataSourceListDto>> Handle(GetDataSourceQuery request, CancellationToken cancellationToken = default)
	{
		var pagedList = Context.Set<DataSourceState>()
			.AsNoTracking().Select(e => new DataSourceListDto()
			{
				Id = e.Id,
				LastModifiedDate = e.LastModifiedDate,
				Name = e.Name,
				ServerAddress = e.ServerAddress,
				DatabaseName = e.DatabaseName,
				AuthenticationType = e.AuthenticationType,
				Username = e.Username,
				Description = e.Description,
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
