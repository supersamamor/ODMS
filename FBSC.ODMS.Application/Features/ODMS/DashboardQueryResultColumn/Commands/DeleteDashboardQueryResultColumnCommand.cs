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

namespace FBSC.ODMS.Application.Features.ODMS.DashboardQueryResultColumn.Commands;

public record DeleteDashboardQueryResultColumnCommand : BaseCommand, IRequest<Validation<Error, DashboardQueryResultColumnState>>;

public class DeleteDashboardQueryResultColumnCommandHandler(ApplicationContext context,
                                   IMapper mapper,
                                   CompositeValidator<DeleteDashboardQueryResultColumnCommand> validator) : BaseCommandHandler<ApplicationContext, DashboardQueryResultColumnState, DeleteDashboardQueryResultColumnCommand>(context, mapper, validator), IRequestHandler<DeleteDashboardQueryResultColumnCommand, Validation<Error, DashboardQueryResultColumnState>>
{ 
    public async Task<Validation<Error, DashboardQueryResultColumnState>> Handle(DeleteDashboardQueryResultColumnCommand request, CancellationToken cancellationToken) =>
        await Validators.ValidateTAsync(request, cancellationToken).BindT(
            async request => await Delete(request.Id, cancellationToken));
}


public class DeleteDashboardQueryResultColumnCommandValidator : AbstractValidator<DeleteDashboardQueryResultColumnCommand>
{
    readonly ApplicationContext _context;

    public DeleteDashboardQueryResultColumnCommandValidator(ApplicationContext context)
    {
        _context = context;
        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<DashboardQueryResultColumnState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("DashboardQueryResultColumn with id {PropertyValue} does not exists");
    }
}
