using AutoMapper;
using FBSC.Common.Core.Commands;
using FBSC.Common.Data;
using FBSC.Common.Utility.Validators;
using FBSC.HTMLTemplate.Models;
using FBSC.HTMLTemplate.Context;
using FluentValidation;
using LanguageExt;
using LanguageExt.Common;
using MediatR;
using static LanguageExt.Prelude;

namespace FBSC.HTMLTemplate.Features.HTMLTemplate.Commands;

public record AddHTMLTemplateCommand : HTMLTemplateState, IRequest<Validation<Error, HTMLTemplateState>>;

public class AddHTMLTemplateCommandHandler(HTMLTemplateContext context,
                                IMapper mapper,
                                CompositeValidator<AddHTMLTemplateCommand> validator) : BaseCommandHandler<HTMLTemplateContext, HTMLTemplateState, AddHTMLTemplateCommand>(context, mapper, validator), IRequestHandler<AddHTMLTemplateCommand, Validation<Error, HTMLTemplateState>>
{
    public async Task<Validation<Error, HTMLTemplateState>> Handle(AddHTMLTemplateCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await AddHTMLTemplate(request, cancellationToken));


	public async Task<Validation<Error, HTMLTemplateState>> AddHTMLTemplate(AddHTMLTemplateCommand request, CancellationToken cancellationToken)
	{
		HTMLTemplateState entity = Mapper.Map<HTMLTemplateState>(request);
		_ = await Context.AddAsync(entity, cancellationToken);
		_ = await Context.SaveChangesAsync(cancellationToken);
		return Success<Error, HTMLTemplateState>(entity);
	}
	
}

public class AddHTMLTemplateCommandValidator : AbstractValidator<AddHTMLTemplateCommand>
{
    readonly HTMLTemplateContext _context;

    public AddHTMLTemplateCommandValidator(HTMLTemplateContext context)
    {
        _context = context;

        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.NotExists<HTMLTemplateState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("HTMLTemplate with id {PropertyValue} already exists");
        RuleFor(x => x.HTMLTemplateName).MustAsync(async (documentFormName, cancellation) => await _context.NotExists<HTMLTemplateState>(x => x.HTMLTemplateName == documentFormName, cancellationToken: cancellation)).WithMessage("HTMLTemplate with documentFormName {PropertyValue} already exists");
	
    }
}
