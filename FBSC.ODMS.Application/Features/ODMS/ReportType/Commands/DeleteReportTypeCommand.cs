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

namespace FBSC.ODMS.Application.Features.ODMS.ReportType.Commands;

public record DeleteReportTypeCommand : BaseCommand, IRequest<Validation<Error, ReportTypeState>>;

public class DeleteReportTypeCommandHandler(ApplicationContext context,
                                   IMapper mapper,
                                   CompositeValidator<DeleteReportTypeCommand> validator) : BaseCommandHandler<ApplicationContext, ReportTypeState, DeleteReportTypeCommand>(context, mapper, validator), IRequestHandler<DeleteReportTypeCommand, Validation<Error, ReportTypeState>>
{ 
    public async Task<Validation<Error, ReportTypeState>> Handle(DeleteReportTypeCommand request, CancellationToken cancellationToken) =>
        await Validators.ValidateTAsync(request, cancellationToken).BindT(
            async request => await Delete(request.Id, cancellationToken));
}


public class DeleteReportTypeCommandValidator : AbstractValidator<DeleteReportTypeCommand>
{
    readonly ApplicationContext _context;

    public DeleteReportTypeCommandValidator(ApplicationContext context)
    {
        _context = context;
        RuleFor(x => x.Id).MustAsync(async (id, cancellation) => await _context.Exists<ReportTypeState>(x => x.Id == id, cancellationToken: cancellation))
                          .WithMessage("ReportType with id {PropertyValue} does not exists");
    }
}
