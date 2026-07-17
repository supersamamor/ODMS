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

namespace FBSC.ODMS.Application.Features.ODMS.Report.Commands;

public record AddReportWithSQLQueryFromAICommand : ReportState, IRequest<Validation<Error, ReportState>>
{   
    public string SQLQuery { get; set; } = "";
}

public class AddReportWithSQLQueryFromAICommandHandler(ApplicationContext context,
                                IMapper mapper,
                                CompositeValidator<AddReportWithSQLQueryFromAICommand> validator) : BaseCommandHandler<ApplicationContext, ReportState, AddReportWithSQLQueryFromAICommand>(context, mapper, validator), IRequestHandler<AddReportWithSQLQueryFromAICommand, Validation<Error, ReportState>>
{
    public async Task<Validation<Error, ReportState>> Handle(AddReportWithSQLQueryFromAICommand request, CancellationToken cancellationToken) =>
        await Validators.ValidateTAsync(request, cancellationToken).BindT(
            async request => await AddReport(request, cancellationToken));


    public async Task<Validation<Error, ReportState>> AddReport(AddReportWithSQLQueryFromAICommand request, CancellationToken cancellationToken)
    {
        while (await Context.Exists<ReportState>(x => x.ReportName == request.ReportName, cancellationToken: cancellationToken))
        {
            request.IncrementReportName();
        }
        ReportState entity = Mapper.Map<ReportState>(request);
        entity.SetQueryType(request.SQLQuery);
        UpdateReportQueryFilterList(entity);
        UpdateReportRoleAssignmentList(entity);
        _ = await Context.AddAsync(entity, cancellationToken);
        await AddReportAIIntegration(entity.Id, entity.ReportDescription, request.SQLQuery, cancellationToken);
        _ = await Context.SaveChangesAsync(cancellationToken);
        return Success<Error, ReportState>(entity);
    }

    private void UpdateReportQueryFilterList(ReportState entity)
    {
        if (entity.ReportQueryFilterList?.Count > 0)
        {
            foreach (var reportQueryFilter in entity.ReportQueryFilterList!)
            {
                Context.Entry(reportQueryFilter).State = EntityState.Added;
            }
        }
    }
    private void UpdateReportRoleAssignmentList(ReportState entity)
    {
        if (entity.ReportQueryFilterList?.Count > 0)
        {
            foreach (var reportRoleAssignment in entity.ReportRoleAssignmentList!)
            {
                Context.Entry(reportRoleAssignment).State = EntityState.Added;
            }
        }
    }
    private async Task AddReportAIIntegration(string reportId, string? input, string output, CancellationToken cancellationToken)
    {
        ReportAIIntegrationState entity = new()
        {
            ReportId = reportId,
            Input = input ?? "",
            Output = output,
        };
        _ = await Context.AddAsync(entity, cancellationToken);
    }
}

public class AddReportWithSQLQueryFromAICommandValidator : AbstractValidator<AddReportWithSQLQueryFromAICommand>
{
    readonly ApplicationContext _context;

    public AddReportWithSQLQueryFromAICommandValidator(ApplicationContext context)
    {
        _context = context;
        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.NotExists<ReportState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("Report with id {PropertyValue} already exists");       
					  
		RuleFor(x => x.QueryString)
            .Custom((queryString, context) =>
            {
                var result = Helpers.SQLValidatorHelper.Validate(queryString);
                if (!result.IsValid)
                {
                    foreach (var error in result.Errors)
                    {
                        context.AddFailure(error);
                    }
                }
            });
    }
}
