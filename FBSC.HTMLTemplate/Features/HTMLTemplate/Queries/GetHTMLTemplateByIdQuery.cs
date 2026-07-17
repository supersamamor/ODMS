using FBSC.Common.Core.Queries;
using FBSC.HTMLTemplate.Models;
using FBSC.HTMLTemplate.Context;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FBSC.HTMLTemplate.Features.HTMLTemplate.Queries;

public record GetHTMLTemplateByIdQuery(string Id) : BaseQueryById(Id), IRequest<Option<HTMLTemplateState>>;

public class GetHTMLTemplateByIdQueryHandler(HTMLTemplateContext context) : BaseQueryByIdHandler<HTMLTemplateContext, HTMLTemplateState, GetHTMLTemplateByIdQuery>(context), IRequestHandler<GetHTMLTemplateByIdQuery, Option<HTMLTemplateState>>
{
		
}
