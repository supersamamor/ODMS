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

namespace FBSC.ApiHub.Features.WebhookEventAssignment.Commands;

public record DeleteWebhookEventAssignmentCommand : BaseCommand, IRequest<Validation<Error, WebhookEventAssignmentState>>;

public class DeleteWebhookEventAssignmentCommandHandler(WebhookContext context,
                                   IMapper mapper,
                                   CompositeValidator<DeleteWebhookEventAssignmentCommand> validator) : BaseCommandHandler<WebhookContext, WebhookEventAssignmentState, DeleteWebhookEventAssignmentCommand>(context, mapper, validator), IRequestHandler<DeleteWebhookEventAssignmentCommand, Validation<Error, WebhookEventAssignmentState>>
{ 
    public async Task<Validation<Error, WebhookEventAssignmentState>> Handle(DeleteWebhookEventAssignmentCommand request, CancellationToken cancellationToken) =>
        await Validators.ValidateTAsync(request, cancellationToken).BindT(
            async request => await Delete(request.Id, cancellationToken));
}


public class DeleteWebhookEventAssignmentCommandValidator : AbstractValidator<DeleteWebhookEventAssignmentCommand>
{
    readonly WebhookContext _context;

    public DeleteWebhookEventAssignmentCommandValidator(WebhookContext context)
    {
        _context = context;
        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<WebhookEventAssignmentState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("WebhookEventAssignment with id {PropertyValue} does not exists");
    }
}
