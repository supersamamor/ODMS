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

public record EditTeamMembersCommand : TeamMembersState, IRequest<Validation<Error, TeamMembersState>>;

public class EditTeamMembersCommandHandler(ApplicationContext context,
                                 IMapper mapper,
                                 CompositeValidator<EditTeamMembersCommand> validator) : BaseCommandHandler<ApplicationContext, TeamMembersState, EditTeamMembersCommand>(context, mapper, validator), IRequestHandler<EditTeamMembersCommand, Validation<Error, TeamMembersState>>
{ 
    
public async Task<Validation<Error, TeamMembersState>> Handle(EditTeamMembersCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await Edit(request, cancellationToken));
	
}

public class EditTeamMembersCommandValidator : AbstractValidator<EditTeamMembersCommand>
{
    readonly ApplicationContext _context;

    public EditTeamMembersCommandValidator(ApplicationContext context)
    {
        _context = context;
		RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<TeamMembersState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("TeamMembers with id {PropertyValue} does not exists");
        // Level and Role are mandatory - nulls must not slip through the direct
        // command path (Employee may legitimately be null = "(Unknown)").
        RuleFor(x => x.MemberLevel).NotEmpty().WithMessage("'Level' is required.");
        RuleFor(x => x.Role).NotEmpty().WithMessage("'Role' is required.");

    }
}
