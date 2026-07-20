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

namespace FBSC.ODMS.Application.Features.ODMS.TeamMembersHistory.Commands;

public record DeleteTeamMembersHistoryCommand : BaseCommand, IRequest<Validation<Error, TeamMembersHistoryState>>;

public class DeleteTeamMembersHistoryCommandHandler(ApplicationContext context,
                                   IMapper mapper,
                                   CompositeValidator<DeleteTeamMembersHistoryCommand> validator) : BaseCommandHandler<ApplicationContext, TeamMembersHistoryState, DeleteTeamMembersHistoryCommand>(context, mapper, validator), IRequestHandler<DeleteTeamMembersHistoryCommand, Validation<Error, TeamMembersHistoryState>>
{ 
    public async Task<Validation<Error, TeamMembersHistoryState>> Handle(DeleteTeamMembersHistoryCommand request, CancellationToken cancellationToken) =>
        await Validators.ValidateTAsync(request, cancellationToken).BindT(
            async request => await Delete(request.Id, cancellationToken));
}


public class DeleteTeamMembersHistoryCommandValidator : AbstractValidator<DeleteTeamMembersHistoryCommand>
{
    readonly ApplicationContext _context;

    public DeleteTeamMembersHistoryCommandValidator(ApplicationContext context)
    {
        _context = context;
        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<TeamMembersHistoryState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("TeamMembersHistory with id {PropertyValue} does not exists");
    }
}
