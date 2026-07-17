using FBSC.ApiHub.Context;
using FBSC.ApiHub.Models;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FBSC.Common.Core.Queries;

namespace FBSC.ApiHub.Features.WebhookApi.Queries;

public record GetWebhookApiByIdQuery(string Id) : BaseQueryById(Id), IRequest<Option<WebhookApiState>>;

public class GetWebhookApiByIdQueryHandler(WebhookContext context) : BaseQueryByIdHandler<WebhookContext, WebhookApiState, GetWebhookApiByIdQuery>(context), IRequestHandler<GetWebhookApiByIdQuery, Option<WebhookApiState>>
{
	
	public override async Task<Option<WebhookApiState>> Handle(GetWebhookApiByIdQuery request, CancellationToken cancellationToken = default)
	{
		return await Context.WebhookApi
			.Include(l=>l.WebhookEventAssignmentList)
			.Where(e => e.Id == request.Id).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
	}
	
}
