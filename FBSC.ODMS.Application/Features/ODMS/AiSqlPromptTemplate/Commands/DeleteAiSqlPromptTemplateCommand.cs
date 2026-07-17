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

namespace FBSC.ODMS.Application.Features.ODMS.AiSqlPromptTemplate.Commands;

public record DeleteAiSqlPromptTemplateCommand : BaseCommand, IRequest<Validation<Error, AiSqlPromptTemplateState>>;

public class DeleteAiSqlPromptTemplateCommandHandler(ApplicationContext context,
                                   IMapper mapper,
                                   CompositeValidator<DeleteAiSqlPromptTemplateCommand> validator) : BaseCommandHandler<ApplicationContext, AiSqlPromptTemplateState, DeleteAiSqlPromptTemplateCommand>(context, mapper, validator), IRequestHandler<DeleteAiSqlPromptTemplateCommand, Validation<Error, AiSqlPromptTemplateState>>
{ 
    public async Task<Validation<Error, AiSqlPromptTemplateState>> Handle(DeleteAiSqlPromptTemplateCommand request, CancellationToken cancellationToken) =>
        await Validators.ValidateTAsync(request, cancellationToken).BindT(
            async request => await Delete(request.Id, cancellationToken));
}


public class DeleteAiSqlPromptTemplateCommandValidator : AbstractValidator<DeleteAiSqlPromptTemplateCommand>
{
    readonly ApplicationContext _context;

    public DeleteAiSqlPromptTemplateCommandValidator(ApplicationContext context)
    {
        _context = context;
        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<AiSqlPromptTemplateState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("AiSqlPromptTemplate with id {PropertyValue} does not exists");
    }
}
