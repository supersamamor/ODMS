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

namespace FBSC.ODMS.Application.Features.ODMS.Dashboard.Commands;

public record EditDashboardCommand : DashboardState, IRequest<Validation<Error, DashboardState>>;

public class EditDashboardCommandHandler(ApplicationContext context,
                                 IMapper mapper,
                                 CompositeValidator<EditDashboardCommand> validator) : BaseCommandHandler<ApplicationContext, DashboardState, EditDashboardCommand>(context, mapper, validator), IRequestHandler<EditDashboardCommand, Validation<Error, DashboardState>>
{ 
    public async Task<Validation<Error, DashboardState>> Handle(EditDashboardCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await EditDashboard(request, cancellationToken));


	public async Task<Validation<Error, DashboardState>> EditDashboard(EditDashboardCommand request, CancellationToken cancellationToken)
	{
		var entity = await Context.Dashboard.Where(l => l.Id == request.Id).SingleAsync(cancellationToken: cancellationToken);
		Mapper.Map(request, entity);
		await UpdateEntitySubCollectionAsync<DashboardState, DashboardWidgetState>(request.Id, nameof(DashboardWidgetState.DrillDownDashboardId), nameof(request.DashboardWidgetList), entity, cancellationToken);
		await UpdateEntitySubCollectionAsync<DashboardState, DashboardWidgetState>(request.Id, nameof(DashboardWidgetState.DrillDownDashboardId), nameof(request.DashboardWidgetList), entity, cancellationToken);
		await UpdateEntitySubCollectionAsync<DashboardState, DashboardAccessState>(request.Id, nameof(DashboardAccessState.DashboardId), nameof(request.DashboardAccessList), entity, cancellationToken);
		Context.Update(entity);
		_ = await Context.SaveChangesAsync(cancellationToken);
		return Success<Error, DashboardState>(entity);
	}
	
}

public class EditDashboardCommandValidator : AbstractValidator<EditDashboardCommand>
{
    readonly ApplicationContext _context;

    public EditDashboardCommandValidator(ApplicationContext context)
    {
        _context = context;
		RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<DashboardState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("Dashboard with id {PropertyValue} does not exists");
        RuleFor(x => x.Code).MustAsync(async (request, code, cancellation) => await _context.NotExists<DashboardState>(x => x.Code == code && x.Id != request.Id, cancellationToken: cancellation)).WithMessage("Dashboard with code {PropertyValue} already exists");
	
    }
}
