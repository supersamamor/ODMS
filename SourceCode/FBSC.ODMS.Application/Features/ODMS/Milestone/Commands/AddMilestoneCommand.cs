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

namespace FBSC.ODMS.Application.Features.ODMS.Milestone.Commands;

public record AddMilestoneCommand : MilestoneState, IRequest<Validation<Error, MilestoneState>>;

public class AddMilestoneCommandHandler(ApplicationContext context,
                                IMapper mapper,
                                CompositeValidator<AddMilestoneCommand> validator) : BaseCommandHandler<ApplicationContext, MilestoneState, AddMilestoneCommand>(context, mapper, validator), IRequestHandler<AddMilestoneCommand, Validation<Error, MilestoneState>>
{
    public async Task<Validation<Error, MilestoneState>> Handle(AddMilestoneCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await AddMilestone(request, cancellationToken));


	public async Task<Validation<Error, MilestoneState>> AddMilestone(AddMilestoneCommand request, CancellationToken cancellationToken)
	{
		MilestoneState entity = Mapper.Map<MilestoneState>(request);	
		_ = await Context.AddAsync(entity, cancellationToken);
		_ = await Context.SaveChangesAsync(cancellationToken);
		return Success<Error, MilestoneState>(entity);
	}
	
}

public class AddMilestoneCommandValidator : AbstractValidator<AddMilestoneCommand>
{
    readonly ApplicationContext _context;

    public AddMilestoneCommandValidator(ApplicationContext context)
    {
        _context = context;

        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.NotExists<MilestoneState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("Milestone with id {PropertyValue} already exists");
        RuleFor(x => x.Name).MustAsync(async (name, cancellation) => await _context.NotExists<MilestoneState>(x => x.Name == name, cancellationToken: cancellation)).WithMessage("Milestone with name {PropertyValue} already exists");
	
    }
}
