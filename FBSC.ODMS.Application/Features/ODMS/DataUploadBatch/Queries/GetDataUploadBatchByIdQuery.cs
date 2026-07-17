using FBSC.Common.Core.Queries;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FBSC.ODMS.Application.Features.ODMS.DataUploadBatch.Queries;

public record GetDataUploadBatchByIdQuery(string Id) : BaseQueryById(Id), IRequest<Option<DataUploadBatchState>>;

public class GetDataUploadBatchByIdQueryHandler(ApplicationContext context) : BaseQueryByIdHandler<ApplicationContext, DataUploadBatchState, GetDataUploadBatchByIdQuery>(context), IRequestHandler<GetDataUploadBatchByIdQuery, Option<DataUploadBatchState>>
{
	
	public override async Task<Option<DataUploadBatchState>> Handle(GetDataUploadBatchByIdQuery request, CancellationToken cancellationToken = default)
	{
		return await Context.DataUploadBatch.Include(l=>l.DataSource)
			.Include(l=>l.DataUploadColumnList)
			.Where(e => e.Id == request.Id).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
	}
	
}
