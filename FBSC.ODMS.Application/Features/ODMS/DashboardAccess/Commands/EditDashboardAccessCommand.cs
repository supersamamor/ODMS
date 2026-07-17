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

namespace FBSC.ODMS.Application.Features.ODMS.DashboardAccess.Commands;

public record EditDashboardAccessCommand : DashboardAccessState, IRequest<Validation<Error, DashboardAccessState>>;

public class EditDashboardAccessCommandHandler(ApplicationContext context,
                                 IMapper mapper,
                                 CompositeValidator<EditDashboardAccessCommand> validator) : BaseCommandHandler<ApplicationContext, DashboardAccessState, EditDashboardAccessCommand>(context, mapper, validator), IRequestHandler<EditDashboardAccessCommand, Validation<Error, DashboardAccessState>>
{ 
    
public async Task<Validation<Error, DashboardAccessState>> Handle(EditDashboardAccessCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await Edit(request, cancellationToken));
	
}

public class EditDashboardAccessCommandValidator : AbstractValidator<EditDashboardAccessCommand>
{
    readonly ApplicationContext _context;

    public EditDashboardAccessCommandValidator(ApplicationContext context)
    {
        _context = context;
		RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<DashboardAccessState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("DashboardAccess with id {PropertyValue} does not exists");
        
    }
}
