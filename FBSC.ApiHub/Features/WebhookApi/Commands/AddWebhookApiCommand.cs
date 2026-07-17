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
using Microsoft.Extensions.Configuration;
using static LanguageExt.Prelude;

namespace FBSC.ApiHub.Features.WebhookApi.Commands;

public record AddWebhookApiCommand : WebhookApiState, IRequest<Validation<Error, WebhookApiState>>;

public class AddWebhookApiCommandHandler(WebhookContext context,
                                IMapper mapper,
                                CompositeValidator<AddWebhookApiCommand> validator,
                                IConfiguration configuration) : BaseCommandHandler<WebhookContext, WebhookApiState, AddWebhookApiCommand>(context, mapper, validator), IRequestHandler<AddWebhookApiCommand, Validation<Error, WebhookApiState>>
{
    public async Task<Validation<Error, WebhookApiState>> Handle(AddWebhookApiCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await AddWebhookApi(request, cancellationToken));


	public async Task<Validation<Error, WebhookApiState>> AddWebhookApi(AddWebhookApiCommand request, CancellationToken cancellationToken)
	{
		WebhookApiState entity = Mapper.Map<WebhookApiState>(request);    
        entity.EncryptHMAC(configuration.GetValue<string>("EncryptionDecryptionKeyPrefix")!);
        entity.EncryptClientSecret(configuration.GetValue<string>("EncryptionDecryptionKeyPrefix")!);
        AddEntitySubCollection<WebhookApiState, WebhookEventAssignmentState>(entity, nameof(request.WebhookEventAssignmentList));
		_ = await Context.AddAsync(entity, cancellationToken);
		_ = await Context.SaveChangesAsync(cancellationToken);
		return Success<Error, WebhookApiState>(entity);
	}
	
}

public class AddWebhookApiCommandValidator : AbstractValidator<AddWebhookApiCommand>
{
    readonly WebhookContext _context;

    public AddWebhookApiCommandValidator(WebhookContext context)
    {
        _context = context;

        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.NotExists<WebhookApiState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("WebhookApi with id {PropertyValue} already exists");
        RuleFor(x => x.Name).MustAsync(async (name, cancellation) => await _context.NotExists<WebhookApiState>(x => x.Name == name, cancellationToken: cancellation)).WithMessage("WebhookApi with name {PropertyValue} already exists");
	
    }
}
