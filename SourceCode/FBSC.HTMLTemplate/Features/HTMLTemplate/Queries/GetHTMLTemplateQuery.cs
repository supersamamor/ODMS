using FBSC.Common.Core.Queries;
using FBSC.Common.Utility.Models;
using MediatR;
using FBSC.Common.Utility.Extensions;
using Microsoft.EntityFrameworkCore;
using FBSC.HTMLTemplate.Dtos;
using LanguageExt;
using FBSC.HTMLTemplate.Context;
using FBSC.HTMLTemplate.Models;

namespace FBSC.HTMLTemplate.Features.HTMLTemplate.Queries;

public record GetHTMLTemplateQuery : BaseQuery, IRequest<PagedListResponse<HTMLTemplateListDto>>;

public class GetHTMLTemplateQueryHandler(HTMLTemplateContext context) : BaseQueryHandler<HTMLTemplateContext, HTMLTemplateListDto, GetHTMLTemplateQuery>(context), IRequestHandler<GetHTMLTemplateQuery, PagedListResponse<HTMLTemplateListDto>>
{
	public override async Task<PagedListResponse<HTMLTemplateListDto>> Handle(GetHTMLTemplateQuery request, CancellationToken cancellationToken = default)
	{
        return Context.Set<HTMLTemplateState>()
			.AsNoTracking().Select(e => new HTMLTemplateListDto()
			{
				Id = e.Id,
				LastModifiedDate = e.LastModifiedDate,
				HTMLTemplateName = e.HTMLTemplateName,		
			})
			.ToPagedResponse(request.SearchColumns, request.SearchValue,
				request.SortColumn, request.SortOrder,
				request.PageNumber, request.PageSize);	
	}	
}
