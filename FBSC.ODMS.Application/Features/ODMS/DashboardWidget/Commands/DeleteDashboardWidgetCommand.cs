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

namespace FBSC.ODMS.Application.Features.ODMS.DashboardWidget.Commands;

public record DeleteDashboardWidgetCommand : BaseCommand, IRequest<Validation<Error, DashboardWidgetState>>;

public class DeleteDashboardWidgetCommandHandler(ApplicationContext context,
                                   IMapper mapper,
                                   CompositeValidator<DeleteDashboardWidgetCommand> validator) : BaseCommandHandler<ApplicationContext, DashboardWidgetState, DeleteDashboardWidgetCommand>(context, mapper, validator), IRequestHandler<DeleteDashboardWidgetCommand, Validation<Error, DashboardWidgetState>>
{ 
    public async Task<Validation<Error, DashboardWidgetState>> Handle(DeleteDashboardWidgetCommand request, CancellationToken cancellationToken) =>
        await Validators.ValidateTAsync(request, cancellationToken).BindT(
            async request => await Delete(request.Id, cancellationToken));
}


public class DeleteDashboardWidgetCommandValidator : AbstractValidator<DeleteDashboardWidgetCommand>
{
    readonly ApplicationContext _context;

    public DeleteDashboardWidgetCommandValidator(ApplicationContext context)
    {
        _context = context;
        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<DashboardWidgetState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("DashboardWidget with id {PropertyValue} does not exists");
    }
}
