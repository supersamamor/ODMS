using FBSC.Common.Core.Queries;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FBSC.ODMS.Application.Features.ODMS.DataSource.Queries;

public record GetDataSourceByIdQuery(string Id) : BaseQueryById(Id), IRequest<Option<DataSourceState>>;

public class GetDataSourceByIdQueryHandler(ApplicationContext context) : BaseQueryByIdHandler<ApplicationContext, DataSourceState, GetDataSourceByIdQuery>(context), IRequestHandler<GetDataSourceByIdQuery, Option<DataSourceState>>
{
		
}
