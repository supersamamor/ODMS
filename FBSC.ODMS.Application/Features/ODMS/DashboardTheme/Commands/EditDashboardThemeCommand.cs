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

namespace FBSC.ODMS.Application.Features.ODMS.DashboardTheme.Commands;

public record EditDashboardThemeCommand : DashboardThemeState, IRequest<Validation<Error, DashboardThemeState>>;

public class EditDashboardThemeCommandHandler(ApplicationContext context,
                                 IMapper mapper,
                                 CompositeValidator<EditDashboardThemeCommand> validator) : BaseCommandHandler<ApplicationContext, DashboardThemeState, EditDashboardThemeCommand>(context, mapper, validator), IRequestHandler<EditDashboardThemeCommand, Validation<Error, DashboardThemeState>>
{ 
    
public async Task<Validation<Error, DashboardThemeState>> Handle(EditDashboardThemeCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await Edit(request, cancellationToken));
	
}

public class EditDashboardThemeCommandValidator : AbstractValidator<EditDashboardThemeCommand>
{
    readonly ApplicationContext _context;

    public EditDashboardThemeCommandValidator(ApplicationContext context)
    {
        _context = context;
		RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<DashboardThemeState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("DashboardTheme with id {PropertyValue} does not exists");
        RuleFor(x => x.Code).MustAsync(async (request, code, cancellation) => await _context.NotExists<DashboardThemeState>(x => x.Code == code && x.Id != request.Id, cancellationToken: cancellation)).WithMessage("DashboardTheme with code {PropertyValue} already exists");
	
    }
}
