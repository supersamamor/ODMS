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

public record AddDataUploadBatchCommand : DataUploadBatchState, IRequest<Validation<Error, DataUploadBatchState>>;

public class AddDataUploadBatchCommandHandler(ApplicationContext context,
                                IMapper mapper,
                                CompositeValidator<AddDataUploadBatchCommand> validator) : BaseCommandHandler<ApplicationContext, DataUploadBatchState, AddDataUploadBatchCommand>(context, mapper, validator), IRequestHandler<AddDataUploadBatchCommand, Validation<Error, DataUploadBatchState>>
{
    public async Task<Validation<Error, DataUploadBatchState>> Handle(AddDataUploadBatchCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await AddDataUploadBatch(request, cancellationToken));


	public async Task<Validation<Error, DataUploadBatchState>> AddDataUploadBatch(AddDataUploadBatchCommand request, CancellationToken cancellationToken)
	{
		DataUploadBatchState entity = Mapper.Map<DataUploadBatchState>(request);
		AddEntitySubCollection<DataUploadBatchState, DataUploadColumnState>(entity, nameof(request.DataUploadColumnList));
		_ = await Context.AddAsync(entity, cancellationToken);
		_ = await Context.SaveChangesAsync(cancellationToken);
		return Success<Error, DataUploadBatchState>(entity);
	}
	
}

public class AddDataUploadBatchCommandValidator : AbstractValidator<AddDataUploadBatchCommand>
{
    readonly ApplicationContext _context;

    public AddDataUploadBatchCommandValidator(ApplicationContext context)
    {
        _context = context;

        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.NotExists<DataUploadBatchState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("DataUploadBatch with id {PropertyValue} already exists");
        
    }
}
