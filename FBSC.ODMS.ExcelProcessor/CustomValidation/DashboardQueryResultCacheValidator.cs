using FBSC.ODMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.ExcelProcessor.Models;
using FBSC.ODMS.ExcelProcessor.Helper;


namespace FBSC.ODMS.ExcelProcessor.CustomValidation
{
    public static class DashboardQueryResultCacheValidator
    {
        public static async Task<Dictionary<string, object?>>  ValidatePerRecordAsync(ApplicationContext context, Dictionary<string, object?> rowValue)
        {
			string errorValidation = "";
            var dashboardQueryId = rowValue[nameof(DashboardQueryResultCacheState.DashboardQueryId)]?.ToString();
			var dashboardQuery = await context.DashboardQuery.Where(l => l.QueryHash == dashboardQueryId).AsNoTracking().IgnoreQueryFilters().FirstOrDefaultAsync();
			if(dashboardQuery == null) {
				errorValidation += $"Dashboard Query is not valid.; ";
			}
			else
			{
				rowValue[nameof(DashboardQueryResultCacheState.DashboardQueryId)] = dashboardQuery?.Id;
			}
			if (!string.IsNullOrEmpty(dashboardQueryId))
			{
				var dashboardQueryIdMaxLength = 450;
				if (dashboardQueryId.Length > dashboardQueryIdMaxLength)
				{
					errorValidation += $"Dashboard Query should be less than {dashboardQueryIdMaxLength} characters.;";
				}
			}
			var parameterSetHash = rowValue[nameof(DashboardQueryResultCacheState.ParameterSetHash)]?.ToString();
			if (!string.IsNullOrEmpty(parameterSetHash))
			{
				var parameterSetHashMaxLength = 64;
				if (parameterSetHash.Length > parameterSetHashMaxLength)
				{
					errorValidation += $"Hash Of The Resolved Parameter Values For This Cache Entry should be less than {parameterSetHashMaxLength} characters.;";
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
