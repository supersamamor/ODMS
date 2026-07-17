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

namespace FBSC.ODMS.Application.Features.ODMS.DashboardRefreshJob.Commands;

public record DeleteDashboardRefreshJobCommand : BaseCommand, IRequest<Validation<Error, DashboardRefreshJobState>>;

public class DeleteDashboardRefreshJobCommandHandler(ApplicationContext context,
                                   IMapper mapper,
                                   CompositeValidator<DeleteDashboardRefreshJobCommand> validator) : BaseCommandHandler<ApplicationContext, DashboardRefreshJobState, DeleteDashboardRefreshJobCommand>(context, mapper, validator), IRequestHandler<DeleteDashboardRefreshJobCommand, Validation<Error, DashboardRefreshJobState>>
{ 
    public async Task<Validation<Error, DashboardRefreshJobState>> Handle(DeleteDashboardRefreshJobCommand request, CancellationToken cancellationToken) =>
        await Validators.ValidateTAsync(request, cancellationToken).BindT(
            async request => await Delete(request.Id, cancellationToken));
}


public class DeleteDashboardRefreshJobCommandValidator : AbstractValidator<DeleteDashboardRefreshJobCommand>
{
    readonly ApplicationContext _context;

    public DeleteDashboardRefreshJobCommandValidator(ApplicationContext context)
    {
        _context = context;
        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<DashboardRefreshJobState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("DashboardRefreshJob with id {PropertyValue} does not exists");
    }
}
