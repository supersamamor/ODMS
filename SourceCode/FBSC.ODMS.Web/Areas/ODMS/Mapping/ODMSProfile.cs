using AutoMapper;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Web.Areas.ODMS.Models;
using FBSC.ODMS.Application.Features.ODMS.Report.Commands;
using FBSC.ODMS.Application.DTOs;
using FBSC.ODMS.Application.Features.ODMS.Approval.Commands;
using FBSC.ODMS.Application.Features.ODMS.DataSource.Commands;
using FBSC.ODMS.Application.Features.ODMS.DataUpload.Commands;


namespace FBSC.ODMS.Web.Areas.ODMS.Mapping;

public class ODMSProfile : Profile
{
    public ODMSProfile()
    {
		CreateMap<ReportViewModel, AddReportCommand>()
            .ForMember(dest => dest.ReportRoleAssignmentList, opt => opt.MapFrom(src => src.ReportRoleAssignmentList!.Select(x => new ReportRoleAssignmentState { RoleName = x })));
        CreateMap<ReportViewModel, EditReportCommand>()
           .ForMember(dest => dest.ReportRoleAssignmentList, opt => opt.MapFrom(src => src.ReportRoleAssignmentList!.Select(x => new ReportRoleAssignmentState { RoleName = x })));
        CreateMap<ReportState, ReportViewModel>()
            .ForMember(dest => dest.ReportRoleAssignmentList, opt => opt.MapFrom(src => src.ReportRoleAssignmentList!.Select(x => x.RoleName)));
		CreateMap<ReportViewModel, AddReportWithSQLQueryFromAICommand>()
			.ForMember(dest => dest.ReportRoleAssignmentList, opt => opt.MapFrom(src => src.ReportRoleAssignmentList!.Select(x => new ReportRoleAssignmentState { RoleName = x })));
        CreateMap<ReportViewModel, ReportState>();      
        CreateMap<ReportQueryFilterState, ReportQueryFilterViewModel>().ForPath(e => e.ForeignKeyReport, o => o.MapFrom(s => s.Report!.ReportName));
        CreateMap<ReportQueryFilterViewModel, ReportQueryFilterState>();
        CreateMap<ReportResultModel, ReportResultViewModel>().ReverseMap();
        CreateMap<ReportQueryFilterModel, ReportQueryFilterViewModel>().ReverseMap();	
		
        CreateMap<DataSourceViewModel, AddDataSourceCommand>();
		CreateMap<DataSourceViewModel, EditDataSourceCommand>();
		CreateMap<DataSourceState, DataSourceViewModel>().ReverseMap();
		CreateMap<DataUploadViewModel, AddDataUploadCommand>().ForPath(e => e.FilePath, o => o.MapFrom(s => s.GeneratedFilePathPath));
		CreateMap<DataUploadViewModel, EditDataUploadCommand>().ForPath(e => e.FilePath, o => o.MapFrom(s => s.GeneratedFilePathPath));
		CreateMap<DataUploadState, DataUploadViewModel>().ReverseMap();
		
		CreateMap<ApproverAssignmentState, ApproverAssignmentViewModel>().ReverseMap();
		CreateMap<ApproverSetupViewModel, EditApproverSetupCommand>();
		CreateMap<ApproverSetupViewModel, AddApproverSetupCommand>();
		CreateMap<ApproverSetupState, ApproverSetupViewModel>().ReverseMap();
    }
}
