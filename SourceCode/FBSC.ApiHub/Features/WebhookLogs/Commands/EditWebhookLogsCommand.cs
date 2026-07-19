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

namespace FBSC.ApiHub.Features.WebhookLogs.Commands;

public record EditWebhookLogsCommand : WebhookLogsState, IRequest<Validation<Error, WebhookLogsState>>;

public class EditWebhookLogsCommandHandler(WebhookContext context,
                                 IMapper mapper,
                                 CompositeValidator<EditWebhookLogsCommand> validator) : BaseCommandHandler<WebhookContext, WebhookLogsState, EditWebhookLogsCommand>(context, mapper, validator), IRequestHandler<EditWebhookLogsCommand, Validation<Error, WebhookLogsState>>
{ 
    
public async Task<Validation<Error, WebhookLogsState>> Handle(EditWebhookLogsCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await Edit(request, cancellationToken));
	
}

public class EditWebhookLogsCommandValidator : AbstractValidator<EditWebhookLogsCommand>
{
    readonly WebhookContext _context;

    public EditWebhookLogsCommandValidator(WebhookContext context)
    {
        _context = context;
		RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<WebhookLogsState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("WebhookLogs with id {PropertyValue} does not exists");
        
    }
}
