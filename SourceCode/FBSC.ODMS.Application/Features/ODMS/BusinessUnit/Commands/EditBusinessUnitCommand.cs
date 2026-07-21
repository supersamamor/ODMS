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
			async req => await EditBusinessUnit(req, cancellationToken));
	}

	public async Task<Validation<Error, BusinessUnitState>> EditBusinessUnit(EditBusinessUnitCommand request, CancellationToken cancellationToken)
	{
		var entity = await Context.BusinessUnit.Where(l => l.Id == request.Id).SingleAsync(cancellationToken);
		Mapper.Map(request, entity);
		await UpdateEntitySubCollectionAsync<BusinessUnitState, BusinessUnitTechnologyBusinessPartnerState>(request.Id, nameof(BusinessUnitTechnologyBusinessPartnerState.BusinessUnitId), nameof(request.TechnologyBusinessPartnerList), entity, cancellationToken);
		Context.Update(entity);
		_ = await Context.SaveChangesAsync(cancellationToken);
		return Success<Error, BusinessUnitState>(entity);
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

        // Each assigned Technology Business Partner must reference an employee, and
        // no duplicates within the unit (matches the DB unique index).
        RuleForEach(x => x.TechnologyBusinessPartnerList).ChildRules(tbp =>
            tbp.RuleFor(t => t.EmployeeId).NotEmpty().WithMessage("Technology Business Partner is required."));
        RuleFor(x => x.TechnologyBusinessPartnerList)
            .Must(list => list == null || list.Where(t => !string.IsNullOrEmpty(t.EmployeeId)).GroupBy(t => t.EmployeeId).All(g => g.Count() == 1))
            .WithMessage("The same Technology Business Partner cannot be assigned more than once.");

    }
}
