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

namespace FBSC.ODMS.Application.Features.ODMS.TeamMembersHistory.Commands;

public record EditTeamMembersHistoryCommand : TeamMembersHistoryState, IRequest<Validation<Error, TeamMembersHistoryState>>;

public class EditTeamMembersHistoryCommandHandler(ApplicationContext context,
                                 IMapper mapper,
                                 CompositeValidator<EditTeamMembersHistoryCommand> validator) : BaseCommandHandler<ApplicationContext, TeamMembersHistoryState, EditTeamMembersHistoryCommand>(context, mapper, validator), IRequestHandler<EditTeamMembersHistoryCommand, Validation<Error, TeamMembersHistoryState>>
{ 
    
public async Task<Validation<Error, TeamMembersHistoryState>> Handle(EditTeamMembersHistoryCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await Edit(request, cancellationToken));
	
}

public class EditTeamMembersHistoryCommandValidator : AbstractValidator<EditTeamMembersHistoryCommand>
{
    readonly ApplicationContext _context;

    public EditTeamMembersHistoryCommandValidator(ApplicationContext context)
    {
        _context = context;
		RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<TeamMembersHistoryState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("TeamMembersHistory with id {PropertyValue} does not exists");
        
    }
}
