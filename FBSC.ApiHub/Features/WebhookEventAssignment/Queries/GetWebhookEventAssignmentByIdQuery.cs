using FBSC.Common.Core.Queries;
using FBSC.ApiHub.Context;
using FBSC.ApiHub.Models;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FBSC.ApiHub.Features.WebhookEventAssignment.Queries;

public record GetWebhookEventAssignmentByIdQuery(string Id) : BaseQueryById(Id), IRequest<Option<WebhookEventAssignmentState>>;

public class GetWebhookEventAssignmentByIdQueryHandler(WebhookContext context) : BaseQueryByIdHandler<WebhookContext, WebhookEventAssignmentState, GetWebhookEventAssignmentByIdQuery>(context), IRequestHandler<GetWebhookEventAssignmentByIdQuery, Option<WebhookEventAssignmentState>>
{
	
	public override async Task<Option<WebhookEventAssignmentState>> Handle(GetWebhookEventAssignmentByIdQuery request, CancellationToken cancellationToken = default)
	{
		return await Context.WebhookEventAssignment.Include(l=>l.WebhookApi)
			.Where(e => e.Id == request.Id).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
	}
}
