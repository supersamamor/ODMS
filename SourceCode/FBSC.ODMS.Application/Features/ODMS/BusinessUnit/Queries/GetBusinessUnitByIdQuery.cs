using FBSC.Common.Core.Queries;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FBSC.ODMS.Application.Features.ODMS.BusinessUnit.Queries;

public record GetBusinessUnitByIdQuery(string Id) : BaseQueryById(Id), IRequest<Option<BusinessUnitState>>;

public class GetBusinessUnitByIdQueryHandler(ApplicationContext context) : BaseQueryByIdHandler<ApplicationContext, BusinessUnitState, GetBusinessUnitByIdQuery>(context), IRequestHandler<GetBusinessUnitByIdQuery, Option<BusinessUnitState>>
{
		
}
