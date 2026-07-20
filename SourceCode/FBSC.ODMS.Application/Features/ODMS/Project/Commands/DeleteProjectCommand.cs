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

namespace FBSC.ODMS.Application.Features.ODMS.Project.Commands;

public record DeleteProjectCommand : BaseCommand, IRequest<Validation<Error, ProjectState>>;

public class DeleteProjectCommandHandler(ApplicationContext context,
                                   IMapper mapper,
                                   CompositeValidator<DeleteProjectCommand> validator) : BaseCommandHandler<ApplicationContext, ProjectState, DeleteProjectCommand>(context, mapper, validator), IRequestHandler<DeleteProjectCommand, Validation<Error, ProjectState>>
{ 
    public async Task<Validation<Error, ProjectState>> Handle(DeleteProjectCommand request, CancellationToken cancellationToken) =>
        await Validators.ValidateTAsync(request, cancellationToken).BindT(
            async request => await Delete(request.Id, cancellationToken));
}


public class DeleteProjectCommandValidator : AbstractValidator<DeleteProjectCommand>
{
    readonly ApplicationContext _context;

    public DeleteProjectCommandValidator(ApplicationContext context)
    {
        _context = context;
        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<ProjectState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("Project with id {PropertyValue} does not exists");
    }
}
