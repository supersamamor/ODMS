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

public record EditMilestoneCommand : MilestoneState, IRequest<Validation<Error, MilestoneState>>;

public class EditMilestoneCommandHandler(ApplicationContext context,
                                 IMapper mapper,
                                 CompositeValidator<EditMilestoneCommand> validator) : BaseCommandHandler<ApplicationContext, MilestoneState, EditMilestoneCommand>(context, mapper, validator), IRequestHandler<EditMilestoneCommand, Validation<Error, MilestoneState>>
{ 
    public async Task<Validation<Error, MilestoneState>> Handle(EditMilestoneCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await EditMilestone(request, cancellationToken));


	public async Task<Validation<Error, MilestoneState>> EditMilestone(EditMilestoneCommand request, CancellationToken cancellationToken)
	{
		var entity = await Context.Milestone.Where(l => l.Id == request.Id).SingleAsync(cancellationToken: cancellationToken);
		Mapper.Map(request, entity);		
		Context.Update(entity);
		_ = await Context.SaveChangesAsync(cancellationToken);
		return Success<Error, MilestoneState>(entity);
	}
	
}

public class EditMilestoneCommandValidator : AbstractValidator<EditMilestoneCommand>
{
    readonly ApplicationContext _context;

    public EditMilestoneCommandValidator(ApplicationContext context)
    {
        _context = context;
		RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<MilestoneState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("Milestone with id {PropertyValue} does not exists");
        RuleFor(x => x.Name).MustAsync(async (request, name, cancellation) => await _context.NotExists<MilestoneState>(x => x.Name == name && x.Id != request.Id, cancellationToken: cancellation)).WithMessage("Milestone with name {PropertyValue} already exists");
	
    }
}
