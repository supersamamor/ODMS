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

public record AddDashboardWidgetCommand : DashboardWidgetState, IRequest<Validation<Error, DashboardWidgetState>>;

public class AddDashboardWidgetCommandHandler(ApplicationContext context,
                                IMapper mapper,
                                CompositeValidator<AddDashboardWidgetCommand> validator) : BaseCommandHandler<ApplicationContext, DashboardWidgetState, AddDashboardWidgetCommand>(context, mapper, validator), IRequestHandler<AddDashboardWidgetCommand, Validation<Error, DashboardWidgetState>>
{
    
public async Task<Validation<Error, DashboardWidgetState>> Handle(AddDashboardWidgetCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await Add(request, cancellationToken));
	
}

public class AddDashboardWidgetCommandValidator : AbstractValidator<AddDashboardWidgetCommand>
{
    readonly ApplicationContext _context;

    public AddDashboardWidgetCommandValidator(ApplicationContext context)
    {
        _context = context;

        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.NotExists<DashboardWidgetState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("DashboardWidget with id {PropertyValue} already exists");
        
    }
}
