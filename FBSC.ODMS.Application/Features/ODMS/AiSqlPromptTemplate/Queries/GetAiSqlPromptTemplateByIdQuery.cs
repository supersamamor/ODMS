using FBSC.Common.Core.Queries;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FBSC.ODMS.Application.Features.ODMS.AiSqlPromptTemplate.Queries;

public record GetAiSqlPromptTemplateByIdQuery(string Id) : BaseQueryById(Id), IRequest<Option<AiSqlPromptTemplateState>>;

public class GetAiSqlPromptTemplateByIdQueryHandler(ApplicationContext context) : BaseQueryByIdHandler<ApplicationContext, AiSqlPromptTemplateState, GetAiSqlPromptTemplateByIdQuery>(context), IRequestHandler<GetAiSqlPromptTemplateByIdQuery, Option<AiSqlPromptTemplateState>>
{
		
}
