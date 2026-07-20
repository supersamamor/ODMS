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

public record EditProjectHistoryCommand : ProjectHistoryState, IRequest<Validation<Error, ProjectHistoryState>>;

public class EditProjectHistoryCommandHandler(ApplicationContext context,
                                 IMapper mapper,
                                 CompositeValidator<EditProjectHistoryCommand> validator) : BaseCommandHandler<ApplicationContext, ProjectHistoryState, EditProjectHistoryCommand>(context, mapper, validator), IRequestHandler<EditProjectHistoryCommand, Validation<Error, ProjectHistoryState>>
{ 
    public async Task<Validation<Error, ProjectHistoryState>> Handle(EditProjectHistoryCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await EditProjectHistory(request, cancellationToken));


	public async Task<Validation<Error, ProjectHistoryState>> EditProjectHistory(EditProjectHistoryCommand request, CancellationToken cancellationToken)
	{
		var entity = await Context.ProjectHistory.Where(l => l.Id == request.Id).SingleAsync(cancellationToken: cancellationToken);
		Mapper.Map(request, entity);
		await UpdateEntitySubCollectionAsync<ProjectHistoryState, TeamMembersHistoryState>(request.Id, nameof(TeamMembersHistoryState.ProjectHistoryId), nameof(request.TeamMembersHistoryList), entity, cancellationToken);
		Context.Update(entity);
		_ = await Context.SaveChangesAsync(cancellationToken);
		return Success<Error, ProjectHistoryState>(entity);
	}
	
}

public class EditProjectHistoryCommandValidator : AbstractValidator<EditProjectHistoryCommand>
{
    readonly ApplicationContext _context;

    public EditProjectHistoryCommandValidator(ApplicationContext context)
    {
        _context = context;
		RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<ProjectHistoryState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("ProjectHistory with id {PropertyValue} does not exists");
        
    }
}
