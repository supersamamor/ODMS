using FBSC.Common.Core.Queries;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FBSC.ODMS.Application.Features.ODMS.Employee.Queries;

public record GetEmployeeByIdQuery(string Id) : BaseQueryById(Id), IRequest<Option<EmployeeState>>;

public class GetEmployeeByIdQueryHandler(ApplicationContext context) : BaseQueryByIdHandler<ApplicationContext, EmployeeState, GetEmployeeByIdQuery>(context), IRequestHandler<GetEmployeeByIdQuery, Option<EmployeeState>>
{
		
}
