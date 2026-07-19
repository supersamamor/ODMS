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

namespace FBSC.HTMLTemplate.Features.HTMLTemplate.Commands;

public record DeleteHTMLTemplateCommand : BaseCommand, IRequest<Validation<Error, HTMLTemplateState>>;

public class DeleteHTMLTemplateCommandHandler(HTMLTemplateContext context,
                                   IMapper mapper,
                                   CompositeValidator<DeleteHTMLTemplateCommand> validator) : BaseCommandHandler<HTMLTemplateContext, HTMLTemplateState, DeleteHTMLTemplateCommand>(context, mapper, validator), IRequestHandler<DeleteHTMLTemplateCommand, Validation<Error, HTMLTemplateState>>
{ 
    public async Task<Validation<Error, HTMLTemplateState>> Handle(DeleteHTMLTemplateCommand request, CancellationToken cancellationToken) =>
        await Validators.ValidateTAsync(request, cancellationToken).BindT(
            async request => await Delete(request.Id, cancellationToken));
}


public class DeleteHTMLTemplateCommandValidator : AbstractValidator<DeleteHTMLTemplateCommand>
{
    readonly HTMLTemplateContext _context;

    public DeleteHTMLTemplateCommandValidator(HTMLTemplateContext context)
    {
        _context = context;
        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<HTMLTemplateState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("HTMLTemplate with id {PropertyValue} does not exists");
    }
}
