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

public record AddDataUploadCommand : DataUploadState, IRequest<Validation<Error, DataUploadState>>;

public class AddDataUploadCommandHandler(ApplicationContext context,
                                IMapper mapper,
                                CompositeValidator<AddDataUploadCommand> validator) : BaseCommandHandler<ApplicationContext, DataUploadState, AddDataUploadCommand>(context, mapper, validator), IRequestHandler<AddDataUploadCommand, Validation<Error, DataUploadState>>
{
    
public async Task<Validation<Error, DataUploadState>> Handle(AddDataUploadCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await Add(request, cancellationToken));
	
}

public class AddDataUploadCommandValidator : AbstractValidator<AddDataUploadCommand>
{
    readonly ApplicationContext _context;

    public AddDataUploadCommandValidator(ApplicationContext context)
    {
        _context = context;

        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.NotExists<DataUploadState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("DataUpload with id {PropertyValue} already exists");
        RuleFor(x => x.Description).MustAsync(async (description, cancellation) => await _context.NotExists<DataUploadState>(x => x.Description == description, cancellationToken: cancellation)).WithMessage("DataUpload with description {PropertyValue} already exists");
	
    }
}
