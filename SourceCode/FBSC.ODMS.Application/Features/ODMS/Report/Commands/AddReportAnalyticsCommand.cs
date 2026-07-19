using AutoMapper;
using FBSC.Common.Core.Commands;
using FBSC.Common.Utility.Validators;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using FluentValidation;
using LanguageExt;
using LanguageExt.Common;
using MediatR;
using static LanguageExt.Prelude;

namespace FBSC.ODMS.Application.Features.ODMS.Report.Commands;

public record AddReportAnalyticsCommand : ReportAIIntegrationState, IRequest<Validation<Error, ReportAIIntegrationState>>;

public class AddReportAnalyticsCommandHandler(ApplicationContext context,
                                IMapper mapper,
                                CompositeValidator<AddReportAnalyticsCommand> validator) : BaseCommandHandler<ApplicationContext, ReportState, AddReportAnalyticsCommand>(context, mapper, validator), IRequestHandler<AddReportAnalyticsCommand, Validation<Error, ReportAIIntegrationState>>
{
    public async Task<Validation<Error, ReportAIIntegrationState>> Handle(AddReportAnalyticsCommand request, CancellationToken cancellationToken) =>
		await Validators.ValidateTAsync(request, cancellationToken).BindT(
			async request => await AddReport(request, cancellationToken));
	public async Task<Validation<Error, ReportAIIntegrationState>> AddReport(AddReportAnalyticsCommand request, CancellationToken cancellationToken)
	{
        ReportAIIntegrationState entity = Mapper.Map<ReportAIIntegrationState>(request);			
        _ = await Context.AddAsync(entity, cancellationToken);	
		_ = await Context.SaveChangesAsync(cancellationToken);
		return Success<Error, ReportAIIntegrationState>(entity);
	}	
}

public class AddReportAnalyticsCommandValidator : AbstractValidator<AddReportAnalyticsCommand>
{
    public AddReportAnalyticsCommandValidator()
    {
      
    }
}