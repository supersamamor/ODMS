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

public record EditDashboardQueryCommand : DashboardQueryState, IRequest<Validation<Error, DashboardQueryState>>;

public class EditDashboardQueryCommandHandler(ApplicationContext context,
                                 IMapper mapper,
                                 CompositeValidator<EditDashboardQueryCommand> validator) : BaseCommandHandler<ApplicationContext, DashboardQueryState, EditDashboardQueryCommand>(context, mapper, validator), IRequestHandler<EditDashboardQueryCommand, Validation<Error, DashboardQueryState>>
{ 
    public async Task<Validation<Error, DashboardQueryState>> Handle(EditDashboardQueryCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await EditDashboardQuery(request, cancellationToken));


	public async Task<Validation<Error, DashboardQueryState>> EditDashboardQuery(EditDashboardQueryCommand request, CancellationToken cancellationToken)
	{
		var entity = await Context.DashboardQuery.Where(l => l.Id == request.Id).SingleAsync(cancellationToken: cancellationToken);
		Mapper.Map(request, entity);
		await UpdateEntitySubCollectionAsync<DashboardQueryState, DashboardQueryParameterState>(request.Id, nameof(DashboardQueryParameterState.DashboardQueryId), nameof(request.DashboardQueryParameterList), entity, cancellationToken);
		await UpdateEntitySubCollectionAsync<DashboardQueryState, DashboardQueryResultColumnState>(request.Id, nameof(DashboardQueryResultColumnState.DashboardQueryId), nameof(request.DashboardQueryResultColumnList), entity, cancellationToken);
		await UpdateEntitySubCollectionAsync<DashboardQueryState, DashboardQueryResultCacheState>(request.Id, nameof(DashboardQueryResultCacheState.DashboardQueryId), nameof(request.DashboardQueryResultCacheList), entity, cancellationToken);
		await UpdateEntitySubCollectionAsync<DashboardQueryState, DashboardWidgetState>(request.Id, nameof(DashboardWidgetState.DashboardQueryId), nameof(request.DashboardWidgetList), entity, cancellationToken);
		await UpdateEntitySubCollectionAsync<DashboardQueryState, DashboardRefreshJobState>(request.Id, nameof(DashboardRefreshJobState.DashboardQueryId), nameof(request.DashboardRefreshJobList), entity, cancellationToken);
		Context.Update(entity);
		_ = await Context.SaveChangesAsync(cancellationToken);
		return Success<Error, DashboardQueryState>(entity);
	}
	
}

public class EditDashboardQueryCommandValidator : AbstractValidator<EditDashboardQueryCommand>
{
    readonly ApplicationContext _context;

    public EditDashboardQueryCommandValidator(ApplicationContext context)
    {
        _context = context;
		RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<DashboardQueryState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("DashboardQuery with id {PropertyValue} does not exists");
        RuleFor(x => x.Name).MustAsync(async (request, name, cancellation) => await _context.NotExists<DashboardQueryState>(x => x.Name == name && x.Id != request.Id, cancellationToken: cancellation)).WithMessage("DashboardQuery with name {PropertyValue} already exists");
	RuleFor(x => x.QueryHash).MustAsync(async (request, queryHash, cancellation) => await _context.NotExists<DashboardQueryState>(x => x.QueryHash == queryHash && x.Id != request.Id, cancellationToken: cancellation)).WithMessage("DashboardQuery with queryHash {PropertyValue} already exists");
	
    }
}
