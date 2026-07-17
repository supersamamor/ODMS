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

namespace FBSC.ODMS.Application.Features.ODMS.DashboardTheme.Commands;

public record DeleteDashboardThemeCommand : BaseCommand, IRequest<Validation<Error, DashboardThemeState>>;

public class DeleteDashboardThemeCommandHandler(ApplicationContext context,
                                   IMapper mapper,
                                   CompositeValidator<DeleteDashboardThemeCommand> validator) : BaseCommandHandler<ApplicationContext, DashboardThemeState, DeleteDashboardThemeCommand>(context, mapper, validator), IRequestHandler<DeleteDashboardThemeCommand, Validation<Error, DashboardThemeState>>
{ 
    public async Task<Validation<Error, DashboardThemeState>> Handle(DeleteDashboardThemeCommand request, CancellationToken cancellationToken) =>
        await Validators.ValidateTAsync(request, cancellationToken).BindT(
            async request => await Delete(request.Id, cancellationToken));
}


public class DeleteDashboardThemeCommandValidator : AbstractValidator<DeleteDashboardThemeCommand>
{
    readonly ApplicationContext _context;

    public DeleteDashboardThemeCommandValidator(ApplicationContext context)
    {
        _context = context;
        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<DashboardThemeState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("DashboardTheme with id {PropertyValue} does not exists");
    }
}
