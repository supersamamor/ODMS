using FBSC.ODMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.ExcelProcessor.Models;
using FBSC.ODMS.ExcelProcessor.Helper;


namespace FBSC.ODMS.ExcelProcessor.CustomValidation
{
    public static class DashboardAccessValidator
    {
        public static async Task<Dictionary<string, object?>>  ValidatePerRecordAsync(ApplicationContext context, Dictionary<string, object?> rowValue)
        {
			string errorValidation = "";
            var dashboardId = rowValue[nameof(DashboardAccessState.DashboardId)]?.ToString();
			var dashboard = await context.Dashboard.Where(l => l.Code == dashboardId).AsNoTracking().IgnoreQueryFilters().FirstOrDefaultAsync();
			if(dashboard == null) {
				errorValidation += $"Dashboard is not valid.; ";
			}
			else
			{
				rowValue[nameof(DashboardAccessState.DashboardId)] = dashboard?.Id;
			}
			if (!string.IsNullOrEmpty(dashboardId))
			{
				var dashboardIdMaxLength = 450;
				if (dashboardId.Length > dashboardIdMaxLength)
				{
					errorValidation += $"Dashboard should be less than {dashboardIdMaxLength} characters.;";
				}
			}
			var granteeType = rowValue[nameof(DashboardAccessState.GranteeType)]?.ToString();
			if (!string.IsNullOrEmpty(granteeType))
			{
				var granteeTypeMaxLength = 20;
				if (granteeType.Length > granteeTypeMaxLength)
				{
					errorValidation += $"User Or Role should be less than {granteeTypeMaxLength} characters.;";
				}
			}
			var granteeId = rowValue[nameof(DashboardAccessState.GranteeId)]?.ToString();
			if (!string.IsNullOrEmpty(granteeId))
			{
				var granteeIdMaxLength = 100;
				if (granteeId.Length > granteeIdMaxLength)
				{
					errorValidation += $"User Or Role Identifier should be less than {granteeIdMaxLength} characters.;";
				}
			}
			var accessLevel = rowValue[nameof(DashboardAccessState.AccessLevel)]?.ToString();
			if (!string.IsNullOrEmpty(accessLevel))
			{
				var accessLevelMaxLength = 20;
				if (accessLevel.Length > accessLevelMaxLength)
				{
					errorValidation += $"View, Edit, Or Owner should be less than {accessLevelMaxLength} characters.;";
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
