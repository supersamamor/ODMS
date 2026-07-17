using FBSC.ODMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.ExcelProcessor.Models;
using FBSC.ODMS.ExcelProcessor.Helper;


namespace FBSC.ODMS.ExcelProcessor.CustomValidation
{
    public static class DashboardRefreshJobValidator
    {
        public static async Task<Dictionary<string, object?>>  ValidatePerRecordAsync(ApplicationContext context, Dictionary<string, object?> rowValue)
        {
			string errorValidation = "";
            var dashboardQueryId = rowValue[nameof(DashboardRefreshJobState.DashboardQueryId)]?.ToString();
			var dashboardQuery = await context.DashboardQuery.Where(l => l.QueryHash == dashboardQueryId).AsNoTracking().IgnoreQueryFilters().FirstOrDefaultAsync();
			if(dashboardQuery == null) {
				errorValidation += $"Dashboard Query is not valid.; ";
			}
			else
			{
				rowValue[nameof(DashboardRefreshJobState.DashboardQueryId)] = dashboardQuery?.Id;
			}
			if (!string.IsNullOrEmpty(dashboardQueryId))
			{
				var dashboardQueryIdMaxLength = 450;
				if (dashboardQueryId.Length > dashboardQueryIdMaxLength)
				{
					errorValidation += $"Dashboard Query should be less than {dashboardQueryIdMaxLength} characters.;";
				}
			}
			var triggerType = rowValue[nameof(DashboardRefreshJobState.TriggerType)]?.ToString();
			if (!string.IsNullOrEmpty(triggerType))
			{
				var triggerTypeMaxLength = 20;
				if (triggerType.Length > triggerTypeMaxLength)
				{
					errorValidation += $"Manual Or Schedule should be less than {triggerTypeMaxLength} characters.;";
				}
			}
			var status = rowValue[nameof(DashboardRefreshJobState.Status)]?.ToString();
			if (!string.IsNullOrEmpty(status))
			{
				var statusMaxLength = 50;
				if (status.Length > statusMaxLength)
				{
					errorValidation += $"Status should be less than {statusMaxLength} characters.;";
				}
			}
			
			
			
			
			
			
			
			var errorRemarks = rowValue[nameof(DashboardRefreshJobState.ErrorRemarks)]?.ToString();
			if (!string.IsNullOrEmpty(errorRemarks))
			{
				var errorRemarksMaxLength = 450;
				if (errorRemarks.Length > errorRemarksMaxLength)
				{
					errorValidation += $"Error Remarks should be less than {errorRemarksMaxLength} characters.;";
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
