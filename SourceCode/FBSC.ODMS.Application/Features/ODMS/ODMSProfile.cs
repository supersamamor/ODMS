using AutoMapper;
using FBSC.Common.Core.Mapping;
using FBSC.ODMS.Application.Features.ODMS.Approval.Commands;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Application.Features.ODMS.Report.Commands;
using FBSC.ODMS.Application.Features.ODMS.DataSource.Commands;
using FBSC.ODMS.Application.Features.ODMS.DataUpload.Commands;



namespace FBSC.ODMS.Application.Features.ODMS;

public class ODMSProfile : Profile
{
    public ODMSProfile()
    {
		CreateMap<AddReportCommand, ReportState>();
        CreateMap<EditReportCommand, ReportState>().IgnoreBaseEntityProperties();
		CreateMap<AddReportAnalyticsCommand, ReportAIIntegrationState>();
		CreateMap<AddReportWithSQLQueryFromAICommand, ReportState>();
		
        CreateMap<AddDataSourceCommand, DataSourceState>();
		CreateMap<EditDataSourceCommand, DataSourceState>()
			.IgnoreBaseEntityProperties()
			.ForMember(dest => dest.PasswordEncrypted, opt => opt.Ignore());
		CreateMap<AddDataUploadCommand, DataUploadState>();
		CreateMap <EditDataUploadCommand, DataUploadState>().IgnoreBaseEntityProperties();
		
		CreateMap<EditApproverSetupCommand, ApproverSetupState>().IgnoreBaseEntityProperties();
		CreateMap<AddApproverSetupCommand, ApproverSetupState>().IgnoreBaseEntityProperties();
		CreateMap<ApproverAssignmentState, ApproverAssignmentState>().IgnoreBaseEntityProperties();
    }
}
