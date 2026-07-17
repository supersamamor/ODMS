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

public record AddDashboardThemeCommand : DashboardThemeState, IRequest<Validation<Error, DashboardThemeState>>;

public class AddDashboardThemeCommandHandler(ApplicationContext context,
                                IMapper mapper,
                                CompositeValidator<AddDashboardThemeCommand> validator) : BaseCommandHandler<ApplicationContext, DashboardThemeState, AddDashboardThemeCommand>(context, mapper, validator), IRequestHandler<AddDashboardThemeCommand, Validation<Error, DashboardThemeState>>
{
    
public async Task<Validation<Error, DashboardThemeState>> Handle(AddDashboardThemeCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await Add(request, cancellationToken));
	
}

public class AddDashboardThemeCommandValidator : AbstractValidator<AddDashboardThemeCommand>
{
    readonly ApplicationContext _context;

    public AddDashboardThemeCommandValidator(ApplicationContext context)
    {
        _context = context;

        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.NotExists<DashboardThemeState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("DashboardTheme with id {PropertyValue} already exists");
        RuleFor(x => x.Code).MustAsync(async (code, cancellation) => await _context.NotExists<DashboardThemeState>(x => x.Code == code, cancellationToken: cancellation)).WithMessage("DashboardTheme with code {PropertyValue} already exists");
	
    }
}
