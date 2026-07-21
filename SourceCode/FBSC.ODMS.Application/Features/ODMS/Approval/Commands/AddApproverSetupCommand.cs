using AutoMapper;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using FBSC.Common.Core.Commands;
using FBSC.Common.Data;
using FBSC.Common.Utility.Validators;
using FluentValidation;
using LanguageExt;
using LanguageExt.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static LanguageExt.Prelude;

namespace FBSC.ODMS.Application.Features.ODMS.Approval.Commands;

public record AddApproverSetupCommand : ApproverSetupState, IRequest<Validation<Error, ApproverSetupState>>;

public class AddApproverSetupCommandHandler(ApplicationContext context,
                                IMapper mapper,
                                CompositeValidator<AddApproverSetupCommand> validator) : BaseCommandHandler<ApplicationContext, ApproverSetupState, AddApproverSetupCommand>(context, mapper, validator), IRequestHandler<AddApproverSetupCommand, Validation<Error, ApproverSetupState>>
{
    public async Task<Validation<Error, ApproverSetupState>> Handle(AddApproverSetupCommand request, CancellationToken cancellationToken) =>
        await Validators.ValidateTAsync(request, cancellationToken).BindT(
            async request => await AddApproverSetup(request, cancellationToken));


    public async Task<Validation<Error, ApproverSetupState>> AddApproverSetup(AddApproverSetupCommand request, CancellationToken cancellationToken)
    {
        ApproverSetupState entity = Mapper.Map<ApproverSetupState>(request);
        UpdateApproverAssignmentList(entity);
        _ = await Context.AddAsync(entity, cancellationToken);
        _ = await Context.SaveChangesAsync(cancellationToken);
        return Success<Error, ApproverSetupState>(entity);
    }

    private void UpdateApproverAssignmentList(ApproverSetupState entity)
    {
        if (entity.ApproverAssignmentList?.Count > 0)
        {
            foreach (var approverAssignment in entity.ApproverAssignmentList!)
            {
                Context.Entry(approverAssignment).State = EntityState.Added;
            }
        }
    }

}

public class AddApproverSetupCommandValidator : AbstractValidator<AddApproverSetupCommand>
{
    readonly ApplicationContext _context;

    public AddApproverSetupCommandValidator(ApplicationContext context)
    {
        _context = context;

        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.NotExists<ApproverSetupState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("ApproverSetup with id {PropertyValue} already exists");
        // Uniqueness is on the composite (TableName, DeliveryCategory) key: the same
        // table may have separate setups per Delivery Category, so both must match
        // an existing row before it counts as a duplicate.
        RuleFor(x => x.TableName)
            .MustAsync(async (command, tableName, cancellation) =>
                await _context.NotExists<ApproverSetupState>(
                    x => x.TableName == command.TableName && x.DeliveryCategory == command.DeliveryCategory,
                    cancellationToken: cancellation))
            .WithMessage("An Approver Setup for table '{PropertyValue}' and the selected Delivery Category already exists.");

    }
}
