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

public record AddReportTypeCommand : ReportTypeState, IRequest<Validation<Error, ReportTypeState>>;

public class AddReportTypeCommandHandler(ApplicationContext context,
                                IMapper mapper,
                                CompositeValidator<AddReportTypeCommand> validator,IdentityContext identityContext) : BaseCommandHandler<ApplicationContext, ReportTypeState, AddReportTypeCommand>(context, mapper, validator), IRequestHandler<AddReportTypeCommand, Validation<Error, ReportTypeState>>
{
    public async Task<Validation<Error, ReportTypeState>> Handle(AddReportTypeCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await AddReportType(request, cancellationToken));


	public async Task<Validation<Error, ReportTypeState>> AddReportType(AddReportTypeCommand request, CancellationToken cancellationToken)
	{
		ReportTypeState entity = Mapper.Map<ReportTypeState>(request);
		AddEntitySubCollection<ReportTypeState, DashboardWidgetState>(entity, nameof(request.DashboardWidgetList));
		_ = await Context.AddAsync(entity, cancellationToken);
		await Helpers.ApprovalHelper.AddApprovers(Context, identityContext, ApprovalModule.ReportType, entity.Id, cancellationToken);
		_ = await Context.SaveChangesAsync(cancellationToken);
		return Success<Error, ReportTypeState>(entity);
	}
	
}

public class AddReportTypeCommandValidator : AbstractValidator<AddReportTypeCommand>
{
    readonly ApplicationContext _context;

    public AddReportTypeCommandValidator(ApplicationContext context)
    {
        _context = context;

        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.NotExists<ReportTypeState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("ReportType with id {PropertyValue} already exists");
        RuleFor(x => x.Code).MustAsync(async (code, cancellation) => await _context.NotExists<ReportTypeState>(x => x.Code == code, cancellationToken: cancellation)).WithMessage("ReportType with code {PropertyValue} already exists");
	
    }
}
