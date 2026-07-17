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
using Microsoft.EntityFrameworkCore;
using static LanguageExt.Prelude;

namespace FBSC.ODMS.Application.Features.ODMS.DataSourceSchemaCache.Commands;

public record AddDataSourceSchemaCacheCommand : DataSourceSchemaCacheState, IRequest<Validation<Error, DataSourceSchemaCacheState>>;

public class AddDataSourceSchemaCacheCommandHandler(ApplicationContext context,
                                IMapper mapper,
                                CompositeValidator<AddDataSourceSchemaCacheCommand> validator) : BaseCommandHandler<ApplicationContext, DataSourceSchemaCacheState, AddDataSourceSchemaCacheCommand>(context, mapper, validator), IRequestHandler<AddDataSourceSchemaCacheCommand, Validation<Error, DataSourceSchemaCacheState>>
{
    
public async Task<Validation<Error, DataSourceSchemaCacheState>> Handle(AddDataSourceSchemaCacheCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await Add(request, cancellationToken));
	
}

public class AddDataSourceSchemaCacheCommandValidator : AbstractValidator<AddDataSourceSchemaCacheCommand>
{
    readonly ApplicationContext _context;

    public AddDataSourceSchemaCacheCommandValidator(ApplicationContext context)
    {
        _context = context;

        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.NotExists<DataSourceSchemaCacheState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("DataSourceSchemaCache with id {PropertyValue} already exists");
        
    }
}
