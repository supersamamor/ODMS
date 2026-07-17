using FBSC.Common.Core.Queries;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using MediatR;
using FBSC.Common.Utility.Extensions;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Application.DTOs;
using LanguageExt;

namespace FBSC.ODMS.Application.Features.ODMS.AiSqlPromptTemplate.Queries;

public record GetAiSqlPromptTemplateQuery : BaseQuery, IRequest<PagedListResponse<AiSqlPromptTemplateListDto>>;

public class GetAiSqlPromptTemplateQueryHandler(ApplicationContext context) : BaseQueryHandler<ApplicationContext, AiSqlPromptTemplateListDto, GetAiSqlPromptTemplateQuery>(context), IRequestHandler<GetAiSqlPromptTemplateQuery, PagedListResponse<AiSqlPromptTemplateListDto>>
{
	public override Task<PagedListResponse<AiSqlPromptTemplateListDto>> Handle(GetAiSqlPromptTemplateQuery request, CancellationToken cancellationToken = default)
	{
		return Task.FromResult(Context.Set<AiSqlPromptTemplateState>()
			.AsNoTracking().Select(e => new AiSqlPromptTemplateListDto()
			{
				Id = e.Id,
				LastModifiedDate = e.LastModifiedDate,
				SystemType = e.SystemType,
				PromptTemplate = e.PromptTemplate,
				Sequence = e.Sequence,
				IsActive = e.IsActive,
			})
			.ToPagedResponse(request.SearchColumns, request.SearchValue,
				request.SortColumn, request.SortOrder,
				request.PageNumber, request.PageSize));
	}	
}
