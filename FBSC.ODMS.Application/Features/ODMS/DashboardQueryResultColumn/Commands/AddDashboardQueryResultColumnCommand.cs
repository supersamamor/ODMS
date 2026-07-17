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

namespace FBSC.ODMS.Application.Features.ODMS.DashboardQueryResultColumn.Commands;

public record AddDashboardQueryResultColumnCommand : DashboardQueryResultColumnState, IRequest<Validation<Error, DashboardQueryResultColumnState>>;

public class AddDashboardQueryResultColumnCommandHandler(ApplicationContext context,
                                IMapper mapper,
                                CompositeValidator<AddDashboardQueryResultColumnCommand> validator) : BaseCommandHandler<ApplicationContext, DashboardQueryResultColumnState, AddDashboardQueryResultColumnCommand>(context, mapper, validator), IRequestHandler<AddDashboardQueryResultColumnCommand, Validation<Error, DashboardQueryResultColumnState>>
{
    
public async Task<Validation<Error, DashboardQueryResultColumnState>> Handle(AddDashboardQueryResultColumnCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await Add(request, cancellationToken));
	
}

public class AddDashboardQueryResultColumnCommandValidator : AbstractValidator<AddDashboardQueryResultColumnCommand>
{
    readonly ApplicationContext _context;

    public AddDashboardQueryResultColumnCommandValidator(ApplicationContext context)
    {
        _context = context;

        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.NotExists<DashboardQueryResultColumnState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("DashboardQueryResultColumn with id {PropertyValue} already exists");
        
    }
}
