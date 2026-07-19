using LanguageExt;
using LanguageExt.Common;
using MediatR;
using static LanguageExt.Prelude;
using Microsoft.EntityFrameworkCore;
using FBSC.ApiHub.Context;
namespace FBSC.ApiHub.Features.WebhookLogs.Commands;
public record ResendWebhookLogsCommand(string Id) : IRequest<Validation<Error, bool>>;
public class ResendWebhookLogsCommandHandler(WebhookContext context) : IRequestHandler<ResendWebhookLogsCommand, Validation<Error, bool>>
{
    public async Task<Validation<Error, bool>> Handle(ResendWebhookLogsCommand request, CancellationToken cancellationToken) =>
        await Resend(request, cancellationToken);

    public async Task<Validation<Error, bool>> Resend(ResendWebhookLogsCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.WebhookLogs.Where(l => l.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
        entity!.TagAsPending();
        context.Update(entity);
        await context.SaveChangesAsync(cancellationToken);
        return Success<Error, bool>(true);
    }    
}
