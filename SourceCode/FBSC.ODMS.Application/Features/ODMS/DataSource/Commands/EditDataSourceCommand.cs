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

namespace FBSC.ODMS.Application.Features.ODMS.DataSource.Commands;

public record EditDataSourceCommand : DataSourceState, IRequest<Validation<Error, DataSourceState>>;

public class EditDataSourceCommandHandler(ApplicationContext context,
                                 IMapper mapper,
                                 CompositeValidator<EditDataSourceCommand> validator) : BaseCommandHandler<ApplicationContext, DataSourceState, EditDataSourceCommand>(context, mapper, validator), IRequestHandler<EditDataSourceCommand, Validation<Error, DataSourceState>>
{ 
    
public async Task<Validation<Error, DataSourceState>> Handle(EditDataSourceCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await Edit(request, cancellationToken));
	
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
	
    }
}
