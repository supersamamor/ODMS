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

public record EditEmployeeCommand : EmployeeState, IRequest<Validation<Error, EmployeeState>>;

public class EditEmployeeCommandHandler(ApplicationContext context,
                                 IMapper mapper,
                                 CompositeValidator<EditEmployeeCommand> validator) : BaseCommandHandler<ApplicationContext, EmployeeState, EditEmployeeCommand>(context, mapper, validator), IRequestHandler<EditEmployeeCommand, Validation<Error, EmployeeState>>
{ 
    
public async Task<Validation<Error, EmployeeState>> Handle(EditEmployeeCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await Edit(request, cancellationToken));
	
}

public class EditEmployeeCommandValidator : AbstractValidator<EditEmployeeCommand>
{
    readonly ApplicationContext _context;

    public EditEmployeeCommandValidator(ApplicationContext context)
    {
        _context = context;
		RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<EmployeeState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("Employee with id {PropertyValue} does not exists");
        
    }
}
