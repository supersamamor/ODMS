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

namespace FBSC.ODMS.Application.Features.ODMS.TeamMembers.Commands;

public record DeleteTeamMembersCommand : BaseCommand, IRequest<Validation<Error, TeamMembersState>>;

public class DeleteTeamMembersCommandHandler(ApplicationContext context,
                                   IMapper mapper,
                                   CompositeValidator<DeleteTeamMembersCommand> validator) : BaseCommandHandler<ApplicationContext, TeamMembersState, DeleteTeamMembersCommand>(context, mapper, validator), IRequestHandler<DeleteTeamMembersCommand, Validation<Error, TeamMembersState>>
{ 
    public async Task<Validation<Error, TeamMembersState>> Handle(DeleteTeamMembersCommand request, CancellationToken cancellationToken) =>
        await Validators.ValidateTAsync(request, cancellationToken).BindT(
            async request => await Delete(request.Id, cancellationToken));
}


public class DeleteTeamMembersCommandValidator : AbstractValidator<DeleteTeamMembersCommand>
{
    readonly ApplicationContext _context;

    public DeleteTeamMembersCommandValidator(ApplicationContext context)
    {
        _context = context;
        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<TeamMembersState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("TeamMembers with id {PropertyValue} does not exists");
    }
}
