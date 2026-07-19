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

public record EditWebhookEventAssignmentCommand : WebhookEventAssignmentState, IRequest<Validation<Error, WebhookEventAssignmentState>>;

public class EditWebhookEventAssignmentCommandHandler(WebhookContext context,
                                 IMapper mapper,
                                 CompositeValidator<EditWebhookEventAssignmentCommand> validator) : BaseCommandHandler<WebhookContext, WebhookEventAssignmentState, EditWebhookEventAssignmentCommand>(context, mapper, validator), IRequestHandler<EditWebhookEventAssignmentCommand, Validation<Error, WebhookEventAssignmentState>>
{ 
    
public async Task<Validation<Error, WebhookEventAssignmentState>> Handle(EditWebhookEventAssignmentCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await Edit(request, cancellationToken));
	
}

public class EditWebhookEventAssignmentCommandValidator : AbstractValidator<EditWebhookEventAssignmentCommand>
{
    readonly WebhookContext _context;

    public EditWebhookEventAssignmentCommandValidator(WebhookContext context)
    {
        _context = context;
		RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<WebhookEventAssignmentState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("WebhookEventAssignment with id {PropertyValue} does not exists");        
    }
}
