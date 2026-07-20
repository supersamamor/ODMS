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

namespace FBSC.ODMS.Application.Features.ODMS.ProjectHistory.Commands;

public record DeleteProjectHistoryCommand : BaseCommand, IRequest<Validation<Error, ProjectHistoryState>>;

public class DeleteProjectHistoryCommandHandler(ApplicationContext context,
                                   IMapper mapper,
                                   CompositeValidator<DeleteProjectHistoryCommand> validator) : BaseCommandHandler<ApplicationContext, ProjectHistoryState, DeleteProjectHistoryCommand>(context, mapper, validator), IRequestHandler<DeleteProjectHistoryCommand, Validation<Error, ProjectHistoryState>>
{ 
    public async Task<Validation<Error, ProjectHistoryState>> Handle(DeleteProjectHistoryCommand request, CancellationToken cancellationToken) =>
        await Validators.ValidateTAsync(request, cancellationToken).BindT(
            async request => await Delete(request.Id, cancellationToken));
}


public class DeleteProjectHistoryCommandValidator : AbstractValidator<DeleteProjectHistoryCommand>
{
    readonly ApplicationContext _context;

    public DeleteProjectHistoryCommandValidator(ApplicationContext context)
    {
        _context = context;
        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<ProjectHistoryState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("ProjectHistory with id {PropertyValue} does not exists");
    }
}
