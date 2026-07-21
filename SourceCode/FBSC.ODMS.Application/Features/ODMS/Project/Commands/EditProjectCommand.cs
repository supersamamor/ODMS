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

namespace FBSC.ODMS.Application.Features.ODMS.Project.Commands;

public record EditProjectCommand : ProjectState, IRequest<Validation<Error, ProjectState>>;

public class EditProjectCommandHandler(ApplicationContext context,
                                 IMapper mapper,
                                 CompositeValidator<EditProjectCommand> validator) : BaseCommandHandler<ApplicationContext, ProjectState, EditProjectCommand>(context, mapper, validator), IRequestHandler<EditProjectCommand, Validation<Error, ProjectState>>
{ 
    public async Task<Validation<Error, ProjectState>> Handle(EditProjectCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await EditProject(request, cancellationToken));


	public async Task<Validation<Error, ProjectState>> EditProject(EditProjectCommand request, CancellationToken cancellationToken)
	{
		var entity = await Context.Project.Where(l => l.Id == request.Id).SingleAsync(cancellationToken: cancellationToken);
		Mapper.Map(request, entity);
		await UpdateEntitySubCollectionAsync<ProjectState, TeamMembersState>(request.Id, nameof(TeamMembersState.ProjectId), nameof(request.TeamMembersList), entity, cancellationToken);
		Context.Update(entity);
		_ = await Context.SaveChangesAsync(cancellationToken);
		return Success<Error, ProjectState>(entity);
	}
	
}

public class EditProjectCommandValidator : AbstractValidator<EditProjectCommand>
{
    readonly ApplicationContext _context;

    public EditProjectCommandValidator(ApplicationContext context)
    {
        _context = context;
		RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<ProjectState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("Project with id {PropertyValue} does not exists");
        RuleFor(x => x).MustAsync(async (cmd, cancellation) => await _context.NotExists<ProjectState>(x => x.ProjectCode == cmd.ProjectCode && x.Id != cmd.Id, cancellationToken: cancellation))
                          .WithMessage(cmd => $"Project ID {cmd.ProjectCode} is already in use");

    }
}
