using FBSC.Common.Core.Queries;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using MediatR;
using FBSC.Common.Utility.Extensions;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Application.DTOs;
using LanguageExt;

namespace FBSC.ODMS.Application.Features.ODMS.BusinessUnit.Queries;

public record GetBusinessUnitQuery : BaseQuery, IRequest<PagedListResponse<BusinessUnitListDto>>;

public class GetBusinessUnitQueryHandler(ApplicationContext context) : BaseQueryHandler<ApplicationContext, BusinessUnitListDto, GetBusinessUnitQuery>(context), IRequestHandler<GetBusinessUnitQuery, PagedListResponse<BusinessUnitListDto>>
{
	public override Task<PagedListResponse<BusinessUnitListDto>> Handle(GetBusinessUnitQuery request, CancellationToken cancellationToken = default)
	{
		return Task.FromResult(Context.Set<BusinessUnitState>()
			.AsNoTracking().Select(e => new BusinessUnitListDto()
			{
				Id = e.Id,
				LastModifiedDate = e.LastModifiedDate,
				Name = e.Name,
			})
			.ToPagedResponse(request.SearchColumns, request.SearchValue,
				request.SortColumn, request.SortOrder,
				request.PageNumber, request.PageSize));
	}	
}
