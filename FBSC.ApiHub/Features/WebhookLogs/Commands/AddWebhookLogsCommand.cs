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

public record AddWebhookLogsCommand : WebhookLogsState, IRequest<Validation<Error, WebhookLogsState>>;

public class AddWebhookLogsCommandHandler(WebhookContext context,
                                IMapper mapper,
                                CompositeValidator<AddWebhookLogsCommand> validator) : BaseCommandHandler<WebhookContext, WebhookLogsState, AddWebhookLogsCommand>(context, mapper, validator), IRequestHandler<AddWebhookLogsCommand, Validation<Error, WebhookLogsState>>
{
    
public async Task<Validation<Error, WebhookLogsState>> Handle(AddWebhookLogsCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await Add(request, cancellationToken));
	
}

public class AddWebhookLogsCommandValidator : AbstractValidator<AddWebhookLogsCommand>
{
    readonly WebhookContext _context;

    public AddWebhookLogsCommandValidator(WebhookContext context)
    {
        _context = context;

        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.NotExists<WebhookLogsState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("WebhookLogs with id {PropertyValue} already exists");        
    }
}
