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

public record AddProjectCommand : ProjectState, IRequest<Validation<Error, ProjectState>>;

public class AddProjectCommandHandler(ApplicationContext context,
                                IMapper mapper,
                                CompositeValidator<AddProjectCommand> validator) : BaseCommandHandler<ApplicationContext, ProjectState, AddProjectCommand>(context, mapper, validator), IRequestHandler<AddProjectCommand, Validation<Error, ProjectState>>
{
    public async Task<Validation<Error, ProjectState>> Handle(AddProjectCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await AddProject(request, cancellationToken));


	public async Task<Validation<Error, ProjectState>> AddProject(AddProjectCommand request, CancellationToken cancellationToken)
	{
		ProjectState entity = Mapper.Map<ProjectState>(request);
		AddEntitySubCollection<ProjectState, TeamMembersState>(entity, nameof(request.TeamMembersList));
		_ = await Context.AddAsync(entity, cancellationToken);
		_ = await Context.SaveChangesAsync(cancellationToken);
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
        RuleFor(x => x.ProjectCode).MustAsync(async (code, cancellation) => await _context.NotExists<ProjectState>(x => x.ProjectCode == code, cancellationToken: cancellation))
                          .WithMessage("Project ID {PropertyValue} is already in use");

    }
}
