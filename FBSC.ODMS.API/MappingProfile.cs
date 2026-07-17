using AutoMapper;
using FBSC.ODMS.API.Controllers.v1;
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


namespace FBSC.ODMS.API;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<DataSourceViewModel, AddDataSourceCommand>();
		CreateMap <DataSourceViewModel, EditDataSourceCommand>();
		CreateMap<DataSourceSchemaCacheViewModel, AddDataSourceSchemaCacheCommand>();
		CreateMap <DataSourceSchemaCacheViewModel, EditDataSourceSchemaCacheCommand>();
		CreateMap<DataUploadBatchViewModel, AddDataUploadBatchCommand>();
		CreateMap <DataUploadBatchViewModel, EditDataUploadBatchCommand>();
		CreateMap<DataUploadColumnViewModel, AddDataUploadColumnCommand>();
		CreateMap <DataUploadColumnViewModel, EditDataUploadColumnCommand>();
		CreateMap<ReportTypeViewModel, AddReportTypeCommand>();
		CreateMap <ReportTypeViewModel, EditReportTypeCommand>();
		CreateMap<DashboardQueryViewModel, AddDashboardQueryCommand>();
		CreateMap <DashboardQueryViewModel, EditDashboardQueryCommand>();
		CreateMap<DashboardQueryParameterViewModel, AddDashboardQueryParameterCommand>();
		CreateMap <DashboardQueryParameterViewModel, EditDashboardQueryParameterCommand>();
		CreateMap<DashboardQueryResultColumnViewModel, AddDashboardQueryResultColumnCommand>();
		CreateMap <DashboardQueryResultColumnViewModel, EditDashboardQueryResultColumnCommand>();
		CreateMap<DashboardQueryResultCacheViewModel, AddDashboardQueryResultCacheCommand>();
		CreateMap <DashboardQueryResultCacheViewModel, EditDashboardQueryResultCacheCommand>();
		CreateMap<DashboardThemeViewModel, AddDashboardThemeCommand>();
		CreateMap <DashboardThemeViewModel, EditDashboardThemeCommand>();
		CreateMap<DashboardViewModel, AddDashboardCommand>();
		CreateMap <DashboardViewModel, EditDashboardCommand>();
		CreateMap<DashboardWidgetViewModel, AddDashboardWidgetCommand>();
		CreateMap <DashboardWidgetViewModel, EditDashboardWidgetCommand>();
		CreateMap<AiSqlPromptTemplateViewModel, AddAiSqlPromptTemplateCommand>();
		CreateMap <AiSqlPromptTemplateViewModel, EditAiSqlPromptTemplateCommand>();
		CreateMap<AiSqlGenerationRequestViewModel, AddAiSqlGenerationRequestCommand>();
		CreateMap <AiSqlGenerationRequestViewModel, EditAiSqlGenerationRequestCommand>();
		CreateMap<DashboardRefreshJobViewModel, AddDashboardRefreshJobCommand>();
		CreateMap <DashboardRefreshJobViewModel, EditDashboardRefreshJobCommand>();
		CreateMap<DashboardAccessViewModel, AddDashboardAccessCommand>();
		CreateMap <DashboardAccessViewModel, EditDashboardAccessCommand>();
		
    }
}
