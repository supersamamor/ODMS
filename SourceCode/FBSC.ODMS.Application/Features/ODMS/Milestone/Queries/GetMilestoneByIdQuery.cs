using FBSC.Common.Core.Queries;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FBSC.ODMS.Application.Features.ODMS.Milestone.Queries;

public record GetMilestoneByIdQuery(string Id) : BaseQueryById(Id), IRequest<Option<MilestoneState>>;

public class GetMilestoneByIdQueryHandler(ApplicationContext context) : BaseQueryByIdHandler<ApplicationContext, MilestoneState, GetMilestoneByIdQuery>(context), IRequestHandler<GetMilestoneByIdQuery, Option<MilestoneState>>
{
	
	public override async Task<Option<MilestoneState>> Handle(GetMilestoneByIdQuery request, CancellationToken cancellationToken = default)
	{
		return await Context.Milestone		
			.Where(e => e.Id == request.Id).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
	}
	
}
