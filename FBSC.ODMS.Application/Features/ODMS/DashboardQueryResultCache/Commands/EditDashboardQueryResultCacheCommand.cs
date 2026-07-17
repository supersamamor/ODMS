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

public record EditDashboardQueryResultCacheCommand : DashboardQueryResultCacheState, IRequest<Validation<Error, DashboardQueryResultCacheState>>;

public class EditDashboardQueryResultCacheCommandHandler(ApplicationContext context,
                                 IMapper mapper,
                                 CompositeValidator<EditDashboardQueryResultCacheCommand> validator) : BaseCommandHandler<ApplicationContext, DashboardQueryResultCacheState, EditDashboardQueryResultCacheCommand>(context, mapper, validator), IRequestHandler<EditDashboardQueryResultCacheCommand, Validation<Error, DashboardQueryResultCacheState>>
{ 
    
public async Task<Validation<Error, DashboardQueryResultCacheState>> Handle(EditDashboardQueryResultCacheCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await Edit(request, cancellationToken));
	
}

public class EditDashboardQueryResultCacheCommandValidator : AbstractValidator<EditDashboardQueryResultCacheCommand>
{
    readonly ApplicationContext _context;

    public EditDashboardQueryResultCacheCommandValidator(ApplicationContext context)
    {
        _context = context;
		RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<DashboardQueryResultCacheState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("DashboardQueryResultCache with id {PropertyValue} does not exists");
        
    }
}
