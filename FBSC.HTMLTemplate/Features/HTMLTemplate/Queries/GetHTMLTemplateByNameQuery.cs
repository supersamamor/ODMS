using FBSC.HTMLTemplate.Context;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FBSC.HTMLTemplate.Models;
namespace FBSC.HTMLTemplate.Features.HTMLTemplate.Queries;

public record GetHTMLTemplateByNameQuery(string Name) : IRequest<Option<HTMLTemplateState>>;

public class GetHTMLTemplateByNameQueryHandler(HTMLTemplateContext context) : IRequestHandler<GetHTMLTemplateByNameQuery, Option<HTMLTemplateState>>
{
    public async Task<Option<HTMLTemplateState>> Handle(GetHTMLTemplateByNameQuery request, CancellationToken cancellationToken = default)
    {
        return await context.HTMLTemplate.AsNoTracking().Where(l => l.HTMLTemplateName == request.Name).FirstOrDefaultAsync(cancellationToken);
    }
}
