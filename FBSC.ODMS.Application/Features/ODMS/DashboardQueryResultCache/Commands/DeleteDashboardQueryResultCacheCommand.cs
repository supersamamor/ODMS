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

namespace FBSC.ODMS.Application.Features.ODMS.DashboardQueryResultCache.Commands;

public record DeleteDashboardQueryResultCacheCommand : BaseCommand, IRequest<Validation<Error, DashboardQueryResultCacheState>>;

public class DeleteDashboardQueryResultCacheCommandHandler(ApplicationContext context,
                                   IMapper mapper,
                                   CompositeValidator<DeleteDashboardQueryResultCacheCommand> validator) : BaseCommandHandler<ApplicationContext, DashboardQueryResultCacheState, DeleteDashboardQueryResultCacheCommand>(context, mapper, validator), IRequestHandler<DeleteDashboardQueryResultCacheCommand, Validation<Error, DashboardQueryResultCacheState>>
{ 
    public async Task<Validation<Error, DashboardQueryResultCacheState>> Handle(DeleteDashboardQueryResultCacheCommand request, CancellationToken cancellationToken) =>
        await Validators.ValidateTAsync(request, cancellationToken).BindT(
            async request => await Delete(request.Id, cancellationToken));
}


public class DeleteDashboardQueryResultCacheCommandValidator : AbstractValidator<DeleteDashboardQueryResultCacheCommand>
{
    readonly ApplicationContext _context;

    public DeleteDashboardQueryResultCacheCommandValidator(ApplicationContext context)
    {
        _context = context;
        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<DashboardQueryResultCacheState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("DashboardQueryResultCache with id {PropertyValue} does not exists");
    }
}
