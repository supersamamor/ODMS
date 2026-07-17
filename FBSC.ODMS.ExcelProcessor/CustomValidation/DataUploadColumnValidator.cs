using FBSC.ODMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.ExcelProcessor.Models;
using FBSC.ODMS.ExcelProcessor.Helper;


namespace FBSC.ODMS.ExcelProcessor.CustomValidation
{
    public static class DataUploadColumnValidator
    {
        public static async Task<Dictionary<string, object?>>  ValidatePerRecordAsync(ApplicationContext context, Dictionary<string, object?> rowValue)
        {
			string errorValidation = "";
            var dataUploadBatchId = rowValue[nameof(DataUploadColumnState.DataUploadBatchId)]?.ToString();
			var dataUploadBatch = await context.DataUploadBatch.Where(l => l.Id == dataUploadBatchId).AsNoTracking().IgnoreQueryFilters().FirstOrDefaultAsync();
			if(dataUploadBatch == null) {
				errorValidation += $"Data Upload Batch is not valid.; ";
			}
			else
			{
				rowValue[nameof(DataUploadColumnState.DataUploadBatchId)] = dataUploadBatch?.Id;
			}
			if (!string.IsNullOrEmpty(dataUploadBatchId))
			{
				var dataUploadBatchIdMaxLength = 450;
				if (dataUploadBatchId.Length > dataUploadBatchIdMaxLength)
				{
					errorValidation += $"Data Upload Batch should be less than {dataUploadBatchIdMaxLength} characters.;";
				}
			}
			var columnName = rowValue[nameof(DataUploadColumnState.ColumnName)]?.ToString();
			if (!string.IsNullOrEmpty(columnName))
			{
				var columnNameMaxLength = 150;
				if (columnName.Length > columnNameMaxLength)
				{
					errorValidation += $"Detected Column Name should be less than {columnNameMaxLength} characters.;";
				}
			}
			var detectedDataType = rowValue[nameof(DataUploadColumnState.DetectedDataType)]?.ToString();
			if (!string.IsNullOrEmpty(detectedDataType))
			{
				var detectedDataTypeMaxLength = 50;
				if (detectedDataType.Length > detectedDataTypeMaxLength)
				{
					errorValidation += $"Auto-Detected Data Type should be less than {detectedDataTypeMaxLength} characters.;";
				}
			}
			var mappedSqlDataType = rowValue[nameof(DataUploadColumnState.MappedSqlDataType)]?.ToString();
			if (!string.IsNullOrEmpty(mappedSqlDataType))
			{
				var mappedSqlDataTypeMaxLength = 50;
				if (mappedSqlDataType.Length > mappedSqlDataTypeMaxLength)
				{
					errorValidation += $"Sql Data Type Used In Staging Table should be less than {mappedSqlDataTypeMaxLength} characters.;";
				}
			}
			
			var sampleValue = rowValue[nameof(DataUploadColumnState.SampleValue)]?.ToString();
			if (!string.IsNullOrEmpty(sampleValue))
			{
				var sampleValueMaxLength = 450;
				if (sampleValue.Length > sampleValueMaxLength)
				{
					errorValidation += $"First Non-Blank Sample Value should be less than {sampleValueMaxLength} characters.;";
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
