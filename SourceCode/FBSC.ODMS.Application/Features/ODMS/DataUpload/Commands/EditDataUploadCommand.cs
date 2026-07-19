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

namespace FBSC.ODMS.Application.Features.ODMS.DataUpload.Commands;

public record EditDataUploadCommand : DataUploadState, IRequest<Validation<Error, DataUploadState>>;

public class EditDataUploadCommandHandler(ApplicationContext context,
                                 IMapper mapper,
                                 CompositeValidator<EditDataUploadCommand> validator) : BaseCommandHandler<ApplicationContext, DataUploadState, EditDataUploadCommand>(context, mapper, validator), IRequestHandler<EditDataUploadCommand, Validation<Error, DataUploadState>>
{ 
    
public async Task<Validation<Error, DataUploadState>> Handle(EditDataUploadCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await Edit(request, cancellationToken));
	
}

public class EditDataUploadCommandValidator : AbstractValidator<EditDataUploadCommand>
{
    readonly ApplicationContext _context;

    public EditDataUploadCommandValidator(ApplicationContext context)
    {
        _context = context;
		RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<DataUploadState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("DataUpload with id {PropertyValue} does not exists");
        RuleFor(x => x.Description).MustAsync(async (request, description, cancellation) => await _context.NotExists<DataUploadState>(x => x.Description == description && x.Id != request.Id, cancellationToken: cancellation)).WithMessage("DataUpload with description {PropertyValue} already exists");
	
    }
}
