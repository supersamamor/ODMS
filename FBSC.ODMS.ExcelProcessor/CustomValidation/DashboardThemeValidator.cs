using FBSC.ODMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.ExcelProcessor.Models;
using FBSC.ODMS.ExcelProcessor.Helper;


namespace FBSC.ODMS.ExcelProcessor.CustomValidation
{
    public static class DashboardThemeValidator
    {
        public static async Task<Dictionary<string, object?>>  ValidatePerRecordAsync(ApplicationContext context, Dictionary<string, object?> rowValue)
        {
			string errorValidation = "";
            var code = rowValue[nameof(DashboardThemeState.Code)]?.ToString();
			if (!string.IsNullOrEmpty(code))
			{
				var codeMaxLength = 30;
				if (code.Length > codeMaxLength)
				{
					errorValidation += $"Code should be less than {codeMaxLength} characters.;";
				}
				if (await context.DashboardTheme.Where(l => l.Code == code).AsNoTracking().IgnoreQueryFilters().AnyAsync())
				{
					errorValidation += $"Code already exist from the database.";
				}
			}
			var name = rowValue[nameof(DashboardThemeState.Name)]?.ToString();
			if (!string.IsNullOrEmpty(name))
			{
				var nameMaxLength = 100;
				if (name.Length > nameMaxLength)
				{
					errorValidation += $"Name should be less than {nameMaxLength} characters.;";
				}
			}
			
			var primaryColorHex = rowValue[nameof(DashboardThemeState.PrimaryColorHex)]?.ToString();
			if (!string.IsNullOrEmpty(primaryColorHex))
			{
				var primaryColorHexMaxLength = 10;
				if (primaryColorHex.Length > primaryColorHexMaxLength)
				{
					errorValidation += $"Primary Seed Color should be less than {primaryColorHexMaxLength} characters.;";
				}
			}
			
			var generationAlgorithm = rowValue[nameof(DashboardThemeState.GenerationAlgorithm)]?.ToString();
			if (!string.IsNullOrEmpty(generationAlgorithm))
			{
				var generationAlgorithmMaxLength = 50;
				if (generationAlgorithm.Length > generationAlgorithmMaxLength)
				{
					errorValidation += $"Sequential, Categorical, Or Diverging should be less than {generationAlgorithmMaxLength} characters.;";
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
				nameof(DashboardThemeState.Code),
																																
			];
			return listOfKeys.Count > 0 ? DictionaryHelper.FindDuplicateRowNumbersPerKey(records, listOfKeys) : [];
		}
    }
}
