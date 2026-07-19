using FBSC.Common.Core.Queries;
using FBSC.ApiHub.Context;
using FBSC.ApiHub.Models;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FBSC.ApiHub.Features.WebhookLogs.Queries;

public record GetWebhookLogsByIdQuery(string Id) : BaseQueryById(Id), IRequest<Option<WebhookLogsState>>;

public class GetWebhookLogsByIdQueryHandler(WebhookContext context) : BaseQueryByIdHandler<WebhookContext, WebhookLogsState, GetWebhookLogsByIdQuery>(context), IRequestHandler<GetWebhookLogsByIdQuery, Option<WebhookLogsState>>
{
	
	public override async Task<Option<WebhookLogsState>> Handle(GetWebhookLogsByIdQuery request, CancellationToken cancellationToken = default)
	{
		return await Context.WebhookLogs.Include(l=>l.WebhookEventAssignment).ThenInclude(l=>l!.WebhookApi)
			.Where(e => e.Id == request.Id).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
	}
	
}
