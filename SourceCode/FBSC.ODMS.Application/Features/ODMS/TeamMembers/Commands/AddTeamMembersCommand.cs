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

namespace FBSC.ODMS.Application.Features.ODMS.TeamMembers.Commands;

public record AddTeamMembersCommand : TeamMembersState, IRequest<Validation<Error, TeamMembersState>>;

public class AddTeamMembersCommandHandler(ApplicationContext context,
                                IMapper mapper,
                                CompositeValidator<AddTeamMembersCommand> validator) : BaseCommandHandler<ApplicationContext, TeamMembersState, AddTeamMembersCommand>(context, mapper, validator), IRequestHandler<AddTeamMembersCommand, Validation<Error, TeamMembersState>>
{
    
public async Task<Validation<Error, TeamMembersState>> Handle(AddTeamMembersCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await Add(request, cancellationToken));
	
}

public class AddTeamMembersCommandValidator : AbstractValidator<AddTeamMembersCommand>
{
    readonly ApplicationContext _context;

    public AddTeamMembersCommandValidator(ApplicationContext context)
    {
        _context = context;

        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.NotExists<TeamMembersState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("TeamMembers with id {PropertyValue} already exists");
        
    }
}
