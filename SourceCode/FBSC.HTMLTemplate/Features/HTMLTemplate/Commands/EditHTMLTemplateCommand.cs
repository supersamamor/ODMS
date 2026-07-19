using AutoMapper;
using FBSC.Common.Core.Commands;
using FBSC.Common.Data;
using FBSC.Common.Utility.Validators;
using FBSC.HTMLTemplate.Context;
using FBSC.HTMLTemplate.Models;
using FluentValidation;
using LanguageExt;
using LanguageExt.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static LanguageExt.Prelude;

namespace FBSC.HTMLTemplate.Features.HTMLTemplate.Commands;

public record EditHTMLTemplateCommand : HTMLTemplateState, IRequest<Validation<Error, HTMLTemplateState>>;

public class EditHTMLTemplateCommandHandler(HTMLTemplateContext context,
                                 IMapper mapper,
                                 CompositeValidator<EditHTMLTemplateCommand> validator) : BaseCommandHandler<HTMLTemplateContext, HTMLTemplateState, EditHTMLTemplateCommand>(context, mapper, validator), IRequestHandler<EditHTMLTemplateCommand, Validation<Error, HTMLTemplateState>>
{ 
    
public async Task<Validation<Error, HTMLTemplateState>> Handle(EditHTMLTemplateCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await Edit(request, cancellationToken));
	
}

public class EditHTMLTemplateCommandValidator : AbstractValidator<EditHTMLTemplateCommand>
{
    readonly HTMLTemplateContext _context;

    public EditHTMLTemplateCommandValidator(HTMLTemplateContext context)
    {
        _context = context;
		RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<HTMLTemplateState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("HTMLTemplate with id {PropertyValue} does not exists");
        RuleFor(x => x.HTMLTemplateName).MustAsync(async (request, documentFormName, cancellation) => await _context.NotExists<HTMLTemplateState>(x => x.HTMLTemplateName == documentFormName && x.Id != request.Id, cancellationToken: cancellation)).WithMessage("HTMLTemplate with documentFormName {PropertyValue} already exists");
	
    }
}
