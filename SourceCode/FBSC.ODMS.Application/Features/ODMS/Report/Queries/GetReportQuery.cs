using FBSC.Common.Core.Queries;
using FBSC.Common.Utility.Models;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using MediatR;
using FBSC.Common.Utility.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FBSC.ODMS.Application.Features.ODMS.Report.Queries;

public record GetReportQuery : BaseQuery, IRequest<PagedListResponse<ReportState>>;

public class GetReportQueryHandler(ApplicationContext context) : BaseQueryHandler<ApplicationContext, ReportState, GetReportQuery>(context), IRequestHandler<GetReportQuery, PagedListResponse<ReportState>>
{
}
