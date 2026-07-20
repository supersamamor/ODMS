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

namespace FBSC.ODMS.Application.Features.ODMS.BusinessUnit.Commands;

public record DeleteBusinessUnitCommand : BaseCommand, IRequest<Validation<Error, BusinessUnitState>>;

public class DeleteBusinessUnitCommandHandler(ApplicationContext context,
                                   IMapper mapper,
                                   CompositeValidator<DeleteBusinessUnitCommand> validator) : BaseCommandHandler<ApplicationContext, BusinessUnitState, DeleteBusinessUnitCommand>(context, mapper, validator), IRequestHandler<DeleteBusinessUnitCommand, Validation<Error, BusinessUnitState>>
{ 
    public async Task<Validation<Error, BusinessUnitState>> Handle(DeleteBusinessUnitCommand request, CancellationToken cancellationToken) =>
        await Validators.ValidateTAsync(request, cancellationToken).BindT(
            async request => await Delete(request.Id, cancellationToken));
}


public class DeleteBusinessUnitCommandValidator : AbstractValidator<DeleteBusinessUnitCommand>
{
    readonly ApplicationContext _context;

    public DeleteBusinessUnitCommandValidator(ApplicationContext context)
    {
        _context = context;
        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<BusinessUnitState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("BusinessUnit with id {PropertyValue} does not exists");
    }
}
