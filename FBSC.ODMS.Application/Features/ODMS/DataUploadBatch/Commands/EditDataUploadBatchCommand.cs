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

namespace FBSC.ODMS.Application.Features.ODMS.DataUploadBatch.Commands;

public record EditDataUploadBatchCommand : DataUploadBatchState, IRequest<Validation<Error, DataUploadBatchState>>;

public class EditDataUploadBatchCommandHandler(ApplicationContext context,
                                 IMapper mapper,
                                 CompositeValidator<EditDataUploadBatchCommand> validator) : BaseCommandHandler<ApplicationContext, DataUploadBatchState, EditDataUploadBatchCommand>(context, mapper, validator), IRequestHandler<EditDataUploadBatchCommand, Validation<Error, DataUploadBatchState>>
{ 
    public async Task<Validation<Error, DataUploadBatchState>> Handle(EditDataUploadBatchCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await EditDataUploadBatch(request, cancellationToken));


	public async Task<Validation<Error, DataUploadBatchState>> EditDataUploadBatch(EditDataUploadBatchCommand request, CancellationToken cancellationToken)
	{
		var entity = await Context.DataUploadBatch.Where(l => l.Id == request.Id).SingleAsync(cancellationToken: cancellationToken);
		Mapper.Map(request, entity);
		await UpdateEntitySubCollectionAsync<DataUploadBatchState, DataUploadColumnState>(request.Id, nameof(DataUploadColumnState.DataUploadBatchId), nameof(request.DataUploadColumnList), entity, cancellationToken);
		Context.Update(entity);
		_ = await Context.SaveChangesAsync(cancellationToken);
		return Success<Error, DataUploadBatchState>(entity);
	}
	
}

public class EditDataUploadBatchCommandValidator : AbstractValidator<EditDataUploadBatchCommand>
{
    readonly ApplicationContext _context;

    public EditDataUploadBatchCommandValidator(ApplicationContext context)
    {
        _context = context;
		RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<DataUploadBatchState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("DataUploadBatch with id {PropertyValue} does not exists");
        
    }
}
