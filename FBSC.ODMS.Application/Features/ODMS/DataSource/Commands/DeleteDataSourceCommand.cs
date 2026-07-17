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

namespace FBSC.ODMS.Application.Features.ODMS.DataSource.Commands;

public record DeleteDataSourceCommand : BaseCommand, IRequest<Validation<Error, DataSourceState>>;

public class DeleteDataSourceCommandHandler(ApplicationContext context,
                                   IMapper mapper,
                                   CompositeValidator<DeleteDataSourceCommand> validator) : BaseCommandHandler<ApplicationContext, DataSourceState, DeleteDataSourceCommand>(context, mapper, validator), IRequestHandler<DeleteDataSourceCommand, Validation<Error, DataSourceState>>
{ 
    public async Task<Validation<Error, DataSourceState>> Handle(DeleteDataSourceCommand request, CancellationToken cancellationToken) =>
        await Validators.ValidateTAsync(request, cancellationToken).BindT(
            async request => await Delete(request.Id, cancellationToken));
}


public class DeleteDataSourceCommandValidator : AbstractValidator<DeleteDataSourceCommand>
{
    readonly ApplicationContext _context;

    public DeleteDataSourceCommandValidator(ApplicationContext context)
    {
        _context = context;
        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<DataSourceState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("DataSource with id {PropertyValue} does not exists");
    }
}
