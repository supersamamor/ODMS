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

public record AddDashboardCommand : DashboardState, IRequest<Validation<Error, DashboardState>>;

public class AddDashboardCommandHandler(ApplicationContext context,
                                IMapper mapper,
                                CompositeValidator<AddDashboardCommand> validator,IdentityContext identityContext) : BaseCommandHandler<ApplicationContext, DashboardState, AddDashboardCommand>(context, mapper, validator), IRequestHandler<AddDashboardCommand, Validation<Error, DashboardState>>
{
    public async Task<Validation<Error, DashboardState>> Handle(AddDashboardCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await AddDashboard(request, cancellationToken));


	public async Task<Validation<Error, DashboardState>> AddDashboard(AddDashboardCommand request, CancellationToken cancellationToken)
	{
		DashboardState entity = Mapper.Map<DashboardState>(request);
		AddEntitySubCollection<DashboardState, DashboardWidgetState>(entity, nameof(request.DashboardWidgetList));
		AddEntitySubCollection<DashboardState, DashboardWidgetState>(entity, nameof(request.DashboardWidgetList));
		AddEntitySubCollection<DashboardState, DashboardAccessState>(entity, nameof(request.DashboardAccessList));
		_ = await Context.AddAsync(entity, cancellationToken);
		await Helpers.ApprovalHelper.AddApprovers(Context, identityContext, ApprovalModule.Dashboard, entity.Id, cancellationToken);
		_ = await Context.SaveChangesAsync(cancellationToken);
		return Success<Error, DashboardState>(entity);
	}
	
}

public class AddDashboardCommandValidator : AbstractValidator<AddDashboardCommand>
{
    readonly ApplicationContext _context;

    public AddDashboardCommandValidator(ApplicationContext context)
    {
        _context = context;

        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.NotExists<DashboardState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("Dashboard with id {PropertyValue} already exists");
        RuleFor(x => x.Code).MustAsync(async (code, cancellation) => await _context.NotExists<DashboardState>(x => x.Code == code, cancellationToken: cancellation)).WithMessage("Dashboard with code {PropertyValue} already exists");
	
    }
}
