using AutoMapper;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Web.Areas.ODMS.Models;
using FBSC.ODMS.Application.Features.ODMS.Report.Commands;
using FBSC.ODMS.Application.DTOs;
using FBSC.ODMS.Application.Features.ODMS.Approval.Commands;
using FBSC.ODMS.Application.Features.ODMS.DataSource.Commands;
using FBSC.ODMS.Application.Features.ODMS.DataSourceSchemaCache.Commands;
using FBSC.ODMS.Application.Features.ODMS.DataUploadBatch.Commands;
using FBSC.ODMS.Application.Features.ODMS.DataUploadColumn.Commands;
using FBSC.ODMS.Application.Features.ODMS.ReportType.Commands;
using FBSC.ODMS.Application.Features.ODMS.DashboardQuery.Commands;
using FBSC.ODMS.Application.Features.ODMS.DashboardQueryParameter.Commands;
using FBSC.ODMS.Application.Features.ODMS.DashboardQueryResultColumn.Commands;
using FBSC.ODMS.Application.Features.ODMS.DashboardQueryResultCache.Commands;
using FBSC.ODMS.Application.Features.ODMS.DashboardTheme.Commands;
using FBSC.ODMS.Application.Features.ODMS.Dashboard.Commands;
using FBSC.ODMS.Application.Features.ODMS.DashboardWidget.Commands;
using FBSC.ODMS.Application.Features.ODMS.AiSqlPromptTemplate.Commands;
using FBSC.ODMS.Application.Features.ODMS.AiSqlGenerationRequest.Commands;
using FBSC.ODMS.Application.Features.ODMS.DashboardRefreshJob.Commands;
using FBSC.ODMS.Application.Features.ODMS.DashboardAccess.Commands;


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
		CreateMap<DataSourceSchemaCacheViewModel, AddDataSourceSchemaCacheCommand>();
		CreateMap<DataSourceSchemaCacheViewModel, EditDataSourceSchemaCacheCommand>();
		CreateMap<DataSourceSchemaCacheState, DataSourceSchemaCacheViewModel>().ForPath(e => e.ReferenceFieldDataSourceId, o => o.MapFrom(s => s.DataSource!.Name));
		CreateMap<DataSourceSchemaCacheViewModel, DataSourceSchemaCacheState>();
		CreateMap<DataUploadBatchViewModel, AddDataUploadBatchCommand>();
		CreateMap<DataUploadBatchViewModel, EditDataUploadBatchCommand>();
		CreateMap<DataUploadBatchState, DataUploadBatchViewModel>().ForPath(e => e.ReferenceFieldDataSourceId, o => o.MapFrom(s => s.DataSource!.Name));
		CreateMap<DataUploadBatchViewModel, DataUploadBatchState>();
		CreateMap<DataUploadColumnViewModel, AddDataUploadColumnCommand>();
		CreateMap<DataUploadColumnViewModel, EditDataUploadColumnCommand>();
		CreateMap<DataUploadColumnState, DataUploadColumnViewModel>().ForPath(e => e.ReferenceFieldDataUploadBatchId, o => o.MapFrom(s => s.DataUploadBatch!.Id));
		CreateMap<DataUploadColumnViewModel, DataUploadColumnState>();
		CreateMap<ReportTypeViewModel, AddReportTypeCommand>();
		CreateMap<ReportTypeViewModel, EditReportTypeCommand>();
		CreateMap<ReportTypeState, ReportTypeViewModel>().ReverseMap();
		CreateMap<DashboardQueryViewModel, AddDashboardQueryCommand>();
		CreateMap<DashboardQueryViewModel, EditDashboardQueryCommand>();
		CreateMap<DashboardQueryState, DashboardQueryViewModel>().ForPath(e => e.ReferenceFieldDataSourceId, o => o.MapFrom(s => s.DataSource!.Name));
		CreateMap<DashboardQueryViewModel, DashboardQueryState>();
		CreateMap<DashboardQueryParameterViewModel, AddDashboardQueryParameterCommand>();
		CreateMap<DashboardQueryParameterViewModel, EditDashboardQueryParameterCommand>();
		CreateMap<DashboardQueryParameterState, DashboardQueryParameterViewModel>().ForPath(e => e.ReferenceFieldDashboardQueryId, o => o.MapFrom(s => s.DashboardQuery!.QueryHash));
		CreateMap<DashboardQueryParameterViewModel, DashboardQueryParameterState>();
		CreateMap<DashboardQueryResultColumnViewModel, AddDashboardQueryResultColumnCommand>();
		CreateMap<DashboardQueryResultColumnViewModel, EditDashboardQueryResultColumnCommand>();
		CreateMap<DashboardQueryResultColumnState, DashboardQueryResultColumnViewModel>().ForPath(e => e.ReferenceFieldDashboardQueryId, o => o.MapFrom(s => s.DashboardQuery!.QueryHash));
		CreateMap<DashboardQueryResultColumnViewModel, DashboardQueryResultColumnState>();
		CreateMap<DashboardQueryResultCacheViewModel, AddDashboardQueryResultCacheCommand>();
		CreateMap<DashboardQueryResultCacheViewModel, EditDashboardQueryResultCacheCommand>();
		CreateMap<DashboardQueryResultCacheState, DashboardQueryResultCacheViewModel>().ForPath(e => e.ReferenceFieldDashboardQueryId, o => o.MapFrom(s => s.DashboardQuery!.QueryHash));
		CreateMap<DashboardQueryResultCacheViewModel, DashboardQueryResultCacheState>();
		CreateMap<DashboardThemeViewModel, AddDashboardThemeCommand>();
		CreateMap<DashboardThemeViewModel, EditDashboardThemeCommand>();
		CreateMap<DashboardThemeState, DashboardThemeViewModel>().ReverseMap();
		CreateMap<DashboardViewModel, AddDashboardCommand>();
		CreateMap<DashboardViewModel, EditDashboardCommand>();
		CreateMap<DashboardState, DashboardViewModel>().ForPath(e => e.ReferenceFieldDashboardThemeId, o => o.MapFrom(s => s.DashboardTheme!.Code));
		CreateMap<DashboardViewModel, DashboardState>();
		CreateMap<DashboardWidgetViewModel, AddDashboardWidgetCommand>();
		CreateMap<DashboardWidgetViewModel, EditDashboardWidgetCommand>();
		CreateMap<DashboardWidgetState, DashboardWidgetViewModel>().ForPath(e => e.ReferenceFieldDrillDownDashboardId, o => o.MapFrom(s => s.Dashboard!.Code)).ForPath(e => e.ReferenceFieldReportTypeId, o => o.MapFrom(s => s.ReportType!.Code)).ForPath(e => e.ReferenceFieldDashboardId, o => o.MapFrom(s => s.Dashboard!.Code)).ForPath(e => e.ReferenceFieldDashboardQueryId, o => o.MapFrom(s => s.DashboardQuery!.QueryHash));
		CreateMap<DashboardWidgetViewModel, DashboardWidgetState>();
		CreateMap<AiSqlPromptTemplateViewModel, AddAiSqlPromptTemplateCommand>();
		CreateMap<AiSqlPromptTemplateViewModel, EditAiSqlPromptTemplateCommand>();
		CreateMap<AiSqlPromptTemplateState, AiSqlPromptTemplateViewModel>().ReverseMap();
		CreateMap<AiSqlGenerationRequestViewModel, AddAiSqlGenerationRequestCommand>();
		CreateMap<AiSqlGenerationRequestViewModel, EditAiSqlGenerationRequestCommand>();
		CreateMap<AiSqlGenerationRequestState, AiSqlGenerationRequestViewModel>().ForPath(e => e.ReferenceFieldDashboardQueryId, o => o.MapFrom(s => s.DashboardQuery!.QueryHash)).ForPath(e => e.ReferenceFieldDataSourceId, o => o.MapFrom(s => s.DataSource!.Name));
		CreateMap<AiSqlGenerationRequestViewModel, AiSqlGenerationRequestState>();
		CreateMap<DashboardRefreshJobViewModel, AddDashboardRefreshJobCommand>();
		CreateMap<DashboardRefreshJobViewModel, EditDashboardRefreshJobCommand>();
		CreateMap<DashboardRefreshJobState, DashboardRefreshJobViewModel>().ForPath(e => e.ReferenceFieldDashboardQueryId, o => o.MapFrom(s => s.DashboardQuery!.QueryHash));
		CreateMap<DashboardRefreshJobViewModel, DashboardRefreshJobState>();
		CreateMap<DashboardAccessViewModel, AddDashboardAccessCommand>();
		CreateMap<DashboardAccessViewModel, EditDashboardAccessCommand>();
		CreateMap<DashboardAccessState, DashboardAccessViewModel>().ForPath(e => e.ReferenceFieldDashboardId, o => o.MapFrom(s => s.Dashboard!.Code));
		CreateMap<DashboardAccessViewModel, DashboardAccessState>();
		
		CreateMap<ApproverAssignmentState, ApproverAssignmentViewModel>().ReverseMap();
		CreateMap<ApproverSetupViewModel, EditApproverSetupCommand>();
		CreateMap<ApproverSetupViewModel, AddApproverSetupCommand>();
		CreateMap<ApproverSetupState, ApproverSetupViewModel>().ReverseMap();
    }
}
