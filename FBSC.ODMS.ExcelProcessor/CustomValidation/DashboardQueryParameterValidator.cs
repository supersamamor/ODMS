using FBSC.ODMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.ExcelProcessor.Models;
using FBSC.ODMS.ExcelProcessor.Helper;


namespace FBSC.ODMS.ExcelProcessor.CustomValidation
{
    public static class DashboardQueryParameterValidator
    {
        public static async Task<Dictionary<string, object?>>  ValidatePerRecordAsync(ApplicationContext context, Dictionary<string, object?> rowValue)
        {
			string errorValidation = "";
            var dashboardQueryId = rowValue[nameof(DashboardQueryParameterState.DashboardQueryId)]?.ToString();
			var dashboardQuery = await context.DashboardQuery.Where(l => l.QueryHash == dashboardQueryId).AsNoTracking().IgnoreQueryFilters().FirstOrDefaultAsync();
			if(dashboardQuery == null) {
				errorValidation += $"Dashboard Query is not valid.; ";
			}
			else
			{
				rowValue[nameof(DashboardQueryParameterState.DashboardQueryId)] = dashboardQuery?.Id;
			}
			if (!string.IsNullOrEmpty(dashboardQueryId))
			{
				var dashboardQueryIdMaxLength = 450;
				if (dashboardQueryId.Length > dashboardQueryIdMaxLength)
				{
					errorValidation += $"Dashboard Query should be less than {dashboardQueryIdMaxLength} characters.;";
				}
			}
			var parameterName = rowValue[nameof(DashboardQueryParameterState.ParameterName)]?.ToString();
			if (!string.IsNullOrEmpty(parameterName))
			{
				var parameterNameMaxLength = 100;
				if (parameterName.Length > parameterNameMaxLength)
				{
					errorValidation += $"Sql Parameter Placeholder, E.g. @DateFrom should be less than {parameterNameMaxLength} characters.;";
				}
			}
			var dataType = rowValue[nameof(DashboardQueryParameterState.DataType)]?.ToString();
			if (!string.IsNullOrEmpty(dataType))
			{
				var dataTypeMaxLength = 50;
				if (dataType.Length > dataTypeMaxLength)
				{
					errorValidation += $"Parameter Data Type should be less than {dataTypeMaxLength} characters.;";
				}
			}
			var controlType = rowValue[nameof(DashboardQueryParameterState.ControlType)]?.ToString();
			if (!string.IsNullOrEmpty(controlType))
			{
				var controlTypeMaxLength = 50;
				if (controlType.Length > controlTypeMaxLength)
				{
					errorValidation += $"Dropdown, DateRange, Text, Or MultiSelect should be less than {controlTypeMaxLength} characters.;";
				}
			}
			var defaultValue = rowValue[nameof(DashboardQueryParameterState.DefaultValue)]?.ToString();
			if (!string.IsNullOrEmpty(defaultValue))
			{
				var defaultValueMaxLength = 450;
				if (defaultValue.Length > defaultValueMaxLength)
				{
					errorValidation += $"Default Value should be less than {defaultValueMaxLength} characters.;";
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
