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

namespace FBSC.ODMS.Application.Features.ODMS.AiSqlGenerationRequest.Commands;

public record EditAiSqlGenerationRequestCommand : AiSqlGenerationRequestState, IRequest<Validation<Error, AiSqlGenerationRequestState>>;

public class EditAiSqlGenerationRequestCommandHandler(ApplicationContext context,
                                 IMapper mapper,
                                 CompositeValidator<EditAiSqlGenerationRequestCommand> validator) : BaseCommandHandler<ApplicationContext, AiSqlGenerationRequestState, EditAiSqlGenerationRequestCommand>(context, mapper, validator), IRequestHandler<EditAiSqlGenerationRequestCommand, Validation<Error, AiSqlGenerationRequestState>>
{ 
    
public async Task<Validation<Error, AiSqlGenerationRequestState>> Handle(EditAiSqlGenerationRequestCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await Edit(request, cancellationToken));
	
}

public class EditAiSqlGenerationRequestCommandValidator : AbstractValidator<EditAiSqlGenerationRequestCommand>
{
    readonly ApplicationContext _context;

    public EditAiSqlGenerationRequestCommandValidator(ApplicationContext context)
    {
        _context = context;
		RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<AiSqlGenerationRequestState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("AiSqlGenerationRequest with id {PropertyValue} does not exists");
        
    }
}
