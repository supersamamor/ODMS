using FBSC.Common.Core.Queries;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FBSC.ODMS.Application.Features.ODMS.DashboardTheme.Queries;

public record GetDashboardThemeByIdQuery(string Id) : BaseQueryById(Id), IRequest<Option<DashboardThemeState>>;

public class GetDashboardThemeByIdQueryHandler(ApplicationContext context) : BaseQueryByIdHandler<ApplicationContext, DashboardThemeState, GetDashboardThemeByIdQuery>(context), IRequestHandler<GetDashboardThemeByIdQuery, Option<DashboardThemeState>>
{
		
}
