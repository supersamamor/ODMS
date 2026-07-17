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

namespace FBSC.ODMS.Application.Features.ODMS.Dashboard.Commands;

public record DeleteDashboardCommand : BaseCommand, IRequest<Validation<Error, DashboardState>>;

public class DeleteDashboardCommandHandler(ApplicationContext context,
                                   IMapper mapper,
                                   CompositeValidator<DeleteDashboardCommand> validator) : BaseCommandHandler<ApplicationContext, DashboardState, DeleteDashboardCommand>(context, mapper, validator), IRequestHandler<DeleteDashboardCommand, Validation<Error, DashboardState>>
{ 
    public async Task<Validation<Error, DashboardState>> Handle(DeleteDashboardCommand request, CancellationToken cancellationToken) =>
        await Validators.ValidateTAsync(request, cancellationToken).BindT(
            async request => await Delete(request.Id, cancellationToken));
}


public class DeleteDashboardCommandValidator : AbstractValidator<DeleteDashboardCommand>
{
    readonly ApplicationContext _context;

    public DeleteDashboardCommandValidator(ApplicationContext context)
    {
        _context = context;
        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<DashboardState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("Dashboard with id {PropertyValue} does not exists");
    }
}
