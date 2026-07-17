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

public record AddAiSqlPromptTemplateCommand : AiSqlPromptTemplateState, IRequest<Validation<Error, AiSqlPromptTemplateState>>;

public class AddAiSqlPromptTemplateCommandHandler(ApplicationContext context,
                                IMapper mapper,
                                CompositeValidator<AddAiSqlPromptTemplateCommand> validator) : BaseCommandHandler<ApplicationContext, AiSqlPromptTemplateState, AddAiSqlPromptTemplateCommand>(context, mapper, validator), IRequestHandler<AddAiSqlPromptTemplateCommand, Validation<Error, AiSqlPromptTemplateState>>
{
    
public async Task<Validation<Error, AiSqlPromptTemplateState>> Handle(AddAiSqlPromptTemplateCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await Add(request, cancellationToken));
	
}

public class AddAiSqlPromptTemplateCommandValidator : AbstractValidator<AddAiSqlPromptTemplateCommand>
{
    readonly ApplicationContext _context;

    public AddAiSqlPromptTemplateCommandValidator(ApplicationContext context)
    {
        _context = context;

        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.NotExists<AiSqlPromptTemplateState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("AiSqlPromptTemplate with id {PropertyValue} already exists");
        
    }
}
