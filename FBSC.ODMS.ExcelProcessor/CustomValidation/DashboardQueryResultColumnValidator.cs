using FBSC.ODMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.ExcelProcessor.Models;
using FBSC.ODMS.ExcelProcessor.Helper;


namespace FBSC.ODMS.ExcelProcessor.CustomValidation
{
    public static class DashboardQueryResultColumnValidator
    {
        public static async Task<Dictionary<string, object?>>  ValidatePerRecordAsync(ApplicationContext context, Dictionary<string, object?> rowValue)
        {
			string errorValidation = "";
            var dashboardQueryId = rowValue[nameof(DashboardQueryResultColumnState.DashboardQueryId)]?.ToString();
			var dashboardQuery = await context.DashboardQuery.Where(l => l.QueryHash == dashboardQueryId).AsNoTracking().IgnoreQueryFilters().FirstOrDefaultAsync();
			if(dashboardQuery == null) {
				errorValidation += $"Dashboard Query is not valid.; ";
			}
			else
			{
				rowValue[nameof(DashboardQueryResultColumnState.DashboardQueryId)] = dashboardQuery?.Id;
			}
			if (!string.IsNullOrEmpty(dashboardQueryId))
			{
				var dashboardQueryIdMaxLength = 450;
				if (dashboardQueryId.Length > dashboardQueryIdMaxLength)
				{
					errorValidation += $"Dashboard Query should be less than {dashboardQueryIdMaxLength} characters.;";
				}
			}
			var columnName = rowValue[nameof(DashboardQueryResultColumnState.ColumnName)]?.ToString();
			if (!string.IsNullOrEmpty(columnName))
			{
				var columnNameMaxLength = 150;
				if (columnName.Length > columnNameMaxLength)
				{
					errorValidation += $"Column Name As Returned By The Query should be less than {columnNameMaxLength} characters.;";
				}
			}
			
			var sqlDataType = rowValue[nameof(DashboardQueryResultColumnState.SqlDataType)]?.ToString();
			if (!string.IsNullOrEmpty(sqlDataType))
			{
				var sqlDataTypeMaxLength = 50;
				if (sqlDataType.Length > sqlDataTypeMaxLength)
				{
					errorValidation += $"Native Sql Data Type Returned should be less than {sqlDataTypeMaxLength} characters.;";
				}
			}
			var inferredRole = rowValue[nameof(DashboardQueryResultColumnState.InferredRole)]?.ToString();
			if (!string.IsNullOrEmpty(inferredRole))
			{
				var inferredRoleMaxLength = 50;
				if (inferredRole.Length > inferredRoleMaxLength)
				{
					errorValidation += $"Dimension, Measure, DateAxis, Label, Or Identifier should be less than {inferredRoleMaxLength} characters.;";
				}
			}
			
			var defaultAggregation = rowValue[nameof(DashboardQueryResultColumnState.DefaultAggregation)]?.ToString();
			if (!string.IsNullOrEmpty(defaultAggregation))
			{
				var defaultAggregationMaxLength = 20;
				if (defaultAggregation.Length > defaultAggregationMaxLength)
				{
					errorValidation += $"SUM, COUNT, AVG, MIN, Or MAX should be less than {defaultAggregationMaxLength} characters.;";
				}
			}
			var formatString = rowValue[nameof(DashboardQueryResultColumnState.FormatString)]?.ToString();
			if (!string.IsNullOrEmpty(formatString))
			{
				var formatStringMaxLength = 50;
				if (formatString.Length > formatStringMaxLength)
				{
					errorValidation += $"Display Format, E.g. Currency Or Percent Mask should be less than {formatStringMaxLength} characters.;";
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
