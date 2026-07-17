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

public record EditDashboardQueryResultColumnCommand : DashboardQueryResultColumnState, IRequest<Validation<Error, DashboardQueryResultColumnState>>;

public class EditDashboardQueryResultColumnCommandHandler(ApplicationContext context,
                                 IMapper mapper,
                                 CompositeValidator<EditDashboardQueryResultColumnCommand> validator) : BaseCommandHandler<ApplicationContext, DashboardQueryResultColumnState, EditDashboardQueryResultColumnCommand>(context, mapper, validator), IRequestHandler<EditDashboardQueryResultColumnCommand, Validation<Error, DashboardQueryResultColumnState>>
{ 
    
public async Task<Validation<Error, DashboardQueryResultColumnState>> Handle(EditDashboardQueryResultColumnCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await Edit(request, cancellationToken));
	
}

public class EditDashboardQueryResultColumnCommandValidator : AbstractValidator<EditDashboardQueryResultColumnCommand>
{
    readonly ApplicationContext _context;

    public EditDashboardQueryResultColumnCommandValidator(ApplicationContext context)
    {
        _context = context;
		RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<DashboardQueryResultColumnState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("DashboardQueryResultColumn with id {PropertyValue} does not exists");
        
    }
}
