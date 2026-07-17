using FBSC.Common.Core.Queries;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FBSC.ODMS.Application.Features.ODMS.AiSqlGenerationRequest.Queries;

public record GetAiSqlGenerationRequestByIdQuery(string Id) : BaseQueryById(Id), IRequest<Option<AiSqlGenerationRequestState>>;

public class GetAiSqlGenerationRequestByIdQueryHandler(ApplicationContext context) : BaseQueryByIdHandler<ApplicationContext, AiSqlGenerationRequestState, GetAiSqlGenerationRequestByIdQuery>(context), IRequestHandler<GetAiSqlGenerationRequestByIdQuery, Option<AiSqlGenerationRequestState>>
{
	
	public override async Task<Option<AiSqlGenerationRequestState>> Handle(GetAiSqlGenerationRequestByIdQuery request, CancellationToken cancellationToken = default)
	{
		return await Context.AiSqlGenerationRequest.Include(l=>l.DashboardQuery).Include(l=>l.DataSource)
			.Where(e => e.Id == request.Id).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
	}
	
}
