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
    
public async Task<Validation<Error, BusinessUnitState>> Handle(EditBusinessUnitCommand request, CancellationToken cancellationToken)
	{
		// Code is immutable after creation: re-stamp the request with the stored
		// value so a tampered payload can never change it.
		var storedCode = await Context.BusinessUnit.AsNoTracking()
			.Where(b => b.Id == request.Id).Select(b => b.Code).FirstOrDefaultAsync(cancellationToken);
		if (storedCode != null)
		{
			request = request with { Code = storedCode };
		}
		return await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async req => await Edit(req, cancellationToken));
	}
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
