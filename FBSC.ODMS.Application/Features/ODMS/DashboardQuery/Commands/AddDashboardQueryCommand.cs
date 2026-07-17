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

namespace FBSC.ODMS.Application.Features.ODMS.DashboardQuery.Commands;

public record AddDashboardQueryCommand : DashboardQueryState, IRequest<Validation<Error, DashboardQueryState>>;

public class AddDashboardQueryCommandHandler(ApplicationContext context,
                                IMapper mapper,
                                CompositeValidator<AddDashboardQueryCommand> validator,IdentityContext identityContext) : BaseCommandHandler<ApplicationContext, DashboardQueryState, AddDashboardQueryCommand>(context, mapper, validator), IRequestHandler<AddDashboardQueryCommand, Validation<Error, DashboardQueryState>>
{
    public async Task<Validation<Error, DashboardQueryState>> Handle(AddDashboardQueryCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await AddDashboardQuery(request, cancellationToken));


	public async Task<Validation<Error, DashboardQueryState>> AddDashboardQuery(AddDashboardQueryCommand request, CancellationToken cancellationToken)
	{
		DashboardQueryState entity = Mapper.Map<DashboardQueryState>(request);
		AddEntitySubCollection<DashboardQueryState, DashboardQueryParameterState>(entity, nameof(request.DashboardQueryParameterList));
		AddEntitySubCollection<DashboardQueryState, DashboardQueryResultColumnState>(entity, nameof(request.DashboardQueryResultColumnList));
		AddEntitySubCollection<DashboardQueryState, DashboardQueryResultCacheState>(entity, nameof(request.DashboardQueryResultCacheList));
		AddEntitySubCollection<DashboardQueryState, DashboardWidgetState>(entity, nameof(request.DashboardWidgetList));
		AddEntitySubCollection<DashboardQueryState, DashboardRefreshJobState>(entity, nameof(request.DashboardRefreshJobList));
		_ = await Context.AddAsync(entity, cancellationToken);
		await Helpers.ApprovalHelper.AddApprovers(Context, identityContext, ApprovalModule.DashboardQuery, entity.Id, cancellationToken);
		_ = await Context.SaveChangesAsync(cancellationToken);
		return Success<Error, DashboardQueryState>(entity);
	}
	
}

public class AddDashboardQueryCommandValidator : AbstractValidator<AddDashboardQueryCommand>
{
    readonly ApplicationContext _context;

    public AddDashboardQueryCommandValidator(ApplicationContext context)
    {
        _context = context;

        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.NotExists<DashboardQueryState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("DashboardQuery with id {PropertyValue} already exists");
        RuleFor(x => x.Name).MustAsync(async (name, cancellation) => await _context.NotExists<DashboardQueryState>(x => x.Name == name, cancellationToken: cancellation)).WithMessage("DashboardQuery with name {PropertyValue} already exists");
	RuleFor(x => x.QueryHash).MustAsync(async (queryHash, cancellation) => await _context.NotExists<DashboardQueryState>(x => x.QueryHash == queryHash, cancellationToken: cancellation)).WithMessage("DashboardQuery with queryHash {PropertyValue} already exists");
	
    }
}
