using AutoMapper;
using FBSC.ODMS.API.Controllers.v1;
using FBSC.ODMS.Application.Features.ODMS.DataSource.Commands;

namespace FBSC.ODMS.API;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<DataSourceViewModel, AddDataSourceCommand>();
		CreateMap <DataSourceViewModel, EditDataSourceCommand>();		
    }
}
