using AutoMapper;
using FBSC.Common.Core.Commands;
using FBSC.Common.Data;
using FBSC.Common.Utility.Validators;
using FBSC.ODMS.Core.Constants;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using FBSC.ODMS.Infrastructure.Extensions;
using FluentValidation;
using LanguageExt;
using LanguageExt.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using static LanguageExt.Prelude;

namespace FBSC.ODMS.Application.Features.ODMS.DataSource.Commands;

public record EditDataSourceCommand : DataSourceState, IRequest<Validation<Error, DataSourceState>>;

public class EditDataSourceCommandHandler(ApplicationContext context,
                                 IMapper mapper,
                                 CompositeValidator<EditDataSourceCommand> validator,
                                 IConfiguration configuration) : BaseCommandHandler<ApplicationContext, DataSourceState, EditDataSourceCommand>(context, mapper, validator), IRequestHandler<EditDataSourceCommand, Validation<Error, DataSourceState>>
{
    public async Task<Validation<Error, DataSourceState>> Handle(EditDataSourceCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await EditDataSource(request, cancellationToken));


	public async Task<Validation<Error, DataSourceState>> EditDataSource(EditDataSourceCommand request, CancellationToken cancellationToken)
	{
		var entity = await Context.DataSource.Where(l => l.Id == request.Id).SingleAsync(cancellationToken: cancellationToken);
		// The edit form never redisplays a saved password/connection-string, so a blank
		// submission always means "keep the current value"; only a non-blank submission is a
		// new plaintext secret that needs encrypting (same at-rest mechanism as WebhookApiState).
		var requestWithNewSecretsOnly = (request with
		{
			PasswordEncrypted = string.IsNullOrEmpty(request.PasswordEncrypted) ? null : request.PasswordEncrypted,
			ConnectionStringEncrypted = string.IsNullOrEmpty(request.ConnectionStringEncrypted) ? null : request.ConnectionStringEncrypted,
		}).EncryptSecrets(configuration.GetValue<string>("EncryptionDecryptionKeyPrefix")!);

		var requestToMap = request with
		{
			PasswordEncrypted = requestWithNewSecretsOnly.PasswordEncrypted ?? entity.PasswordEncrypted,
			ConnectionStringEncrypted = requestWithNewSecretsOnly.ConnectionStringEncrypted ?? entity.ConnectionStringEncrypted,
		};
		Mapper.Map(requestToMap, entity);
		await UpdateEntitySubCollectionAsync<DataSourceState, DataSourceSchemaCacheState>(request.Id, nameof(DataSourceSchemaCacheState.DataSourceId), nameof(request.DataSourceSchemaCacheList), entity, cancellationToken);
		Context.Update(entity);
		_ = await Context.SaveChangesAsync(cancellationToken);
		return Success<Error, DataSourceState>(entity);
	}

}

public class EditDataSourceCommandValidator : AbstractValidator<EditDataSourceCommand>
{
    readonly ApplicationContext _context;

    public EditDataSourceCommandValidator(ApplicationContext context)
    {
        _context = context;
		RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<DataSourceState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("DataSource with id {PropertyValue} does not exists");
        RuleFor(x => x.Name).MustAsync(async (request, name, cancellation) => await _context.NotExists<DataSourceState>(x => x.Name == name && x.Id != request.Id, cancellationToken: cancellation)).WithMessage("DataSource with name {PropertyValue} already exists");
        RuleFor(x => x.ConnectionKind).Must(k => k is DataSourceConnectionKind.ExternalDatabase or DataSourceConnectionKind.UploadedFile)
                          .WithMessage("Connection kind must be either ExternalDatabase or UploadedFile");

        // Note: PasswordEncrypted/ConnectionStringEncrypted are deliberately NOT validated
        // here - a blank submission on Edit means "keep the current secret" (see
        // EditDataSourceCommandHandler), so an empty value here isn't necessarily invalid.
        When(x => x.ConnectionKind == DataSourceConnectionKind.ExternalDatabase, () =>
        {
            RuleFor(x => x.ConnectionMode).NotEmpty().WithMessage("Connection mode is required for a database connection");
            When(x => x.ConnectionMode == DataSourceConnectionMode.ServerCredentials, () =>
            {
                RuleFor(x => x.ServerAddress).NotEmpty().WithMessage("Server address is required for a server-credentials connection");
                RuleFor(x => x.DatabaseName).NotEmpty().WithMessage("Database name is required for a server-credentials connection");
                RuleFor(x => x.AuthenticationType).NotEmpty().WithMessage("Authentication type is required for a server-credentials connection");
                When(x => x.AuthenticationType == DataSourceAuthenticationType.SqlServer, () =>
                {
                    RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required for SQL Server authentication");
                });
            });
        });
    }
}
