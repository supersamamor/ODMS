using FBSC.ODMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.ExcelProcessor.Models;
using FBSC.ODMS.ExcelProcessor.Helper;


namespace FBSC.ODMS.ExcelProcessor.CustomValidation
{
    public static class DataUploadValidator
    {
        public static async Task<Dictionary<string, object?>>  ValidatePerRecordAsync(ApplicationContext context, Dictionary<string, object?> rowValue)
        {
			string errorValidation = "";
            var description = rowValue[nameof(DataUploadState.Description)]?.ToString();
			if (!string.IsNullOrEmpty(description))
			{
				var descriptionMaxLength = 450;
				if (description.Length > descriptionMaxLength)
				{
					errorValidation += $"Description should be less than {descriptionMaxLength} characters.;";
				}
				if (await context.DataUpload.Where(l => l.Description == description).AsNoTracking().IgnoreQueryFilters().AnyAsync())
				{
					errorValidation += $"Description already exist from the database.";
				}
			}
			
			var fileType = rowValue[nameof(DataUploadState.FileType)]?.ToString();
			if (!string.IsNullOrEmpty(fileType))
			{
				var fileTypeMaxLength = 20;
				if (fileType.Length > fileTypeMaxLength)
				{
					errorValidation += $"CSV, XLSX, or JSON should be less than {fileTypeMaxLength} characters.;";
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
				nameof(DataUploadState.Description),
																
			];
			return listOfKeys.Count > 0 ? DictionaryHelper.FindDuplicateRowNumbersPerKey(records, listOfKeys) : [];
		}
    }
}
