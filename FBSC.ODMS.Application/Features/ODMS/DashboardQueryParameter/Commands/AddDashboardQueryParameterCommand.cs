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

namespace FBSC.ODMS.Application.Features.ODMS.DashboardQueryParameter.Commands;

public record AddDashboardQueryParameterCommand : DashboardQueryParameterState, IRequest<Validation<Error, DashboardQueryParameterState>>;

public class AddDashboardQueryParameterCommandHandler(ApplicationContext context,
                                IMapper mapper,
                                CompositeValidator<AddDashboardQueryParameterCommand> validator) : BaseCommandHandler<ApplicationContext, DashboardQueryParameterState, AddDashboardQueryParameterCommand>(context, mapper, validator), IRequestHandler<AddDashboardQueryParameterCommand, Validation<Error, DashboardQueryParameterState>>
{
    
public async Task<Validation<Error, DashboardQueryParameterState>> Handle(AddDashboardQueryParameterCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await Add(request, cancellationToken));
	
}

public class AddDashboardQueryParameterCommandValidator : AbstractValidator<AddDashboardQueryParameterCommand>
{
    readonly ApplicationContext _context;

    public AddDashboardQueryParameterCommandValidator(ApplicationContext context)
    {
        _context = context;

        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.NotExists<DashboardQueryParameterState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("DashboardQueryParameter with id {PropertyValue} already exists");
        
    }
}
