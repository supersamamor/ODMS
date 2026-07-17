using MediatR;
using Microsoft.EntityFrameworkCore;
using FBSC.ApiHub.Dtos;
using LanguageExt;
using FBSC.ApiHub.Context;
using FBSC.Common.Core.Queries;
using FBSC.Common.Utility.Models;
using FBSC.Common.Utility.Extensions;

namespace FBSC.ApiHub.Features.WebhookEventAssignment.Queries;

public record GetWebhookEventAssignmentQuery : BaseQuery, IRequest<PagedListResponse<WebhookEventAssignmentListDto>>;

public class GetWebhookEventAssignmentQueryHandler(WebhookContext context) : BaseQueryHandler<WebhookContext, WebhookEventAssignmentListDto, GetWebhookEventAssignmentQuery>(context), IRequestHandler<GetWebhookEventAssignmentQuery, PagedListResponse<WebhookEventAssignmentListDto>>
{
	public override Task<PagedListResponse<WebhookEventAssignmentListDto>> Handle(GetWebhookEventAssignmentQuery request, CancellationToken cancellationToken = default)
	{
		return Task.FromResult(Context.WebhookEventAssignment.Include(l=>l.WebhookApi)
			.AsNoTracking().Select(e => new WebhookEventAssignmentListDto()
			{
				Id = e.Id,
				LastModifiedDate = e.LastModifiedDate,
				WebhookApiId = e.WebhookApi == null ? "" : e.WebhookApi!.Name,
				EventName = e.EventName,
				Route = e.Route,
				Method = e.Method,
			})
			.ToPagedResponse(request.SearchColumns, request.SearchValue,
				request.SortColumn, request.SortOrder,
				request.PageNumber, request.PageSize));
	}	
}
