using FBSC.Common.Core.Queries;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using LanguageExt;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FBSC.ODMS.Application.Features.ODMS.DataUpload.Queries;

public record GetDataUploadByIdQuery(string Id) : BaseQueryById(Id), IRequest<Option<DataUploadState>>;

public class GetDataUploadByIdQueryHandler(ApplicationContext context) : BaseQueryByIdHandler<ApplicationContext, DataUploadState, GetDataUploadByIdQuery>(context), IRequestHandler<GetDataUploadByIdQuery, Option<DataUploadState>>
{
		
}
