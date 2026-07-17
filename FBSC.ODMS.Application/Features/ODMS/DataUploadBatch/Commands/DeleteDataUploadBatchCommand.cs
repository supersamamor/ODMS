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

namespace FBSC.ODMS.Application.Features.ODMS.DataUploadBatch.Commands;

public record DeleteDataUploadBatchCommand : BaseCommand, IRequest<Validation<Error, DataUploadBatchState>>;

public class DeleteDataUploadBatchCommandHandler(ApplicationContext context,
                                   IMapper mapper,
                                   CompositeValidator<DeleteDataUploadBatchCommand> validator) : BaseCommandHandler<ApplicationContext, DataUploadBatchState, DeleteDataUploadBatchCommand>(context, mapper, validator), IRequestHandler<DeleteDataUploadBatchCommand, Validation<Error, DataUploadBatchState>>
{ 
    public async Task<Validation<Error, DataUploadBatchState>> Handle(DeleteDataUploadBatchCommand request, CancellationToken cancellationToken) =>
        await Validators.ValidateTAsync(request, cancellationToken).BindT(
            async request => await Delete(request.Id, cancellationToken));
}


public class DeleteDataUploadBatchCommandValidator : AbstractValidator<DeleteDataUploadBatchCommand>
{
    readonly ApplicationContext _context;

    public DeleteDataUploadBatchCommandValidator(ApplicationContext context)
    {
        _context = context;
        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<DataUploadBatchState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("DataUploadBatch with id {PropertyValue} does not exists");
    }
}
