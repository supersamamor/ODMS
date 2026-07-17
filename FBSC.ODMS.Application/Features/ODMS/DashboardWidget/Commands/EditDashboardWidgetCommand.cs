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

namespace FBSC.ODMS.Application.Features.ODMS.DashboardWidget.Commands;

public record EditDashboardWidgetCommand : DashboardWidgetState, IRequest<Validation<Error, DashboardWidgetState>>;

public class EditDashboardWidgetCommandHandler(ApplicationContext context,
                                 IMapper mapper,
                                 CompositeValidator<EditDashboardWidgetCommand> validator) : BaseCommandHandler<ApplicationContext, DashboardWidgetState, EditDashboardWidgetCommand>(context, mapper, validator), IRequestHandler<EditDashboardWidgetCommand, Validation<Error, DashboardWidgetState>>
{ 
    
public async Task<Validation<Error, DashboardWidgetState>> Handle(EditDashboardWidgetCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await Edit(request, cancellationToken));
	
}

public class EditDashboardWidgetCommandValidator : AbstractValidator<EditDashboardWidgetCommand>
{
    readonly ApplicationContext _context;

    public EditDashboardWidgetCommandValidator(ApplicationContext context)
    {
        _context = context;
		RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<DashboardWidgetState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("DashboardWidget with id {PropertyValue} does not exists");
        
    }
}
