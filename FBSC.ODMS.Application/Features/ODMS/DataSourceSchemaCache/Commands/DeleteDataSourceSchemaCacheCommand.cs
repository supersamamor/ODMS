using AutoMapper;
using FBSC.Common.Core.Commands;
using FBSC.Common.Data;
using FBSC.Common.Utility.Validators;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using FluentValidation;
using LanguageExt;
using LanguageExt.Common;
using MediatR;

namespace FBSC.ODMS.Application.Features.ODMS.DataSourceSchemaCache.Commands;

public record DeleteDataSourceSchemaCacheCommand : BaseCommand, IRequest<Validation<Error, DataSourceSchemaCacheState>>;

public class DeleteDataSourceSchemaCacheCommandHandler(ApplicationContext context,
                                   IMapper mapper,
                                   CompositeValidator<DeleteDataSourceSchemaCacheCommand> validator) : BaseCommandHandler<ApplicationContext, DataSourceSchemaCacheState, DeleteDataSourceSchemaCacheCommand>(context, mapper, validator), IRequestHandler<DeleteDataSourceSchemaCacheCommand, Validation<Error, DataSourceSchemaCacheState>>
{ 
    public async Task<Validation<Error, DataSourceSchemaCacheState>> Handle(DeleteDataSourceSchemaCacheCommand request, CancellationToken cancellationToken) =>
        await Validators.ValidateTAsync(request, cancellationToken).BindT(
            async request => await Delete(request.Id, cancellationToken));
}


public class DeleteDataSourceSchemaCacheCommandValidator : AbstractValidator<DeleteDataSourceSchemaCacheCommand>
{
    readonly ApplicationContext _context;

    public DeleteDataSourceSchemaCacheCommandValidator(ApplicationContext context)
    {
        _context = context;
        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<DataSourceSchemaCacheState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("DataSourceSchemaCache with id {PropertyValue} does not exists");
    }
}
