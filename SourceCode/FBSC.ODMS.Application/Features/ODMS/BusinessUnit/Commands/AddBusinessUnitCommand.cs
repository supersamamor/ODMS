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

public record AddBusinessUnitCommand : BusinessUnitState, IRequest<Validation<Error, BusinessUnitState>>;

public class AddBusinessUnitCommandHandler(ApplicationContext context,
                                IMapper mapper,
                                CompositeValidator<AddBusinessUnitCommand> validator) : BaseCommandHandler<ApplicationContext, BusinessUnitState, AddBusinessUnitCommand>(context, mapper, validator), IRequestHandler<AddBusinessUnitCommand, Validation<Error, BusinessUnitState>>
{
    
public async Task<Validation<Error, BusinessUnitState>> Handle(AddBusinessUnitCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await Add(request, cancellationToken));
	
}

public class AddBusinessUnitCommandValidator : AbstractValidator<AddBusinessUnitCommand>
{
    readonly ApplicationContext _context;

    public AddBusinessUnitCommandValidator(ApplicationContext context)
    {
        _context = context;

        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.NotExists<BusinessUnitState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("BusinessUnit with id {PropertyValue} already exists");
        RuleFor(x => x.Name).MustAsync(async (name, cancellation) => await _context.NotExists<BusinessUnitState>(x => x.Name == name, cancellationToken: cancellation)).WithMessage("BusinessUnit with name {PropertyValue} already exists");
        RuleFor(x => x.Code).NotEmpty().WithMessage("Code is required");
        RuleFor(x => x.Code).MustAsync(async (code, cancellation) => await _context.NotExists<BusinessUnitState>(x => x.Code == code, cancellationToken: cancellation)).WithMessage("BusinessUnit with code {PropertyValue} already exists");
	
    }
}
