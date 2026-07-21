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
			async request => await AddBusinessUnit(request, cancellationToken));

	public async Task<Validation<Error, BusinessUnitState>> AddBusinessUnit(AddBusinessUnitCommand request, CancellationToken cancellationToken)
	{
		BusinessUnitState entity = Mapper.Map<BusinessUnitState>(request);
		AddEntitySubCollection<BusinessUnitState, BusinessUnitTechnologyBusinessPartnerState>(entity, nameof(request.TechnologyBusinessPartnerList));
		_ = await Context.AddAsync(entity, cancellationToken);
		_ = await Context.SaveChangesAsync(cancellationToken);
		return Success<Error, BusinessUnitState>(entity);
	}

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

        // Each assigned Technology Business Partner must reference an employee, and
        // the same partner may not be assigned to the unit twice (matches the DB
        // unique index on (BusinessUnitId, EmployeeId)).
        RuleForEach(x => x.TechnologyBusinessPartnerList).ChildRules(tbp =>
            tbp.RuleFor(t => t.EmployeeId).NotEmpty().WithMessage("Technology Business Partner is required."));
        RuleFor(x => x.TechnologyBusinessPartnerList)
            .Must(list => list == null || list.Where(t => !string.IsNullOrEmpty(t.EmployeeId)).GroupBy(t => t.EmployeeId).All(g => g.Count() == 1))
            .WithMessage("The same Technology Business Partner cannot be assigned more than once.");

    }
}
