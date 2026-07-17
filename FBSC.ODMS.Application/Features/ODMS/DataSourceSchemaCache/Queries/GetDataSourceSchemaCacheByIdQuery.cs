using FBSC.Common.Core.Queries;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FBSC.ODMS.Application.Features.ODMS.DataSourceSchemaCache.Queries;

public record GetDataSourceSchemaCacheByIdQuery(string Id) : BaseQueryById(Id), IRequest<Option<DataSourceSchemaCacheState>>;

public class GetDataSourceSchemaCacheByIdQueryHandler(ApplicationContext context) : BaseQueryByIdHandler<ApplicationContext, DataSourceSchemaCacheState, GetDataSourceSchemaCacheByIdQuery>(context), IRequestHandler<GetDataSourceSchemaCacheByIdQuery, Option<DataSourceSchemaCacheState>>
{
	
	public override async Task<Option<DataSourceSchemaCacheState>> Handle(GetDataSourceSchemaCacheByIdQuery request, CancellationToken cancellationToken = default)
	{
		return await Context.DataSourceSchemaCache.Include(l=>l.DataSource)
			.Where(e => e.Id == request.Id).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
	}
	
}
