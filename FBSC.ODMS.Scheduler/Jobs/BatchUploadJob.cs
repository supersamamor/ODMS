using FBSC.Common.Services.Shared.Interfaces;
using FBSC.Common.Services.Shared.Models.Mail;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Data;
using FBSC.ODMS.ExcelProcessor.Services;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;
using FBSC.ODMS.ExcelProcessor.CustomTemplate;
using FBSC.ODMS.ExcelProcessor.CustomProcessor;

namespace FBSC.ODMS.Scheduler.Jobs
{
    [DisallowConcurrentExecution]
    public class BatchUploadJob(ApplicationContext context, ILogger<BatchUploadJob> logger, IConfiguration configuration, IMailService emailSender, ExcelService excelService, IdentityContext identityContext) : IJob
    {
        private readonly string? _uploadPath = configuration.GetValue<string>("UsersUpload:SecureUploadFilePath");

        public async Task Execute(IJobExecutionContext context)
        {
            await ProcessBatchUploadAsync();
        }
        private async Task ProcessBatchUploadAsync()
        {
            var uploadProcessorList = await context.UploadProcessor.Where(l => l.Status == Core.Constants.FileUploadStatus.Pending).IgnoreQueryFilters().AsNoTracking()
                .OrderBy(l => l.CreatedDate).ToListAsync();
            var exceptionFilePath = "";
            foreach (var item in uploadProcessorList)
            {
                try
                {
                    //Tag Start Date/Time
                    item.SetStart();
                    context.Update(item);
                    await context.UpdateBatchRecordAsync(item.CreatedBy, item);
                    //Start Processing
                    exceptionFilePath = await ValidateBatchUpload(item.Module, item.Path, item.CreatedBy!);
                    if (string.IsNullOrEmpty(exceptionFilePath))
                    {
                        item.SetDone();
                    }
                    else
                    {
                        item.SetFailed(exceptionFilePath, "Error from the file.");
                    }
                    context.Update(item);
                    await context.UpdateBatchRecordAsync(item.CreatedBy, item);
                }
                catch (Exception ex)
                {
					context.DetachAllTrackedEntities();
                    logger.LogError(ex, @"ProcessBatchUploadAsync Error Message : {Message} / StackTrace : {StackTrace}", ex.Message, ex.StackTrace);
                    item.SetFailed("", ex.Message);
                    context.Update(item);
                    await context.UpdateBatchRecordAsync(item.CreatedBy, item);
                }
                try
                {
                    if (!string.IsNullOrEmpty(exceptionFilePath))
                    {
                        var email = (await identityContext.Users.Where(l => l.Id == item.CreatedBy).AsNoTracking().FirstOrDefaultAsync())!.Email!;
                        await SendValidatedBatchUploadFile(email, item.Module, exceptionFilePath);
                    }
                }
                catch (Exception ex)
                {               
                    logger.LogError(ex, @"ProcessBatchUploadAsync Error Message : {Message} / StackTrace : {StackTrace}", ex.Message, ex.StackTrace);
                }              
            }
        }
		private async Task<string?> ValidateBatchUpload(string module, string path, string processedByUserId)
        {
            string? exceptionFilePath = null;   
            switch (module)
            {
                case nameof(DataSourceState):
					var dataSourceImportResult = await excelService.ImportAsync<DataSourceState>(path);
					if (dataSourceImportResult.IsSuccess)
					{
						await context.AddRangeAsync(dataSourceImportResult.SuccessRecords);
					}
					else
					{
						exceptionFilePath = ExcelService.UpdateExistingExcelValidationResult<DataSourceState>(dataSourceImportResult.FailedRecords, _uploadPath + "\\BatchUploadErrors", path);
					}
					break;
				case nameof(DataSourceSchemaCacheState):
					var dataSourceSchemaCacheImportResult = await excelService.ImportAsync<DataSourceSchemaCacheState>(path);
					if (dataSourceSchemaCacheImportResult.IsSuccess)
					{
						await context.AddRangeAsync(dataSourceSchemaCacheImportResult.SuccessRecords);
					}
					else
					{
						exceptionFilePath = ExcelService.UpdateExistingExcelValidationResult<DataSourceSchemaCacheState>(dataSourceSchemaCacheImportResult.FailedRecords, _uploadPath + "\\BatchUploadErrors", path);
					}
					break;
				case nameof(DataUploadBatchState):
					var dataUploadBatchImportResult = await excelService.ImportAsync<DataUploadBatchState>(path);
					if (dataUploadBatchImportResult.IsSuccess)
					{
						await context.AddRangeAsync(dataUploadBatchImportResult.SuccessRecords);
					}
					else
					{
						exceptionFilePath = ExcelService.UpdateExistingExcelValidationResult<DataUploadBatchState>(dataUploadBatchImportResult.FailedRecords, _uploadPath + "\\BatchUploadErrors", path);
					}
					break;
				case nameof(DataUploadColumnState):
					var dataUploadColumnImportResult = await excelService.ImportAsync<DataUploadColumnState>(path);
					if (dataUploadColumnImportResult.IsSuccess)
					{
						await context.AddRangeAsync(dataUploadColumnImportResult.SuccessRecords);
					}
					else
					{
						exceptionFilePath = ExcelService.UpdateExistingExcelValidationResult<DataUploadColumnState>(dataUploadColumnImportResult.FailedRecords, _uploadPath + "\\BatchUploadErrors", path);
					}
					break;
				case nameof(ReportTypeState):
					var reportTypeImportResult = await excelService.ImportAsync<ReportTypeState>(path);
					if (reportTypeImportResult.IsSuccess)
					{
						await context.AddRangeAsync(reportTypeImportResult.SuccessRecords);
					}
					else
					{
						exceptionFilePath = ExcelService.UpdateExistingExcelValidationResult<ReportTypeState>(reportTypeImportResult.FailedRecords, _uploadPath + "\\BatchUploadErrors", path);
					}
					break;
				case nameof(DashboardQueryState):
					var dashboardQueryImportResult = await excelService.ImportAsync<DashboardQueryState>(path);
					if (dashboardQueryImportResult.IsSuccess)
					{
						await context.AddRangeAsync(dashboardQueryImportResult.SuccessRecords);
					}
					else
					{
						exceptionFilePath = ExcelService.UpdateExistingExcelValidationResult<DashboardQueryState>(dashboardQueryImportResult.FailedRecords, _uploadPath + "\\BatchUploadErrors", path);
					}
					break;
				case nameof(DashboardQueryParameterState):
					var dashboardQueryParameterImportResult = await excelService.ImportAsync<DashboardQueryParameterState>(path);
					if (dashboardQueryParameterImportResult.IsSuccess)
					{
						await context.AddRangeAsync(dashboardQueryParameterImportResult.SuccessRecords);
					}
					else
					{
						exceptionFilePath = ExcelService.UpdateExistingExcelValidationResult<DashboardQueryParameterState>(dashboardQueryParameterImportResult.FailedRecords, _uploadPath + "\\BatchUploadErrors", path);
					}
					break;
				case nameof(DashboardQueryResultColumnState):
					var dashboardQueryResultColumnImportResult = await excelService.ImportAsync<DashboardQueryResultColumnState>(path);
					if (dashboardQueryResultColumnImportResult.IsSuccess)
					{
						await context.AddRangeAsync(dashboardQueryResultColumnImportResult.SuccessRecords);
					}
					else
					{
						exceptionFilePath = ExcelService.UpdateExistingExcelValidationResult<DashboardQueryResultColumnState>(dashboardQueryResultColumnImportResult.FailedRecords, _uploadPath + "\\BatchUploadErrors", path);
					}
					break;
				case nameof(DashboardQueryResultCacheState):
					var dashboardQueryResultCacheImportResult = await excelService.ImportAsync<DashboardQueryResultCacheState>(path);
					if (dashboardQueryResultCacheImportResult.IsSuccess)
					{
						await context.AddRangeAsync(dashboardQueryResultCacheImportResult.SuccessRecords);
					}
					else
					{
						exceptionFilePath = ExcelService.UpdateExistingExcelValidationResult<DashboardQueryResultCacheState>(dashboardQueryResultCacheImportResult.FailedRecords, _uploadPath + "\\BatchUploadErrors", path);
					}
					break;
				case nameof(DashboardThemeState):
					var dashboardThemeImportResult = await excelService.ImportAsync<DashboardThemeState>(path);
					if (dashboardThemeImportResult.IsSuccess)
					{
						await context.AddRangeAsync(dashboardThemeImportResult.SuccessRecords);
					}
					else
					{
						exceptionFilePath = ExcelService.UpdateExistingExcelValidationResult<DashboardThemeState>(dashboardThemeImportResult.FailedRecords, _uploadPath + "\\BatchUploadErrors", path);
					}
					break;
				case nameof(DashboardState):
					var dashboardImportResult = await excelService.ImportAsync<DashboardState>(path);
					if (dashboardImportResult.IsSuccess)
					{
						await context.AddRangeAsync(dashboardImportResult.SuccessRecords);
					}
					else
					{
						exceptionFilePath = ExcelService.UpdateExistingExcelValidationResult<DashboardState>(dashboardImportResult.FailedRecords, _uploadPath + "\\BatchUploadErrors", path);
					}
					break;
				case nameof(DashboardWidgetState):
					var dashboardWidgetImportResult = await excelService.ImportAsync<DashboardWidgetState>(path);
					if (dashboardWidgetImportResult.IsSuccess)
					{
						await context.AddRangeAsync(dashboardWidgetImportResult.SuccessRecords);
					}
					else
					{
						exceptionFilePath = ExcelService.UpdateExistingExcelValidationResult<DashboardWidgetState>(dashboardWidgetImportResult.FailedRecords, _uploadPath + "\\BatchUploadErrors", path);
					}
					break;
				case nameof(AiSqlPromptTemplateState):
					var aiSqlPromptTemplateImportResult = await excelService.ImportAsync<AiSqlPromptTemplateState>(path);
					if (aiSqlPromptTemplateImportResult.IsSuccess)
					{
						await context.AddRangeAsync(aiSqlPromptTemplateImportResult.SuccessRecords);
					}
					else
					{
						exceptionFilePath = ExcelService.UpdateExistingExcelValidationResult<AiSqlPromptTemplateState>(aiSqlPromptTemplateImportResult.FailedRecords, _uploadPath + "\\BatchUploadErrors", path);
					}
					break;
				case nameof(AiSqlGenerationRequestState):
					var aiSqlGenerationRequestImportResult = await excelService.ImportAsync<AiSqlGenerationRequestState>(path);
					if (aiSqlGenerationRequestImportResult.IsSuccess)
					{
						await context.AddRangeAsync(aiSqlGenerationRequestImportResult.SuccessRecords);
					}
					else
					{
						exceptionFilePath = ExcelService.UpdateExistingExcelValidationResult<AiSqlGenerationRequestState>(aiSqlGenerationRequestImportResult.FailedRecords, _uploadPath + "\\BatchUploadErrors", path);
					}
					break;
				case nameof(DashboardRefreshJobState):
					var dashboardRefreshJobImportResult = await excelService.ImportAsync<DashboardRefreshJobState>(path);
					if (dashboardRefreshJobImportResult.IsSuccess)
					{
						await context.AddRangeAsync(dashboardRefreshJobImportResult.SuccessRecords);
					}
					else
					{
						exceptionFilePath = ExcelService.UpdateExistingExcelValidationResult<DashboardRefreshJobState>(dashboardRefreshJobImportResult.FailedRecords, _uploadPath + "\\BatchUploadErrors", path);
					}
					break;
				case nameof(DashboardAccessState):
					var dashboardAccessImportResult = await excelService.ImportAsync<DashboardAccessState>(path);
					if (dashboardAccessImportResult.IsSuccess)
					{
						await context.AddRangeAsync(dashboardAccessImportResult.SuccessRecords);
					}
					else
					{
						exceptionFilePath = ExcelService.UpdateExistingExcelValidationResult<DashboardAccessState>(dashboardAccessImportResult.FailedRecords, _uploadPath + "\\BatchUploadErrors", path);
					}
					break;
				
                case nameof(SampleTableState): //Sample Only For Custom Processing
					var unitImportResult = await excelService.ImportAsync<SampleTableState>(path);
					if (unitImportResult.IsSuccess)
					{
						await SampleTableCustomProcessor.Process(context, unitImportResult.SuccessRecords, processedByUserId);
					}
					else
					{
						exceptionFilePath = ExcelService.UpdateExistingExcelValidationResult<SampleTableState>(unitImportResult.FailedRecords, _uploadPath + "\\BatchUploadErrors", path);
					}
					break;
                default: break;
            }           
            return exceptionFilePath;
        }
        
        public async Task SendValidatedBatchUploadFile(string email, string module, string exceptionFilePath)
        {
            string wordToRemove = "State";
            if (module.EndsWith(wordToRemove))
            {
                module = module[..^wordToRemove.Length];
            }
            string subject = $"Batch Upload - " + module;
            string message = GenerateEmailBody();
            var emailRequest = new MailRequest()
            {
                Subject = subject,
                Body = message,
                Attachments = [ exceptionFilePath ],              
                To = email
            };           
            await emailSender.SendAsync(emailRequest);
        }
        private static string GenerateEmailBody()
        {
            string str = "<span style='font-size:10pt; font-family:Arial;'> ";
            str += "Your uploaded file has been failed on processing. Please see attached file for the validation remarks.";
            str += "<br />";           
            str += "</span> ";
            return str;
        }
    }
}
