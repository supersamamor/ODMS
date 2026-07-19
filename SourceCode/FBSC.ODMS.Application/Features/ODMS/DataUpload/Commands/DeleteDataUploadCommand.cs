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

namespace FBSC.ODMS.Application.Features.ODMS.DataUpload.Commands;

public record DeleteDataUploadCommand : BaseCommand, IRequest<Validation<Error, DataUploadState>>;

public class DeleteDataUploadCommandHandler(ApplicationContext context,
                                   IMapper mapper,
                                   CompositeValidator<DeleteDataUploadCommand> validator) : BaseCommandHandler<ApplicationContext, DataUploadState, DeleteDataUploadCommand>(context, mapper, validator), IRequestHandler<DeleteDataUploadCommand, Validation<Error, DataUploadState>>
{ 
    public async Task<Validation<Error, DataUploadState>> Handle(DeleteDataUploadCommand request, CancellationToken cancellationToken) =>
        await Validators.ValidateTAsync(request, cancellationToken).BindT(
            async request => await Delete(request.Id, cancellationToken));
}


public class DeleteDataUploadCommandValidator : AbstractValidator<DeleteDataUploadCommand>
{
    readonly ApplicationContext _context;

    public DeleteDataUploadCommandValidator(ApplicationContext context)
    {
        _context = context;
        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<DataUploadState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("DataUpload with id {PropertyValue} does not exists");
    }
}
