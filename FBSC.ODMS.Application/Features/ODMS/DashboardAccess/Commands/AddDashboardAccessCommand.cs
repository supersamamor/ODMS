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

public record AddDashboardAccessCommand : DashboardAccessState, IRequest<Validation<Error, DashboardAccessState>>;

public class AddDashboardAccessCommandHandler(ApplicationContext context,
                                IMapper mapper,
                                CompositeValidator<AddDashboardAccessCommand> validator) : BaseCommandHandler<ApplicationContext, DashboardAccessState, AddDashboardAccessCommand>(context, mapper, validator), IRequestHandler<AddDashboardAccessCommand, Validation<Error, DashboardAccessState>>
{
    
public async Task<Validation<Error, DashboardAccessState>> Handle(AddDashboardAccessCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await Add(request, cancellationToken));
	
}

public class AddDashboardAccessCommandValidator : AbstractValidator<AddDashboardAccessCommand>
{
    readonly ApplicationContext _context;

    public AddDashboardAccessCommandValidator(ApplicationContext context)
    {
        _context = context;

        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.NotExists<DashboardAccessState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("DashboardAccess with id {PropertyValue} already exists");
        
    }
}
