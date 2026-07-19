using AutoMapper;
using FBSC.Common.Core.Commands;
using FBSC.Common.Data;
using FBSC.Common.Utility.Validators;
using FBSC.ApiHub.Context;
using FBSC.ApiHub.Models;
using FluentValidation;
using LanguageExt;
using LanguageExt.Common;
using MediatR;

namespace FBSC.ApiHub.Features.WebhookApi.Commands;

public record DeleteWebhookApiCommand : BaseCommand, IRequest<Validation<Error, WebhookApiState>>;

public class DeleteWebhookApiCommandHandler(WebhookContext context,
                                   IMapper mapper,
                                   CompositeValidator<DeleteWebhookApiCommand> validator) : BaseCommandHandler<WebhookContext, WebhookApiState, DeleteWebhookApiCommand>(context, mapper, validator), IRequestHandler<DeleteWebhookApiCommand, Validation<Error, WebhookApiState>>
{ 
    public async Task<Validation<Error, WebhookApiState>> Handle(DeleteWebhookApiCommand request, CancellationToken cancellationToken) =>
        await Validators.ValidateTAsync(request, cancellationToken).BindT(
            async request => await Delete(request.Id, cancellationToken));
}


public class DeleteWebhookApiCommandValidator : AbstractValidator<DeleteWebhookApiCommand>
{
    readonly WebhookContext _context;

    public DeleteWebhookApiCommandValidator(WebhookContext context)
    {
        _context = context;
        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<WebhookApiState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("WebhookApi with id {PropertyValue} does not exists");
    }
}
