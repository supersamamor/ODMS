using AutoMapper;
using FBSC.Common.Core.Commands;
using FBSC.Common.Data;
using FBSC.Common.Utility.Validators;
using FBSC.ODMS.Core.Constants;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using FluentValidation;
using LanguageExt;
using LanguageExt.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using static LanguageExt.Prelude;

namespace FBSC.ODMS.Application.Features.ODMS.StatusReport.Commands;

/// <summary>
/// Creates a weekly status report for a project. The reporting week is supplied
/// as an ISO week number (+ year) and resolved to a ReportingWeek row here -
/// created on first use - so the caller never deals with ReportingWeek ids.
/// </summary>
public record AddStatusReportCommand : StatusReportState, IRequest<Validation<Error, StatusReportState>>
{
    public int? SelectedWeekNumber { get; init; }
    public int? SelectedYear { get; init; }
}

public class AddStatusReportCommandHandler(ApplicationContext context,
                                IMapper mapper,
                                CompositeValidator<AddStatusReportCommand> validator) : BaseCommandHandler<ApplicationContext, StatusReportState, AddStatusReportCommand>(context, mapper, validator), IRequestHandler<AddStatusReportCommand, Validation<Error, StatusReportState>>
{
    public async Task<Validation<Error, StatusReportState>> Handle(AddStatusReportCommand request, CancellationToken cancellationToken) =>
        await Validators.ValidateTAsync(request, cancellationToken).BindT(
            async request => await AddStatusReport(request, cancellationToken));

    public async Task<Validation<Error, StatusReportState>> AddStatusReport(AddStatusReportCommand request, CancellationToken cancellationToken)
    {
        var year = request.SelectedYear ?? ISOWeek.GetYear(DateTime.Today);
        var week = request.SelectedWeekNumber!.Value;

        var reportingWeek = await GetOrCreateReportingWeek(year, week, cancellationToken);

        // One report per project per reporting week.
        var duplicateExists = await Context.StatusReport.AsNoTracking()
            .AnyAsync(s => s.ProjectId == request.ProjectId && s.ReportingWeekId == reportingWeek.Id, cancellationToken);
        if (duplicateExists)
        {
            return Fail<Error, StatusReportState>($"A status report for this project already exists for Week {week}, {year}.");
        }

        StatusReportState entity = Mapper.Map<StatusReportState>(request) with
        {
            ReportingWeekId = reportingWeek.Id,
            Status = string.IsNullOrEmpty(request.Status) ? StatusReportStatuses.PendingReview : request.Status,
            SubmissionDate = request.SubmissionDate ?? DateTime.Today,
        };
        AddEntitySubCollection<StatusReportState, StatusReportHealthIndicatorState>(entity, nameof(request.StatusReportHealthIndicatorList));
        AddEntitySubCollection<StatusReportState, StatusReportMilestoneState>(entity, nameof(request.StatusReportMilestoneList));
        AddEntitySubCollection<StatusReportState, StatusReportRiskIssueState>(entity, nameof(request.StatusReportRiskIssueList));
        _ = await Context.AddAsync(entity, cancellationToken);
        _ = await Context.SaveChangesAsync(cancellationToken);
        return Success<Error, StatusReportState>(entity);
    }

    private async Task<ReportingWeekState> GetOrCreateReportingWeek(int year, int week, CancellationToken cancellationToken)
    {
        var existing = await Context.ReportingWeek
            .FirstOrDefaultAsync(w => w.Year == year && w.WeekNumber == week, cancellationToken);
        if (existing != null)
        {
            return existing;
        }
        var start = ISOWeek.ToDateTime(year, week, DayOfWeek.Monday);
        var reportingWeek = new ReportingWeekState
        {
            Year = year,
            WeekNumber = week,
            StartDate = start,
            EndDate = start.AddDays(6),
        };
        _ = await Context.AddAsync(reportingWeek, cancellationToken);
        // Saved together with the report in AddStatusReport's SaveChanges; the
        // unique (WeekNumber, Year) index backstops a concurrent first-create.
        return reportingWeek;
    }
}

public class AddStatusReportCommandValidator : AbstractValidator<AddStatusReportCommand>
{
    readonly ApplicationContext _context;

    public AddStatusReportCommandValidator(ApplicationContext context)
    {
        _context = context;

        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.NotExists<StatusReportState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("StatusReport with id {PropertyValue} already exists");
        RuleFor(x => x.ProjectId).NotEmpty().WithMessage("Project is required.")
                                 .MustAsync(async (projectId, cancellation) => await _context.Exists<ProjectState>(x => x.Id == projectId, cancellationToken: cancellation))
                                 .WithMessage("Project with id {PropertyValue} does not exist.");
        RuleFor(x => x.SelectedWeekNumber).NotNull().WithMessage("Reporting Week is required.")
                                          .InclusiveBetween(1, 53).WithMessage("Reporting Week must be between 1 and 53.");
        RuleFor(x => x.OverallHealth).NotEmpty().WithMessage("This week's health is required.")
                                     .Must(v => HealthStatuses.List.Contains(v)).WithMessage("Overall health must be Red, Amber, or Green.");

        RuleForEach(x => x.StatusReportMilestoneList).ChildRules(m =>
        {
            m.RuleFor(x => x.Name).NotEmpty().WithMessage("Milestone name is required.");
            m.RuleFor(x => x.Status).NotEmpty().WithMessage("Milestone status is required.");
        });
        RuleForEach(x => x.StatusReportRiskIssueList).ChildRules(r =>
        {
            r.RuleFor(x => x.Title).NotEmpty().WithMessage("Risk/Issue title is required.");
            r.RuleFor(x => x.Type).NotEmpty().WithMessage("Risk/Issue type is required.");
            r.RuleFor(x => x.OwnerId).NotEmpty().WithMessage("Risk/Issue owner is required.");
        });
    }
}
