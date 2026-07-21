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

namespace FBSC.ODMS.Application.Features.ODMS.Milestone.Commands;

public record DeleteMilestoneCommand : BaseCommand, IRequest<Validation<Error, MilestoneState>>;

public class DeleteMilestoneCommandHandler(ApplicationContext context,
                                   IMapper mapper,
                                   CompositeValidator<DeleteMilestoneCommand> validator) : BaseCommandHandler<ApplicationContext, MilestoneState, DeleteMilestoneCommand>(context, mapper, validator), IRequestHandler<DeleteMilestoneCommand, Validation<Error, MilestoneState>>
{ 
    public async Task<Validation<Error, MilestoneState>> Handle(DeleteMilestoneCommand request, CancellationToken cancellationToken) =>
        await Validators.ValidateTAsync(request, cancellationToken).BindT(
            async request => await Delete(request.Id, cancellationToken));
}


public class DeleteMilestoneCommandValidator : AbstractValidator<DeleteMilestoneCommand>
{
    readonly ApplicationContext _context;

    public DeleteMilestoneCommandValidator(ApplicationContext context)
    {
        _context = context;
        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<MilestoneState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("Milestone with id {PropertyValue} does not exists");
    }
}
