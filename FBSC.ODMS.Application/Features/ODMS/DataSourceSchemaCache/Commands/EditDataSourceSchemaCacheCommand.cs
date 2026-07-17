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

public record EditDataSourceSchemaCacheCommand : DataSourceSchemaCacheState, IRequest<Validation<Error, DataSourceSchemaCacheState>>;

public class EditDataSourceSchemaCacheCommandHandler(ApplicationContext context,
                                 IMapper mapper,
                                 CompositeValidator<EditDataSourceSchemaCacheCommand> validator) : BaseCommandHandler<ApplicationContext, DataSourceSchemaCacheState, EditDataSourceSchemaCacheCommand>(context, mapper, validator), IRequestHandler<EditDataSourceSchemaCacheCommand, Validation<Error, DataSourceSchemaCacheState>>
{ 
    
public async Task<Validation<Error, DataSourceSchemaCacheState>> Handle(EditDataSourceSchemaCacheCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await Edit(request, cancellationToken));
	
}

public class EditDataSourceSchemaCacheCommandValidator : AbstractValidator<EditDataSourceSchemaCacheCommand>
{
    readonly ApplicationContext _context;

    public EditDataSourceSchemaCacheCommandValidator(ApplicationContext context)
    {
        _context = context;
		RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<DataSourceSchemaCacheState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("DataSourceSchemaCache with id {PropertyValue} does not exists");
        
    }
}
