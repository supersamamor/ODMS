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

namespace FBSC.ODMS.Application.Features.ODMS.DashboardQueryResultCache.Commands;

public record AddDashboardQueryResultCacheCommand : DashboardQueryResultCacheState, IRequest<Validation<Error, DashboardQueryResultCacheState>>;

public class AddDashboardQueryResultCacheCommandHandler(ApplicationContext context,
                                IMapper mapper,
                                CompositeValidator<AddDashboardQueryResultCacheCommand> validator) : BaseCommandHandler<ApplicationContext, DashboardQueryResultCacheState, AddDashboardQueryResultCacheCommand>(context, mapper, validator), IRequestHandler<AddDashboardQueryResultCacheCommand, Validation<Error, DashboardQueryResultCacheState>>
{
    
public async Task<Validation<Error, DashboardQueryResultCacheState>> Handle(AddDashboardQueryResultCacheCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await Add(request, cancellationToken));
	
}

public class AddDashboardQueryResultCacheCommandValidator : AbstractValidator<AddDashboardQueryResultCacheCommand>
{
    readonly ApplicationContext _context;

    public AddDashboardQueryResultCacheCommandValidator(ApplicationContext context)
    {
        _context = context;

        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.NotExists<DashboardQueryResultCacheState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("DashboardQueryResultCache with id {PropertyValue} already exists");
        
    }
}
