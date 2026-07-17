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

namespace FBSC.ODMS.Application.Features.ODMS.ReportType.Commands;

public record EditReportTypeCommand : ReportTypeState, IRequest<Validation<Error, ReportTypeState>>;

public class EditReportTypeCommandHandler(ApplicationContext context,
                                 IMapper mapper,
                                 CompositeValidator<EditReportTypeCommand> validator) : BaseCommandHandler<ApplicationContext, ReportTypeState, EditReportTypeCommand>(context, mapper, validator), IRequestHandler<EditReportTypeCommand, Validation<Error, ReportTypeState>>
{ 
    public async Task<Validation<Error, ReportTypeState>> Handle(EditReportTypeCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await EditReportType(request, cancellationToken));


	public async Task<Validation<Error, ReportTypeState>> EditReportType(EditReportTypeCommand request, CancellationToken cancellationToken)
	{
		var entity = await Context.ReportType.Where(l => l.Id == request.Id).SingleAsync(cancellationToken: cancellationToken);
		Mapper.Map(request, entity);
		await UpdateEntitySubCollectionAsync<ReportTypeState, DashboardWidgetState>(request.Id, nameof(DashboardWidgetState.ReportTypeId), nameof(request.DashboardWidgetList), entity, cancellationToken);
		Context.Update(entity);
		_ = await Context.SaveChangesAsync(cancellationToken);
		return Success<Error, ReportTypeState>(entity);
	}
	
}

public class EditReportTypeCommandValidator : AbstractValidator<EditReportTypeCommand>
{
    readonly ApplicationContext _context;

    public EditReportTypeCommandValidator(ApplicationContext context)
    {
        _context = context;
		RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<ReportTypeState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("ReportType with id {PropertyValue} does not exists");
        RuleFor(x => x.Code).MustAsync(async (request, code, cancellation) => await _context.NotExists<ReportTypeState>(x => x.Code == code && x.Id != request.Id, cancellationToken: cancellation)).WithMessage("ReportType with code {PropertyValue} already exists");
	
    }
}
