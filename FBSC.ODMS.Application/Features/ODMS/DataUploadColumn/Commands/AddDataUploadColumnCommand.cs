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

public record AddDataUploadColumnCommand : DataUploadColumnState, IRequest<Validation<Error, DataUploadColumnState>>;

public class AddDataUploadColumnCommandHandler(ApplicationContext context,
                                IMapper mapper,
                                CompositeValidator<AddDataUploadColumnCommand> validator) : BaseCommandHandler<ApplicationContext, DataUploadColumnState, AddDataUploadColumnCommand>(context, mapper, validator), IRequestHandler<AddDataUploadColumnCommand, Validation<Error, DataUploadColumnState>>
{
    
public async Task<Validation<Error, DataUploadColumnState>> Handle(AddDataUploadColumnCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await Add(request, cancellationToken));
	
}

public class AddDataUploadColumnCommandValidator : AbstractValidator<AddDataUploadColumnCommand>
{
    readonly ApplicationContext _context;

    public AddDataUploadColumnCommandValidator(ApplicationContext context)
    {
        _context = context;

        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.NotExists<DataUploadColumnState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("DataUploadColumn with id {PropertyValue} already exists");
        
    }
}
