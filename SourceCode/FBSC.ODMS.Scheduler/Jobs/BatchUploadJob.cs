using FBSC.Common.Services.Shared.Interfaces;
using FBSC.Common.Services.Shared.Models.Mail;
using FBSC.ODMS.Core.Constants;
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
    public class BatchUploadJob(ApplicationContext context, ILogger<BatchUploadJob> logger, IConfiguration configuration, IMailService emailSender, ExcelService excelService, DynamicTableImportService dynamicTableImportService, IdentityContext identityContext) : IJob
    {
        private readonly string? _uploadPath = configuration.GetValue<string>("UsersUpload:SecureUploadFilePath");
        private readonly string? _applicationConnectionString = configuration.GetConnectionString("ApplicationContext");

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
                    exceptionFilePath = await ValidateBatchUpload(item.Module, item.Path, item.CreatedBy!, item.TargetEntityId);
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
		private async Task<string?> ValidateBatchUpload(string module, string path, string processedByUserId, string? targetEntityId)
        {
            string? exceptionFilePath = null;
            switch (module)
            {
                case Core.Constants.UploadModules.DataSourceFileImport:
					var dataSource = await context.DataSource.FirstOrDefaultAsync(d => d.Id == targetEntityId)
						?? throw new Exception($"DataSource with id {targetEntityId} does not exist.");
					var tableName = $"Upload_{dataSource.Id.Replace("-", "")}";
					var importResult = await dynamicTableImportService.ImportAsDynamicTableAsync(path, tableName, _applicationConnectionString!);
					if (!importResult.Success)
					{
						throw new Exception(importResult.ErrorMessage);
					}
					dataSource.SetImportResult(tableName, true, null);
					context.Update(dataSource);
					await context.UpdateBatchRecordAsync(processedByUserId, dataSource);
					break;
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
                case nameof(BusinessUnitState):
                    var businessUnitImportResult = await excelService.ImportAsync<BusinessUnitState>(path);
                    if (businessUnitImportResult.IsSuccess)
                    {
                        await context.AddRangeAsync(businessUnitImportResult.SuccessRecords);
                    }
                    else
                    {
                        exceptionFilePath = ExcelService.UpdateExistingExcelValidationResult<BusinessUnitState>(businessUnitImportResult.FailedRecords, _uploadPath + "\\BatchUploadErrors", path);
                    }
                    break;
                case nameof(ProjectState):
                    var projectImportResult = await excelService.ImportAsync<ProjectState>(path);
                    if (projectImportResult.IsSuccess)
                    {
                        var projects = projectImportResult.SuccessRecords.ToList();
                        // Resolve each referenced BusinessUnit's code once.
                        var buIds = projects.Select(p => p.BusinessUnitId).Distinct().ToList();
                        var buCodes = await context.BusinessUnit.AsNoTracking()
                            .Where(b => buIds.Contains(b.Id))
                            .ToDictionaryAsync(b => b.Id, b => b.Code);

                        // Auto-generate ProjectCode ({BU.Code}{0000001}): ONE block
                        // reservation per BusinessUnit regardless of row count - so a
                        // million-row import makes a handful of sequence calls, not a
                        // million. All inside a transaction so the counters and the
                        // rows commit together (a failure rolls both back - no gaps).
                        await using var projectTx = await context.Database.BeginTransactionAsync();
                        var stampedProjects = new List<ProjectState>(projects.Count);
                        foreach (var group in projects.GroupBy(p => p.BusinessUnitId))
                        {
                            buCodes.TryGetValue(group.Key, out var buCode);
                            var startValue = await SequenceGenerator.ReserveAsync(context, SequenceKeys.ProjectCode(group.Key), group.Count());
                            long offset = 0;
                            foreach (var project in group)
                            {
                                stampedProjects.Add(project with { ProjectCode = CodeFormats.Project(buCode ?? "", startValue + offset) });
                                offset++;
                            }
                        }
                        await context.AddRangeAsync(stampedProjects);
                        await context.SaveChangesAsync();
                        await projectTx.CommitAsync();
                    }
                    else
                    {
                        exceptionFilePath = ExcelService.UpdateExistingExcelValidationResult<ProjectState>(projectImportResult.FailedRecords, _uploadPath + "\\BatchUploadErrors", path);
                    }
                    break;
                case nameof(TeamMembersState):
                    var teamMembersImportResult = await excelService.ImportAsync<TeamMembersState>(path);
                    if (teamMembersImportResult.IsSuccess)
                    {
                        await context.AddRangeAsync(teamMembersImportResult.SuccessRecords);
                    }
                    else
                    {
                        exceptionFilePath = ExcelService.UpdateExistingExcelValidationResult<TeamMembersState>(teamMembersImportResult.FailedRecords, _uploadPath + "\\BatchUploadErrors", path);
                    }
                    break;
                case nameof(EmployeeState):
                    var employeeImportResult = await excelService.ImportAsync<EmployeeState>(path);
                    if (employeeImportResult.IsSuccess)
                    {
                        await context.AddRangeAsync(employeeImportResult.SuccessRecords);
                    }
                    else
                    {
                        exceptionFilePath = ExcelService.UpdateExistingExcelValidationResult<EmployeeState>(employeeImportResult.FailedRecords, _uploadPath + "\\BatchUploadErrors", path);
                    }
                    break;
                case nameof(MilestoneState):
                    var milestoneImportResult = await excelService.ImportAsync<MilestoneState>(path);
                    if (milestoneImportResult.IsSuccess)
                    {
                        await context.AddRangeAsync(milestoneImportResult.SuccessRecords);
                    }
                    else
                    {
                        exceptionFilePath = ExcelService.UpdateExistingExcelValidationResult<MilestoneState>(milestoneImportResult.FailedRecords, _uploadPath + "\\BatchUploadErrors", path);
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
