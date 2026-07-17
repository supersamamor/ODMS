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

public record AddWebhookEventAssignmentCommand : WebhookEventAssignmentState, IRequest<Validation<Error, WebhookEventAssignmentState>>;

public class AddWebhookEventAssignmentCommandHandler(WebhookContext context,
                                IMapper mapper,
                                CompositeValidator<AddWebhookEventAssignmentCommand> validator) : BaseCommandHandler<WebhookContext, WebhookEventAssignmentState, AddWebhookEventAssignmentCommand>(context, mapper, validator), IRequestHandler<AddWebhookEventAssignmentCommand, Validation<Error, WebhookEventAssignmentState>>
{
    
public async Task<Validation<Error, WebhookEventAssignmentState>> Handle(AddWebhookEventAssignmentCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await Add(request, cancellationToken));
	
}

public class AddWebhookEventAssignmentCommandValidator : AbstractValidator<AddWebhookEventAssignmentCommand>
{
    readonly WebhookContext _context;

    public AddWebhookEventAssignmentCommandValidator(WebhookContext context)
    {
        _context = context;

        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.NotExists<WebhookEventAssignmentState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("WebhookEventAssignment with id {PropertyValue} already exists");
        
    }
}
