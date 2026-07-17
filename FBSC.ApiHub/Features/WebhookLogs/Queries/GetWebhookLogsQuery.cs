using FBSC.ApiHub.Context;
using FBSC.ApiHub.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FBSC.ApiHub.Dtos;
using LanguageExt;
using FBSC.Common.Utility.Models;
using FBSC.Common.Core.Queries;
using FBSC.Common.Utility.Extensions;

namespace FBSC.ApiHub.Features.WebhookLogs.Queries;

public record GetWebhookLogsQuery : BaseQuery, IRequest<PagedListResponse<WebhookLogsListDto>>
{
    public string? WebhookApiId { get; set; }
}

public class GetWebhookLogsQueryHandler(WebhookContext context) : BaseQueryHandler<WebhookContext, WebhookLogsListDto, GetWebhookLogsQuery>(context), IRequestHandler<GetWebhookLogsQuery, PagedListResponse<WebhookLogsListDto>>
{
    public override Task<PagedListResponse<WebhookLogsListDto>> Handle(GetWebhookLogsQuery request, CancellationToken cancellationToken = default)
    {
        var query = Context.Set<WebhookLogsState>().Include(l => l.WebhookEventAssignment)
            .ThenInclude(l => l!.WebhookApi)
            .Where(l => l.Status == WebhookStatus.Failed).AsNoTracking();
        if (!string.IsNullOrEmpty(request.WebhookApiId))
        {
            query = query.Where(l => l.WebhookEventAssignment!.WebhookApiId == request.WebhookApiId);
        }
        return Task.FromResult(query.Select(e => new WebhookLogsListDto()
        {
            Id = e.Id,
            DataId = e.DataId,
            WebhookApiName = e.WebhookEventAssignment == null || e.WebhookEventAssignment.WebhookApi == null ? "" : e.WebhookEventAssignment.WebhookApi.Name,
            Event = e.WebhookEventAssignment == null ? "" : e.WebhookEventAssignment!.EventName,
            Status = e.Status,
            LastModifiedDate = e.LastModifiedDate,
            WebhookEventAssignmentId = e.WebhookEventAssignment == null ? "" : e.WebhookEventAssignment!.Id,
            DateTimeStarted = e.DateTimeStarted,
            DateTimeEnded = e.DateTimeEnded,          
            WebhookApiId = e.WebhookEventAssignment == null ? "" : e.WebhookEventAssignment!.WebhookApiId,
        })
        .ToPagedResponse(request.SearchColumns, request.SearchValue,
                request.SortColumn, request.SortOrder,
                request.PageNumber, request.PageSize));
    }
}
