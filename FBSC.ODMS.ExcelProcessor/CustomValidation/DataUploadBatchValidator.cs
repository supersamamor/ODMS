using FBSC.ODMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.ExcelProcessor.Models;
using FBSC.ODMS.ExcelProcessor.Helper;


namespace FBSC.ODMS.ExcelProcessor.CustomValidation
{
    public static class DataUploadBatchValidator
    {
        public static async Task<Dictionary<string, object?>>  ValidatePerRecordAsync(ApplicationContext context, Dictionary<string, object?> rowValue)
        {
			string errorValidation = "";
            var dataSourceId = rowValue[nameof(DataUploadBatchState.DataSourceId)]?.ToString();
			var dataSource = await context.DataSource.Where(l => l.Name == dataSourceId).AsNoTracking().IgnoreQueryFilters().FirstOrDefaultAsync();
			if(dataSource == null) {
				errorValidation += $"Data Source is not valid.; ";
			}
			else
			{
				rowValue[nameof(DataUploadBatchState.DataSourceId)] = dataSource?.Id;
			}
			if (!string.IsNullOrEmpty(dataSourceId))
			{
				var dataSourceIdMaxLength = 450;
				if (dataSourceId.Length > dataSourceIdMaxLength)
				{
					errorValidation += $"Data Source should be less than {dataSourceIdMaxLength} characters.;";
				}
			}
			var fileName = rowValue[nameof(DataUploadBatchState.FileName)]?.ToString();
			if (!string.IsNullOrEmpty(fileName))
			{
				var fileNameMaxLength = 260;
				if (fileName.Length > fileNameMaxLength)
				{
					errorValidation += $"Uploaded File Name should be less than {fileNameMaxLength} characters.;";
				}
			}
			var fileType = rowValue[nameof(DataUploadBatchState.FileType)]?.ToString();
			if (!string.IsNullOrEmpty(fileType))
			{
				var fileTypeMaxLength = 20;
				if (fileType.Length > fileTypeMaxLength)
				{
					errorValidation += $"CSV, XLSX, or JSON should be less than {fileTypeMaxLength} characters.;";
				}
			}
			var uploadedBy = rowValue[nameof(DataUploadBatchState.UploadedBy)]?.ToString();
			if (!string.IsNullOrEmpty(uploadedBy))
			{
				var uploadedByMaxLength = 100;
				if (uploadedBy.Length > uploadedByMaxLength)
				{
					errorValidation += $"Uploaded By should be less than {uploadedByMaxLength} characters.;";
				}
			}
			var stagingTableName = rowValue[nameof(DataUploadBatchState.StagingTableName)]?.ToString();
			if (!string.IsNullOrEmpty(stagingTableName))
			{
				var stagingTableNameMaxLength = 150;
				if (stagingTableName.Length > stagingTableNameMaxLength)
				{
					errorValidation += $"Generated Internal Staging Table Name should be less than {stagingTableNameMaxLength} characters.;";
				}
			}
			
			
			var importStatus = rowValue[nameof(DataUploadBatchState.ImportStatus)]?.ToString();
			if (!string.IsNullOrEmpty(importStatus))
			{
				var importStatusMaxLength = 50;
				if (importStatus.Length > importStatusMaxLength)
				{
					errorValidation += $"Status should be less than {importStatusMaxLength} characters.;";
				}
			}
			
			var errorRemarks = rowValue[nameof(DataUploadBatchState.ErrorRemarks)]?.ToString();
			if (!string.IsNullOrEmpty(errorRemarks))
			{
				var errorRemarksMaxLength = 450;
				if (errorRemarks.Length > errorRemarksMaxLength)
				{
					errorValidation += $"Import Error should be less than {errorRemarksMaxLength} characters.;";
				}
			}
			
			if (!string.IsNullOrEmpty(errorValidation))
			{
				throw new Exception(errorValidation);
			}
            return rowValue;
        }
			
		public static Dictionary<string, HashSet<int>> DuplicateValidation(List<ExcelRecord> records)
		{
			List<string> listOfKeys = [
																																																
			];
			return listOfKeys.Count > 0 ? DictionaryHelper.FindDuplicateRowNumbersPerKey(records, listOfKeys) : [];
		}
    }
}
