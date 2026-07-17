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

namespace FBSC.ODMS.Application.Features.ODMS.DashboardAccess.Commands;

public record DeleteDashboardAccessCommand : BaseCommand, IRequest<Validation<Error, DashboardAccessState>>;

public class DeleteDashboardAccessCommandHandler(ApplicationContext context,
                                   IMapper mapper,
                                   CompositeValidator<DeleteDashboardAccessCommand> validator) : BaseCommandHandler<ApplicationContext, DashboardAccessState, DeleteDashboardAccessCommand>(context, mapper, validator), IRequestHandler<DeleteDashboardAccessCommand, Validation<Error, DashboardAccessState>>
{ 
    public async Task<Validation<Error, DashboardAccessState>> Handle(DeleteDashboardAccessCommand request, CancellationToken cancellationToken) =>
        await Validators.ValidateTAsync(request, cancellationToken).BindT(
            async request => await Delete(request.Id, cancellationToken));
}


public class DeleteDashboardAccessCommandValidator : AbstractValidator<DeleteDashboardAccessCommand>
{
    readonly ApplicationContext _context;

    public DeleteDashboardAccessCommandValidator(ApplicationContext context)
    {
        _context = context;
        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<DashboardAccessState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("DashboardAccess with id {PropertyValue} does not exists");
    }
}
