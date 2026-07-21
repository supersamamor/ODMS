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
using static LanguageExt.Prelude;

namespace FBSC.ODMS.Application.Features.ODMS.StatusReport.Commands;

/// <summary>
/// Reviewer decision on a submitted status report: Approve or Request Changes.
/// Stamps the outcome status plus reviewer id/date/comments.
/// </summary>
public record ReviewStatusReportCommand(string Id, string Status, string? ReviewComments, string? ReviewedById) : IRequest<Validation<Error, StatusReportState>>;

public class ReviewStatusReportCommandHandler(ApplicationContext context,
                                CompositeValidator<ReviewStatusReportCommand> validator) : IRequestHandler<ReviewStatusReportCommand, Validation<Error, StatusReportState>>
{
    public async Task<Validation<Error, StatusReportState>> Handle(ReviewStatusReportCommand request, CancellationToken cancellationToken) =>
        await validator.ValidateTAsync(request, cancellationToken).BindT(
            async req => await Review(req, cancellationToken));

    private async Task<Validation<Error, StatusReportState>> Review(ReviewStatusReportCommand request, CancellationToken cancellationToken)
    {
        var entity = await context.StatusReport.Where(l => l.Id == request.Id).SingleAsync(cancellationToken);
        var updated = entity with
        {
            Status = request.Status,
            ReviewComments = request.ReviewComments,
            ReviewedById = request.ReviewedById,
            ReviewedDate = DateTime.Now,
        };
        context.Entry(entity).CurrentValues.SetValues(updated);
        _ = await context.SaveChangesAsync(cancellationToken);
        return Success<Error, StatusReportState>(entity);
    }
}

public class ReviewStatusReportCommandValidator : AbstractValidator<ReviewStatusReportCommand>
{
    readonly ApplicationContext _context;

    public ReviewStatusReportCommandValidator(ApplicationContext context)
    {
        _context = context;

        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<StatusReportState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("StatusReport with id {PropertyValue} does not exist");
        RuleFor(x => x.Status).Must(s => s == StatusReportStatuses.Approved || s == StatusReportStatuses.ChangesRequested)
                              .WithMessage("Review outcome must be Approved or Changes Requested.");
    }
}
