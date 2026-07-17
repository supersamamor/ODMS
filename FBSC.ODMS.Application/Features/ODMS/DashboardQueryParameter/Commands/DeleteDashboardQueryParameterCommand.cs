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

namespace FBSC.ODMS.Application.Features.ODMS.DashboardQueryParameter.Commands;

public record DeleteDashboardQueryParameterCommand : BaseCommand, IRequest<Validation<Error, DashboardQueryParameterState>>;

public class DeleteDashboardQueryParameterCommandHandler(ApplicationContext context,
                                   IMapper mapper,
                                   CompositeValidator<DeleteDashboardQueryParameterCommand> validator) : BaseCommandHandler<ApplicationContext, DashboardQueryParameterState, DeleteDashboardQueryParameterCommand>(context, mapper, validator), IRequestHandler<DeleteDashboardQueryParameterCommand, Validation<Error, DashboardQueryParameterState>>
{ 
    public async Task<Validation<Error, DashboardQueryParameterState>> Handle(DeleteDashboardQueryParameterCommand request, CancellationToken cancellationToken) =>
        await Validators.ValidateTAsync(request, cancellationToken).BindT(
            async request => await Delete(request.Id, cancellationToken));
}


public class DeleteDashboardQueryParameterCommandValidator : AbstractValidator<DeleteDashboardQueryParameterCommand>
{
    readonly ApplicationContext _context;

    public DeleteDashboardQueryParameterCommandValidator(ApplicationContext context)
    {
        _context = context;
        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<DashboardQueryParameterState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("DashboardQueryParameter with id {PropertyValue} does not exists");
    }
}
