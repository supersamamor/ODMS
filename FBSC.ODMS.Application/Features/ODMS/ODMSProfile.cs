using AutoMapper;
using FBSC.Common.Core.Mapping;
using FBSC.ODMS.Application.Features.ODMS.Approval.Commands;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Application.Features.ODMS.Report.Commands;
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
		CreateMap <EditDataSourceCommand, DataSourceState>().IgnoreBaseEntityProperties();
		CreateMap<AddDataSourceSchemaCacheCommand, DataSourceSchemaCacheState>();
		CreateMap <EditDataSourceSchemaCacheCommand, DataSourceSchemaCacheState>().IgnoreBaseEntityProperties();
		CreateMap<AddDataUploadBatchCommand, DataUploadBatchState>();
		CreateMap <EditDataUploadBatchCommand, DataUploadBatchState>().IgnoreBaseEntityProperties();
		CreateMap<AddDataUploadColumnCommand, DataUploadColumnState>();
		CreateMap <EditDataUploadColumnCommand, DataUploadColumnState>().IgnoreBaseEntityProperties();
		CreateMap<AddReportTypeCommand, ReportTypeState>();
		CreateMap <EditReportTypeCommand, ReportTypeState>().IgnoreBaseEntityProperties();
		CreateMap<AddDashboardQueryCommand, DashboardQueryState>();
		CreateMap <EditDashboardQueryCommand, DashboardQueryState>().IgnoreBaseEntityProperties();
		CreateMap<AddDashboardQueryParameterCommand, DashboardQueryParameterState>();
		CreateMap <EditDashboardQueryParameterCommand, DashboardQueryParameterState>().IgnoreBaseEntityProperties();
		CreateMap<AddDashboardQueryResultColumnCommand, DashboardQueryResultColumnState>();
		CreateMap <EditDashboardQueryResultColumnCommand, DashboardQueryResultColumnState>().IgnoreBaseEntityProperties();
		CreateMap<AddDashboardQueryResultCacheCommand, DashboardQueryResultCacheState>();
		CreateMap <EditDashboardQueryResultCacheCommand, DashboardQueryResultCacheState>().IgnoreBaseEntityProperties();
		CreateMap<AddDashboardThemeCommand, DashboardThemeState>();
		CreateMap <EditDashboardThemeCommand, DashboardThemeState>().IgnoreBaseEntityProperties();
		CreateMap<AddDashboardCommand, DashboardState>();
		CreateMap <EditDashboardCommand, DashboardState>().IgnoreBaseEntityProperties();
		CreateMap<AddDashboardWidgetCommand, DashboardWidgetState>();
		CreateMap <EditDashboardWidgetCommand, DashboardWidgetState>().IgnoreBaseEntityProperties();
		CreateMap<AddAiSqlPromptTemplateCommand, AiSqlPromptTemplateState>();
		CreateMap <EditAiSqlPromptTemplateCommand, AiSqlPromptTemplateState>().IgnoreBaseEntityProperties();
		CreateMap<AddAiSqlGenerationRequestCommand, AiSqlGenerationRequestState>();
		CreateMap <EditAiSqlGenerationRequestCommand, AiSqlGenerationRequestState>().IgnoreBaseEntityProperties();
		CreateMap<AddDashboardRefreshJobCommand, DashboardRefreshJobState>();
		CreateMap <EditDashboardRefreshJobCommand, DashboardRefreshJobState>().IgnoreBaseEntityProperties();
		CreateMap<AddDashboardAccessCommand, DashboardAccessState>();
		CreateMap <EditDashboardAccessCommand, DashboardAccessState>().IgnoreBaseEntityProperties();
		
		CreateMap<EditApproverSetupCommand, ApproverSetupState>().IgnoreBaseEntityProperties();
		CreateMap<AddApproverSetupCommand, ApproverSetupState>().IgnoreBaseEntityProperties();
		CreateMap<ApproverAssignmentState, ApproverAssignmentState>().IgnoreBaseEntityProperties();
    }
}
