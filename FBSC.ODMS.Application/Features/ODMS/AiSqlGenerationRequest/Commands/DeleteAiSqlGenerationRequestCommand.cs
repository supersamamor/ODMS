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

namespace FBSC.ODMS.Application.Features.ODMS.AiSqlGenerationRequest.Commands;

public record DeleteAiSqlGenerationRequestCommand : BaseCommand, IRequest<Validation<Error, AiSqlGenerationRequestState>>;

public class DeleteAiSqlGenerationRequestCommandHandler(ApplicationContext context,
                                   IMapper mapper,
                                   CompositeValidator<DeleteAiSqlGenerationRequestCommand> validator) : BaseCommandHandler<ApplicationContext, AiSqlGenerationRequestState, DeleteAiSqlGenerationRequestCommand>(context, mapper, validator), IRequestHandler<DeleteAiSqlGenerationRequestCommand, Validation<Error, AiSqlGenerationRequestState>>
{ 
    public async Task<Validation<Error, AiSqlGenerationRequestState>> Handle(DeleteAiSqlGenerationRequestCommand request, CancellationToken cancellationToken) =>
        await Validators.ValidateTAsync(request, cancellationToken).BindT(
            async request => await Delete(request.Id, cancellationToken));
}


public class DeleteAiSqlGenerationRequestCommandValidator : AbstractValidator<DeleteAiSqlGenerationRequestCommand>
{
    readonly ApplicationContext _context;

    public DeleteAiSqlGenerationRequestCommandValidator(ApplicationContext context)
    {
        _context = context;
        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<AiSqlGenerationRequestState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("AiSqlGenerationRequest with id {PropertyValue} does not exists");
    }
}
