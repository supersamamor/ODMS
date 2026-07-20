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

public record AddTeamMembersHistoryCommand : TeamMembersHistoryState, IRequest<Validation<Error, TeamMembersHistoryState>>;

public class AddTeamMembersHistoryCommandHandler(ApplicationContext context,
                                IMapper mapper,
                                CompositeValidator<AddTeamMembersHistoryCommand> validator) : BaseCommandHandler<ApplicationContext, TeamMembersHistoryState, AddTeamMembersHistoryCommand>(context, mapper, validator), IRequestHandler<AddTeamMembersHistoryCommand, Validation<Error, TeamMembersHistoryState>>
{
    
public async Task<Validation<Error, TeamMembersHistoryState>> Handle(AddTeamMembersHistoryCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await Add(request, cancellationToken));
	
}

public class AddTeamMembersHistoryCommandValidator : AbstractValidator<AddTeamMembersHistoryCommand>
{
    readonly ApplicationContext _context;

    public AddTeamMembersHistoryCommandValidator(ApplicationContext context)
    {
        _context = context;

        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.NotExists<TeamMembersHistoryState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("TeamMembersHistory with id {PropertyValue} already exists");
        
    }
}
