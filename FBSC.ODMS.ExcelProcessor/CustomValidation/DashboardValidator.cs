using FBSC.ODMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.ExcelProcessor.Models;
using FBSC.ODMS.ExcelProcessor.Helper;


namespace FBSC.ODMS.ExcelProcessor.CustomValidation
{
    public static class DashboardValidator
    {
        public static async Task<Dictionary<string, object?>>  ValidatePerRecordAsync(ApplicationContext context, Dictionary<string, object?> rowValue)
        {
			string errorValidation = "";
            var code = rowValue[nameof(DashboardState.Code)]?.ToString();
			if (!string.IsNullOrEmpty(code))
			{
				var codeMaxLength = 50;
				if (code.Length > codeMaxLength)
				{
					errorValidation += $"Code should be less than {codeMaxLength} characters.;";
				}
				if (await context.Dashboard.Where(l => l.Code == code).AsNoTracking().IgnoreQueryFilters().AnyAsync())
				{
					errorValidation += $"Code already exist from the database.";
				}
			}
			var name = rowValue[nameof(DashboardState.Name)]?.ToString();
			if (!string.IsNullOrEmpty(name))
			{
				var nameMaxLength = 150;
				if (name.Length > nameMaxLength)
				{
					errorValidation += $"Name should be less than {nameMaxLength} characters.;";
				}
			}
			var description = rowValue[nameof(DashboardState.Description)]?.ToString();
			if (!string.IsNullOrEmpty(description))
			{
				var descriptionMaxLength = 450;
				if (description.Length > descriptionMaxLength)
				{
					errorValidation += $"Description should be less than {descriptionMaxLength} characters.;";
				}
			}
			var category = rowValue[nameof(DashboardState.Category)]?.ToString();
			if (!string.IsNullOrEmpty(category))
			{
				var categoryMaxLength = 100;
				if (category.Length > categoryMaxLength)
				{
					errorValidation += $"HRIS, Accounting, Sales, Or Custom Grouping should be less than {categoryMaxLength} characters.;";
				}
			}
			var dashboardThemeId = rowValue[nameof(DashboardState.DashboardThemeId)]?.ToString();
			var dashboardTheme = await context.DashboardTheme.Where(l => l.Code == dashboardThemeId).AsNoTracking().IgnoreQueryFilters().FirstOrDefaultAsync();
			if(dashboardTheme == null) {
				errorValidation += $"Dashboard Theme is not valid.; ";
			}
			else
			{
				rowValue[nameof(DashboardState.DashboardThemeId)] = dashboardTheme?.Id;
			}
			if (!string.IsNullOrEmpty(dashboardThemeId))
			{
				var dashboardThemeIdMaxLength = 450;
				if (dashboardThemeId.Length > dashboardThemeIdMaxLength)
				{
					errorValidation += $"Dashboard Theme should be less than {dashboardThemeIdMaxLength} characters.;";
				}
			}
			var ownerUserId = rowValue[nameof(DashboardState.OwnerUserId)]?.ToString();
			if (!string.IsNullOrEmpty(ownerUserId))
			{
				var ownerUserIdMaxLength = 100;
				if (ownerUserId.Length > ownerUserIdMaxLength)
				{
					errorValidation += $"Owner should be less than {ownerUserIdMaxLength} characters.;";
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
				nameof(DashboardState.Code),
																																																
			];
			return listOfKeys.Count > 0 ? DictionaryHelper.FindDuplicateRowNumbersPerKey(records, listOfKeys) : [];
		}
    }
}
