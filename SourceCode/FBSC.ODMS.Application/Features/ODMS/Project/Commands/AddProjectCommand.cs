using AutoMapper;
using FBSC.Common.Core.Commands;
using FBSC.Common.Data;
using FBSC.Common.Utility.Validators;
using FBSC.ODMS.Application.Helpers;
using FBSC.ODMS.Core.Constants;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using FluentValidation;
using LanguageExt;
using LanguageExt.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static LanguageExt.Prelude;

namespace FBSC.ODMS.Application.Features.ODMS.Project.Commands;

public record AddProjectCommand : ProjectState, IRequest<Validation<Error, ProjectState>>;

public class AddProjectCommandHandler(ApplicationContext context,
                                IMapper mapper,
                                CompositeValidator<AddProjectCommand> validator,
                                IdentityContext identityContext) : BaseCommandHandler<ApplicationContext, ProjectState, AddProjectCommand>(context, mapper, validator), IRequestHandler<AddProjectCommand, Validation<Error, ProjectState>>
{
    public async Task<Validation<Error, ProjectState>> Handle(AddProjectCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await AddProject(request, cancellationToken));


	public async Task<Validation<Error, ProjectState>> AddProject(AddProjectCommand request, CancellationToken cancellationToken)
	{
		// Wrap code generation + insert in one transaction so the per-BU
		// counter and the row commit or roll back together - no gaps, no dups.
		await using var transaction = await Context.Database.BeginTransactionAsync(cancellationToken);

		var businessUnitCode = await Context.BusinessUnit.AsNoTracking()
			.Where(b => b.Id == request.BusinessUnitId)
			.Select(b => b.Code)
			.FirstOrDefaultAsync(cancellationToken) ?? "";
		var next = await SequenceGenerator.NextAsync(Context, SequenceKeys.ProjectCode(request.BusinessUnitId), cancellationToken);
		var projectCode = CodeFormats.Project(businessUnitCode, next);

		ProjectState entity = Mapper.Map<ProjectState>(request) with { ProjectCode = projectCode };
		AddEntitySubCollection<ProjectState, TeamMembersState>(entity, nameof(request.TeamMembersList));
		AddEntitySubCollection<ProjectState, ProjectAttachmentState>(entity, nameof(request.ProjectAttachmentList));
		_ = await Context.AddAsync(entity, cancellationToken);
		// Dynamic approval routing: resolve approvers for the project's Delivery Tower.
		await ApprovalHelper.AddApprovers(Context, identityContext, ApprovalModule.Project, entity.Id, cancellationToken, entity.DeliveryCategory);
		_ = await Context.SaveChangesAsync(cancellationToken);
		await transaction.CommitAsync(cancellationToken);
		return Success<Error, ProjectState>(entity);
	}
	
}

public class AddProjectCommandValidator : AbstractValidator<AddProjectCommand>
{
    readonly ApplicationContext _context;

    public AddProjectCommandValidator(ApplicationContext context)
    {
        _context = context;

        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.NotExists<ProjectState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("Project with id {PropertyValue} already exists");
        // ProjectCode is auto-generated in the handler, so no user-facing rule here.

    }
}
