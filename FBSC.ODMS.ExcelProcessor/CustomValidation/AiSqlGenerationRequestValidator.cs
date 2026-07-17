using FBSC.ODMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.ExcelProcessor.Models;
using FBSC.ODMS.ExcelProcessor.Helper;


namespace FBSC.ODMS.ExcelProcessor.CustomValidation
{
    public static class AiSqlGenerationRequestValidator
    {
        public static async Task<Dictionary<string, object?>>  ValidatePerRecordAsync(ApplicationContext context, Dictionary<string, object?> rowValue)
        {
			string errorValidation = "";
            var dataSourceId = rowValue[nameof(AiSqlGenerationRequestState.DataSourceId)]?.ToString();
			var dataSource = await context.DataSource.Where(l => l.Name == dataSourceId).AsNoTracking().IgnoreQueryFilters().FirstOrDefaultAsync();
			if(dataSource == null) {
				errorValidation += $"Data Source is not valid.; ";
			}
			else
			{
				rowValue[nameof(AiSqlGenerationRequestState.DataSourceId)] = dataSource?.Id;
			}
			if (!string.IsNullOrEmpty(dataSourceId))
			{
				var dataSourceIdMaxLength = 450;
				if (dataSourceId.Length > dataSourceIdMaxLength)
				{
					errorValidation += $"Data Source should be less than {dataSourceIdMaxLength} characters.;";
				}
			}
			
			
			var dashboardQueryId = rowValue[nameof(AiSqlGenerationRequestState.DashboardQueryId)]?.ToString();
			var dashboardQuery = await context.DashboardQuery.Where(l => l.QueryHash == dashboardQueryId).AsNoTracking().IgnoreQueryFilters().FirstOrDefaultAsync();
			if(dashboardQuery == null) {
				errorValidation += $"Dashboard Query Created On Acceptance is not valid.; ";
			}
			else
			{
				rowValue[nameof(AiSqlGenerationRequestState.DashboardQueryId)] = dashboardQuery?.Id;
			}
			if (!string.IsNullOrEmpty(dashboardQueryId))
			{
				var dashboardQueryIdMaxLength = 450;
				if (dashboardQueryId.Length > dashboardQueryIdMaxLength)
				{
					errorValidation += $"Dashboard Query Created On Acceptance should be less than {dashboardQueryIdMaxLength} characters.;";
				}
			}
			
			var validationStatus = rowValue[nameof(AiSqlGenerationRequestState.ValidationStatus)]?.ToString();
			if (!string.IsNullOrEmpty(validationStatus))
			{
				var validationStatusMaxLength = 50;
				if (validationStatus.Length > validationStatusMaxLength)
				{
					errorValidation += $"Status should be less than {validationStatusMaxLength} characters.;";
				}
			}
			var errorRemarks = rowValue[nameof(AiSqlGenerationRequestState.ErrorRemarks)]?.ToString();
			if (!string.IsNullOrEmpty(errorRemarks))
			{
				var errorRemarksMaxLength = 450;
				if (errorRemarks.Length > errorRemarksMaxLength)
				{
					errorValidation += $"Generation Or Validation Error should be less than {errorRemarksMaxLength} characters.;";
				}
			}
			var requestedBy = rowValue[nameof(AiSqlGenerationRequestState.RequestedBy)]?.ToString();
			if (!string.IsNullOrEmpty(requestedBy))
			{
				var requestedByMaxLength = 100;
				if (requestedBy.Length > requestedByMaxLength)
				{
					errorValidation += $"Requested By should be less than {requestedByMaxLength} characters.;";
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
