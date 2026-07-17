using FBSC.Common.Core.Queries;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using MediatR;
using FBSC.Common.Utility.Extensions;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Application.DTOs;
using LanguageExt;

namespace FBSC.ODMS.Application.Features.ODMS.DashboardTheme.Queries;

public record GetDashboardThemeQuery : BaseQuery, IRequest<PagedListResponse<DashboardThemeListDto>>;

public class GetDashboardThemeQueryHandler(ApplicationContext context) : BaseQueryHandler<ApplicationContext, DashboardThemeListDto, GetDashboardThemeQuery>(context), IRequestHandler<GetDashboardThemeQuery, PagedListResponse<DashboardThemeListDto>>
{
	public override Task<PagedListResponse<DashboardThemeListDto>> Handle(GetDashboardThemeQuery request, CancellationToken cancellationToken = default)
	{
		return Task.FromResult(Context.Set<DashboardThemeState>()
			.AsNoTracking().Select(e => new DashboardThemeListDto()
			{
				Id = e.Id,
				LastModifiedDate = e.LastModifiedDate,
				Code = e.Code,
				Name = e.Name,
				IsDarkMode = e.IsDarkMode,
				PrimaryColorHex = e.PrimaryColorHex,
				GenerationAlgorithm = e.GenerationAlgorithm,
				IsSystemDefault = e.IsSystemDefault,
			})
			.ToPagedResponse(request.SearchColumns, request.SearchValue,
				request.SortColumn, request.SortOrder,
				request.PageNumber, request.PageSize));
	}	
}
