using FBSC.Common.Core.Queries;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FBSC.ODMS.Application.Features.ODMS.DataUploadColumn.Queries;

public record GetDataUploadColumnByIdQuery(string Id) : BaseQueryById(Id), IRequest<Option<DataUploadColumnState>>;

public class GetDataUploadColumnByIdQueryHandler(ApplicationContext context) : BaseQueryByIdHandler<ApplicationContext, DataUploadColumnState, GetDataUploadColumnByIdQuery>(context), IRequestHandler<GetDataUploadColumnByIdQuery, Option<DataUploadColumnState>>
{
	
	public override async Task<Option<DataUploadColumnState>> Handle(GetDataUploadColumnByIdQuery request, CancellationToken cancellationToken = default)
	{
		return await Context.DataUploadColumn.Include(l=>l.DataUploadBatch)
			.Where(e => e.Id == request.Id).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
	}
	
}
