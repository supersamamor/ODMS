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

namespace FBSC.ODMS.Application.Features.ODMS.AiSqlPromptTemplate.Commands;

public record EditAiSqlPromptTemplateCommand : AiSqlPromptTemplateState, IRequest<Validation<Error, AiSqlPromptTemplateState>>;

public class EditAiSqlPromptTemplateCommandHandler(ApplicationContext context,
                                 IMapper mapper,
                                 CompositeValidator<EditAiSqlPromptTemplateCommand> validator) : BaseCommandHandler<ApplicationContext, AiSqlPromptTemplateState, EditAiSqlPromptTemplateCommand>(context, mapper, validator), IRequestHandler<EditAiSqlPromptTemplateCommand, Validation<Error, AiSqlPromptTemplateState>>
{ 
    
public async Task<Validation<Error, AiSqlPromptTemplateState>> Handle(EditAiSqlPromptTemplateCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await Edit(request, cancellationToken));
	
}

public class EditAiSqlPromptTemplateCommandValidator : AbstractValidator<EditAiSqlPromptTemplateCommand>
{
    readonly ApplicationContext _context;

    public EditAiSqlPromptTemplateCommandValidator(ApplicationContext context)
    {
        _context = context;
		RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<AiSqlPromptTemplateState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("AiSqlPromptTemplate with id {PropertyValue} does not exists");
        
    }
}
