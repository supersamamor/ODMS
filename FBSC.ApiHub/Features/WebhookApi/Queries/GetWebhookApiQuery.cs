using FBSC.ApiHub.Context;
using FBSC.ApiHub.Dtos;
using FBSC.Common.Core.Queries;
using FBSC.Common.Utility.Extensions;
using FBSC.Common.Utility.Models;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FBSC.ApiHub.Features.WebhookApi.Queries;

public record GetWebhookApiQuery : BaseQuery, IRequest<PagedListResponse<WebhookApiListDto>>;

public class GetWebhookApiQueryHandler(WebhookContext context) : BaseQueryHandler<WebhookContext, WebhookApiListDto, GetWebhookApiQuery>(context), IRequestHandler<GetWebhookApiQuery, PagedListResponse<WebhookApiListDto>>
{
	public override Task<PagedListResponse<WebhookApiListDto>> Handle(GetWebhookApiQuery request, CancellationToken cancellationToken = default)
	{
		return Task.FromResult(Context.WebhookApi
			.AsNoTracking().Select(e => new WebhookApiListDto()
			{
				Id = e.Id,
				ClientId = e.ClientId,
				LastModifiedDate = e.LastModifiedDate,
				Name = e.Name,
				GrantType = e.GrantType,
				WithinPrivateNetwork = e.WithinPrivateNetwork,			
				ClientSecret = e.ClientSecret,
				Scope = e.Scope,
				BaseUrl = e.BaseUrl,
				AuthenticationUrl = e.AuthenticationUrl,				
			})
			.ToPagedResponse(request.SearchColumns, request.SearchValue,
				request.SortColumn, request.SortOrder,
				request.PageNumber, request.PageSize));
	}	
}
