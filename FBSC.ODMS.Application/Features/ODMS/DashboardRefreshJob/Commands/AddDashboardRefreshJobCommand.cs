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

public record AddDashboardRefreshJobCommand : DashboardRefreshJobState, IRequest<Validation<Error, DashboardRefreshJobState>>;

public class AddDashboardRefreshJobCommandHandler(ApplicationContext context,
                                IMapper mapper,
                                CompositeValidator<AddDashboardRefreshJobCommand> validator) : BaseCommandHandler<ApplicationContext, DashboardRefreshJobState, AddDashboardRefreshJobCommand>(context, mapper, validator), IRequestHandler<AddDashboardRefreshJobCommand, Validation<Error, DashboardRefreshJobState>>
{
    
public async Task<Validation<Error, DashboardRefreshJobState>> Handle(AddDashboardRefreshJobCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await Add(request, cancellationToken));
	
}

public class AddDashboardRefreshJobCommandValidator : AbstractValidator<AddDashboardRefreshJobCommand>
{
    readonly ApplicationContext _context;

    public AddDashboardRefreshJobCommandValidator(ApplicationContext context)
    {
        _context = context;

        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.NotExists<DashboardRefreshJobState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("DashboardRefreshJob with id {PropertyValue} already exists");
        
    }
}
