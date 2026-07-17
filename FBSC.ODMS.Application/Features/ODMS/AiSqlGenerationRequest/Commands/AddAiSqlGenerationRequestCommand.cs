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

public record AddAiSqlGenerationRequestCommand : AiSqlGenerationRequestState, IRequest<Validation<Error, AiSqlGenerationRequestState>>;

public class AddAiSqlGenerationRequestCommandHandler(ApplicationContext context,
                                IMapper mapper,
                                CompositeValidator<AddAiSqlGenerationRequestCommand> validator) : BaseCommandHandler<ApplicationContext, AiSqlGenerationRequestState, AddAiSqlGenerationRequestCommand>(context, mapper, validator), IRequestHandler<AddAiSqlGenerationRequestCommand, Validation<Error, AiSqlGenerationRequestState>>
{
    
public async Task<Validation<Error, AiSqlGenerationRequestState>> Handle(AddAiSqlGenerationRequestCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await Add(request, cancellationToken));
	
}

public class AddAiSqlGenerationRequestCommandValidator : AbstractValidator<AddAiSqlGenerationRequestCommand>
{
    readonly ApplicationContext _context;

    public AddAiSqlGenerationRequestCommandValidator(ApplicationContext context)
    {
        _context = context;

        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.NotExists<AiSqlGenerationRequestState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("AiSqlGenerationRequest with id {PropertyValue} already exists");
        
    }
}
