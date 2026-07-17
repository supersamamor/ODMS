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

public record EditDashboardQueryParameterCommand : DashboardQueryParameterState, IRequest<Validation<Error, DashboardQueryParameterState>>;

public class EditDashboardQueryParameterCommandHandler(ApplicationContext context,
                                 IMapper mapper,
                                 CompositeValidator<EditDashboardQueryParameterCommand> validator) : BaseCommandHandler<ApplicationContext, DashboardQueryParameterState, EditDashboardQueryParameterCommand>(context, mapper, validator), IRequestHandler<EditDashboardQueryParameterCommand, Validation<Error, DashboardQueryParameterState>>
{ 
    
public async Task<Validation<Error, DashboardQueryParameterState>> Handle(EditDashboardQueryParameterCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await Edit(request, cancellationToken));
	
}

public class EditDashboardQueryParameterCommandValidator : AbstractValidator<EditDashboardQueryParameterCommand>
{
    readonly ApplicationContext _context;

    public EditDashboardQueryParameterCommandValidator(ApplicationContext context)
    {
        _context = context;
		RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<DashboardQueryParameterState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("DashboardQueryParameter with id {PropertyValue} does not exists");
        
    }
}
