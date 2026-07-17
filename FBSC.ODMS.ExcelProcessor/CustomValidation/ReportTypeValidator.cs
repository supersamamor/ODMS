using FBSC.ODMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.ExcelProcessor.Models;
using FBSC.ODMS.ExcelProcessor.Helper;


namespace FBSC.ODMS.ExcelProcessor.CustomValidation
{
    public static class ReportTypeValidator
    {
        public static async Task<Dictionary<string, object?>>  ValidatePerRecordAsync(ApplicationContext context, Dictionary<string, object?> rowValue)
        {
			string errorValidation = "";
            var code = rowValue[nameof(ReportTypeState.Code)]?.ToString();
			if (!string.IsNullOrEmpty(code))
			{
				var codeMaxLength = 30;
				if (code.Length > codeMaxLength)
				{
					errorValidation += $"Code should be less than {codeMaxLength} characters.;";
				}
				if (await context.ReportType.Where(l => l.Code == code).AsNoTracking().IgnoreQueryFilters().AnyAsync())
				{
					errorValidation += $"Code already exist from the database.";
				}
			}
			var name = rowValue[nameof(ReportTypeState.Name)]?.ToString();
			if (!string.IsNullOrEmpty(name))
			{
				var nameMaxLength = 100;
				if (name.Length > nameMaxLength)
				{
					errorValidation += $"Name should be less than {nameMaxLength} characters.;";
				}
			}
			var chartRenderer = rowValue[nameof(ReportTypeState.ChartRenderer)]?.ToString();
			if (!string.IsNullOrEmpty(chartRenderer))
			{
				var chartRendererMaxLength = 50;
				if (chartRenderer.Length > chartRendererMaxLength)
				{
					errorValidation += $"Underlying Chart.js Type Or Widget Renderer Key should be less than {chartRendererMaxLength} characters.;";
				}
			}
			
			
			
			
			
			var iconClass = rowValue[nameof(ReportTypeState.IconClass)]?.ToString();
			if (!string.IsNullOrEmpty(iconClass))
			{
				var iconClassMaxLength = 100;
				if (iconClass.Length > iconClassMaxLength)
				{
					errorValidation += $"Icon Class For Picker UI should be less than {iconClassMaxLength} characters.;";
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
				nameof(ReportTypeState.Code),
																																												
			];
			return listOfKeys.Count > 0 ? DictionaryHelper.FindDuplicateRowNumbersPerKey(records, listOfKeys) : [];
		}
    }
}
