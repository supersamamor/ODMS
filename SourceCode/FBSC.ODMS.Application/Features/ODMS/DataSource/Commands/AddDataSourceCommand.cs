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

namespace FBSC.ODMS.Application.Features.ODMS.DataSource.Commands;

public record AddDataSourceCommand : DataSourceState, IRequest<Validation<Error, DataSourceState>>;

public class AddDataSourceCommandHandler(ApplicationContext context,
                                IMapper mapper,
                                CompositeValidator<AddDataSourceCommand> validator,IdentityContext identityContext) : BaseCommandHandler<ApplicationContext, DataSourceState, AddDataSourceCommand>(context, mapper, validator), IRequestHandler<AddDataSourceCommand, Validation<Error, DataSourceState>>
{
    public async Task<Validation<Error, DataSourceState>> Handle(AddDataSourceCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await AddDataSource(request, cancellationToken));


	public async Task<Validation<Error, DataSourceState>> AddDataSource(AddDataSourceCommand request, CancellationToken cancellationToken)
	{
		DataSourceState entity = Mapper.Map<DataSourceState>(request);
		_ = await Context.AddAsync(entity, cancellationToken);
		await Helpers.ApprovalHelper.AddApprovers(Context, identityContext, ApprovalModule.DataSource, entity.Id, cancellationToken);
		_ = await Context.SaveChangesAsync(cancellationToken);
		return Success<Error, DataSourceState>(entity);
	}
	
}

public class AddDataSourceCommandValidator : AbstractValidator<AddDataSourceCommand>
{
    readonly ApplicationContext _context;

    public AddDataSourceCommandValidator(ApplicationContext context)
    {
        _context = context;

        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.NotExists<DataSourceState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("DataSource with id {PropertyValue} already exists");
        RuleFor(x => x.Name).MustAsync(async (name, cancellation) => await _context.NotExists<DataSourceState>(x => x.Name == name, cancellationToken: cancellation)).WithMessage("DataSource with name {PropertyValue} already exists");
	
    }
}
