using FBSC.Common.Core.Queries;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using MediatR;
using FBSC.Common.Utility.Extensions;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Application.DTOs;
using LanguageExt;

namespace FBSC.ODMS.Application.Features.ODMS.Employee.Queries;

public record GetEmployeeQuery : BaseQuery, IRequest<PagedListResponse<EmployeeListDto>>;

public class GetEmployeeQueryHandler(ApplicationContext context) : BaseQueryHandler<ApplicationContext, EmployeeListDto, GetEmployeeQuery>(context), IRequestHandler<GetEmployeeQuery, PagedListResponse<EmployeeListDto>>
{
	public override Task<PagedListResponse<EmployeeListDto>> Handle(GetEmployeeQuery request, CancellationToken cancellationToken = default)
	{
		return Task.FromResult(Context.Set<EmployeeState>()
			.AsNoTracking().Select(e => new EmployeeListDto()
			{
				Id = e.Id,
				LastModifiedDate = e.LastModifiedDate,
				Department = e.Department,
				Email = e.Email,
				EmployeeCode = e.EmployeeCode,
				Name = e.Name,
				Position = e.Position,
				UserId = e.UserId,
			})
			.ToPagedResponse(request.SearchColumns, request.SearchValue,
				request.SortColumn, request.SortOrder,
				request.PageNumber, request.PageSize));
	}	
}
