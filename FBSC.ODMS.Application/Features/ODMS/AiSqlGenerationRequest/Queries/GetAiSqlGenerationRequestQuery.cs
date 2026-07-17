using FBSC.Common.Core.Queries;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using MediatR;
using FBSC.Common.Utility.Extensions;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Application.DTOs;
using LanguageExt;

namespace FBSC.ODMS.Application.Features.ODMS.AiSqlGenerationRequest.Queries;

public record GetAiSqlGenerationRequestQuery : BaseQuery, IRequest<PagedListResponse<AiSqlGenerationRequestListDto>>;

public class GetAiSqlGenerationRequestQueryHandler(ApplicationContext context) : BaseQueryHandler<ApplicationContext, AiSqlGenerationRequestListDto, GetAiSqlGenerationRequestQuery>(context), IRequestHandler<GetAiSqlGenerationRequestQuery, PagedListResponse<AiSqlGenerationRequestListDto>>
{
	public override Task<PagedListResponse<AiSqlGenerationRequestListDto>> Handle(GetAiSqlGenerationRequestQuery request, CancellationToken cancellationToken = default)
	{
		return Task.FromResult(Context.Set<AiSqlGenerationRequestState>().Include(l=>l.DashboardQuery).Include(l=>l.DataSource)
			.AsNoTracking().Select(e => new AiSqlGenerationRequestListDto()
			{
				Id = e.Id,
				LastModifiedDate = e.LastModifiedDate,
				DataSourceId = e.DataSource == null ? "" : e.DataSource!.Name,
				NaturalLanguagePrompt = e.NaturalLanguagePrompt,
				DashboardQueryId = e.DashboardQuery == null ? "" : e.DashboardQuery!.QueryHash,
				ConfidenceScore = e.ConfidenceScore,
				ValidationStatus = e.ValidationStatus,
				ErrorRemarks = e.ErrorRemarks,
				RequestedBy = e.RequestedBy,
				GeneratedAt = e.GeneratedAt,
			})
			.ToPagedResponse(request.SearchColumns, request.SearchValue,
				request.SortColumn, request.SortOrder,
				request.PageNumber, request.PageSize));
	}	
}
