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

namespace FBSC.ODMS.Application.Features.ODMS.Employee.Commands;

public record AddEmployeeCommand : EmployeeState, IRequest<Validation<Error, EmployeeState>>;

public class AddEmployeeCommandHandler(ApplicationContext context,
                                IMapper mapper,
                                CompositeValidator<AddEmployeeCommand> validator) : BaseCommandHandler<ApplicationContext, EmployeeState, AddEmployeeCommand>(context, mapper, validator), IRequestHandler<AddEmployeeCommand, Validation<Error, EmployeeState>>
{
    
public async Task<Validation<Error, EmployeeState>> Handle(AddEmployeeCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await Add(request, cancellationToken));
	
}

public class AddEmployeeCommandValidator : AbstractValidator<AddEmployeeCommand>
{
    readonly ApplicationContext _context;

    public AddEmployeeCommandValidator(ApplicationContext context)
    {
        _context = context;

        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.NotExists<EmployeeState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("Employee with id {PropertyValue} already exists");
        
    }
}
