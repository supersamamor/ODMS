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

namespace FBSC.ODMS.Application.Features.ODMS.DataUploadColumn.Commands;

public record EditDataUploadColumnCommand : DataUploadColumnState, IRequest<Validation<Error, DataUploadColumnState>>;

public class EditDataUploadColumnCommandHandler(ApplicationContext context,
                                 IMapper mapper,
                                 CompositeValidator<EditDataUploadColumnCommand> validator) : BaseCommandHandler<ApplicationContext, DataUploadColumnState, EditDataUploadColumnCommand>(context, mapper, validator), IRequestHandler<EditDataUploadColumnCommand, Validation<Error, DataUploadColumnState>>
{ 
    
public async Task<Validation<Error, DataUploadColumnState>> Handle(EditDataUploadColumnCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await Edit(request, cancellationToken));
	
}

public class EditDataUploadColumnCommandValidator : AbstractValidator<EditDataUploadColumnCommand>
{
    readonly ApplicationContext _context;

    public EditDataUploadColumnCommandValidator(ApplicationContext context)
    {
        _context = context;
		RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<DataUploadColumnState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("DataUploadColumn with id {PropertyValue} does not exists");
        
    }
}
