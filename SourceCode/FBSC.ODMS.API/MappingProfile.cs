using AutoMapper;
using FBSC.ODMS.API.Controllers.v1;
using FBSC.ODMS.Application.Features.ODMS.DataSource.Commands;
using FBSC.ODMS.Application.Features.ODMS.DataUpload.Commands;


namespace FBSC.ODMS.API;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<DataSourceViewModel, AddDataSourceCommand>();
		CreateMap <DataSourceViewModel, EditDataSourceCommand>();
		CreateMap<DataUploadViewModel, AddDataUploadCommand>();
		CreateMap <DataUploadViewModel, EditDataUploadCommand>();
		
    }
}
