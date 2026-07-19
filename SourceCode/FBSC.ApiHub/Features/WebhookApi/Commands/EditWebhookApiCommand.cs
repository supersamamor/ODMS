using AutoMapper;
using FBSC.ApiHub.Context;
using FBSC.ApiHub.Models;
using FBSC.Common.Core.Commands;
using FBSC.Common.Utility.Validators;
using FBSC.Common.Data;
using FluentValidation;
using LanguageExt;
using LanguageExt.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using static LanguageExt.Prelude;

namespace FBSC.ApiHub.Features.WebhookApi.Commands;

public record EditWebhookApiCommand : WebhookApiState, IRequest<Validation<Error, WebhookApiState>>;

public class EditWebhookApiCommandHandler(WebhookContext context,
                                 IMapper mapper,
                                 CompositeValidator<EditWebhookApiCommand> validator,
                                 IConfiguration configuration) : BaseCommandHandler<WebhookContext, WebhookApiState, EditWebhookApiCommand>(context, mapper, validator), IRequestHandler<EditWebhookApiCommand, Validation<Error, WebhookApiState>>
{
    public async Task<Validation<Error, WebhookApiState>> Handle(EditWebhookApiCommand request, CancellationToken cancellationToken) =>
        await Validators.ValidateTAsync(request, cancellationToken).BindT(
            async request => await EditWebhookApi(request, cancellationToken));


    public async Task<Validation<Error, WebhookApiState>> EditWebhookApi(EditWebhookApiCommand request, CancellationToken cancellationToken)
    {
        var entity = await Context.WebhookApi
            .Where(l => l.Id == request.Id).SingleAsync(cancellationToken: cancellationToken);
        string decryptedValue;
        try
        {
            decryptedValue = request.GetDecryptedClientSecret(configuration.GetValue<string>("EncryptionDecryptionKeyPrefix")!);
        }
        catch
        {
            decryptedValue = string.Empty;
        }
        if (string.IsNullOrEmpty(entity.ClientSecret) || decryptedValue != entity.GetDecryptedClientSecret(configuration.GetValue<string>("EncryptionDecryptionKeyPrefix")!))
        {
            request.EncryptClientSecret(configuration.GetValue<string>("EncryptionDecryptionKeyPrefix")!);
        }
        string decryptedHMACValue;
        try
        {
            decryptedHMACValue = request.GetDecryptedHMAC(configuration.GetValue<string>("EncryptionDecryptionKeyPrefix")!);
        }
        catch
        {
            decryptedHMACValue = string.Empty;
        }
        if (string.IsNullOrEmpty(entity.HMAC) || decryptedHMACValue != entity.GetDecryptedHMAC(configuration.GetValue<string>("EncryptionDecryptionKeyPrefix")!))
        {
            request.EncryptHMAC(configuration.GetValue<string>("EncryptionDecryptionKeyPrefix")!);
        }
        Mapper.Map(request, entity);
        await UpdateEntitySubCollectionAsync<WebhookApiState, WebhookEventAssignmentState>(request.Id, nameof(WebhookEventAssignmentState.WebhookApiId), nameof(request.WebhookEventAssignmentList), entity, cancellationToken);
        Context.Update(entity);
        _ = await Context.SaveChangesAsync(cancellationToken);
        return Success<Error, WebhookApiState>(entity);
    }

}

public class EditWebhookApiCommandValidator : AbstractValidator<EditWebhookApiCommand>
{
    readonly WebhookContext _context;

    public EditWebhookApiCommandValidator(WebhookContext context)
    {
        _context = context;
        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<WebhookApiState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("WebhookApi with id {PropertyValue} does not exists");
        RuleFor(x => x.Name).MustAsync(async (request, name, cancellation) => await _context.NotExists<WebhookApiState>(x => x.Name == name && x.Id != request.Id, cancellationToken: cancellation)).WithMessage("WebhookApi with name {PropertyValue} already exists");

    }
}
