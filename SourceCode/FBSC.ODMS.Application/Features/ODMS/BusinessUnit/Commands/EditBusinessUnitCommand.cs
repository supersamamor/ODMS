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

namespace FBSC.ODMS.Application.Features.ODMS.BusinessUnit.Commands;

public record EditBusinessUnitCommand : BusinessUnitState, IRequest<Validation<Error, BusinessUnitState>>;

public class EditBusinessUnitCommandHandler(ApplicationContext context,
                                 IMapper mapper,
                                 CompositeValidator<EditBusinessUnitCommand> validator) : BaseCommandHandler<ApplicationContext, BusinessUnitState, EditBusinessUnitCommand>(context, mapper, validator), IRequestHandler<EditBusinessUnitCommand, Validation<Error, BusinessUnitState>>
{ 
    
public async Task<Validation<Error, BusinessUnitState>> Handle(EditBusinessUnitCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await Edit(request, cancellationToken));
	
}

public class EditBusinessUnitCommandValidator : AbstractValidator<EditBusinessUnitCommand>
{
    readonly ApplicationContext _context;

    public EditBusinessUnitCommandValidator(ApplicationContext context)
    {
        _context = context;
		RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<BusinessUnitState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("BusinessUnit with id {PropertyValue} does not exists");
        RuleFor(x => x.Name).MustAsync(async (request, name, cancellation) => await _context.NotExists<BusinessUnitState>(x => x.Name == name && x.Id != request.Id, cancellationToken: cancellation)).WithMessage("BusinessUnit with name {PropertyValue} already exists");
	
    }
}
