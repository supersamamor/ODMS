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

namespace FBSC.ODMS.Application.Features.ODMS.DashboardQuery.Commands;

public record DeleteDashboardQueryCommand : BaseCommand, IRequest<Validation<Error, DashboardQueryState>>;

public class DeleteDashboardQueryCommandHandler(ApplicationContext context,
                                   IMapper mapper,
                                   CompositeValidator<DeleteDashboardQueryCommand> validator) : BaseCommandHandler<ApplicationContext, DashboardQueryState, DeleteDashboardQueryCommand>(context, mapper, validator), IRequestHandler<DeleteDashboardQueryCommand, Validation<Error, DashboardQueryState>>
{ 
    public async Task<Validation<Error, DashboardQueryState>> Handle(DeleteDashboardQueryCommand request, CancellationToken cancellationToken) =>
        await Validators.ValidateTAsync(request, cancellationToken).BindT(
            async request => await Delete(request.Id, cancellationToken));
}


public class DeleteDashboardQueryCommandValidator : AbstractValidator<DeleteDashboardQueryCommand>
{
    readonly ApplicationContext _context;

    public DeleteDashboardQueryCommandValidator(ApplicationContext context)
    {
        _context = context;
        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<DashboardQueryState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("DashboardQuery with id {PropertyValue} does not exists");
    }
}
