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

namespace FBSC.ODMS.Application.Features.ODMS.DashboardRefreshJob.Commands;

public record EditDashboardRefreshJobCommand : DashboardRefreshJobState, IRequest<Validation<Error, DashboardRefreshJobState>>;

public class EditDashboardRefreshJobCommandHandler(ApplicationContext context,
                                 IMapper mapper,
                                 CompositeValidator<EditDashboardRefreshJobCommand> validator) : BaseCommandHandler<ApplicationContext, DashboardRefreshJobState, EditDashboardRefreshJobCommand>(context, mapper, validator), IRequestHandler<EditDashboardRefreshJobCommand, Validation<Error, DashboardRefreshJobState>>
{ 
    
public async Task<Validation<Error, DashboardRefreshJobState>> Handle(EditDashboardRefreshJobCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await Edit(request, cancellationToken));
	
}

public class EditDashboardRefreshJobCommandValidator : AbstractValidator<EditDashboardRefreshJobCommand>
{
    readonly ApplicationContext _context;

    public EditDashboardRefreshJobCommandValidator(ApplicationContext context)
    {
        _context = context;
		RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<DashboardRefreshJobState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("DashboardRefreshJob with id {PropertyValue} does not exists");
        
    }
}
