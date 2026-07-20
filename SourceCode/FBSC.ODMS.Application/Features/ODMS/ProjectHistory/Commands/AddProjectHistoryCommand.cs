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

namespace FBSC.ODMS.Application.Features.ODMS.ProjectHistory.Commands;

public record AddProjectHistoryCommand : ProjectHistoryState, IRequest<Validation<Error, ProjectHistoryState>>;

public class AddProjectHistoryCommandHandler(ApplicationContext context,
                                IMapper mapper,
                                CompositeValidator<AddProjectHistoryCommand> validator) : BaseCommandHandler<ApplicationContext, ProjectHistoryState, AddProjectHistoryCommand>(context, mapper, validator), IRequestHandler<AddProjectHistoryCommand, Validation<Error, ProjectHistoryState>>
{
    public async Task<Validation<Error, ProjectHistoryState>> Handle(AddProjectHistoryCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await AddProjectHistory(request, cancellationToken));


	public async Task<Validation<Error, ProjectHistoryState>> AddProjectHistory(AddProjectHistoryCommand request, CancellationToken cancellationToken)
	{
		ProjectHistoryState entity = Mapper.Map<ProjectHistoryState>(request);
		AddEntitySubCollection<ProjectHistoryState, TeamMembersHistoryState>(entity, nameof(request.TeamMembersHistoryList));
		_ = await Context.AddAsync(entity, cancellationToken);
		_ = await Context.SaveChangesAsync(cancellationToken);
		return Success<Error, ProjectHistoryState>(entity);
	}
	
}

public class AddProjectHistoryCommandValidator : AbstractValidator<AddProjectHistoryCommand>
{
    readonly ApplicationContext _context;

    public AddProjectHistoryCommandValidator(ApplicationContext context)
    {
        _context = context;

        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.NotExists<ProjectHistoryState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("ProjectHistory with id {PropertyValue} already exists");
        
    }
}
