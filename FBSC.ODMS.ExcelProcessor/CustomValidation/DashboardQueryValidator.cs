using FBSC.ODMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.ExcelProcessor.Models;
using FBSC.ODMS.ExcelProcessor.Helper;


namespace FBSC.ODMS.ExcelProcessor.CustomValidation
{
    public static class DashboardQueryValidator
    {
        public static async Task<Dictionary<string, object?>>  ValidatePerRecordAsync(ApplicationContext context, Dictionary<string, object?> rowValue)
        {
			string errorValidation = "";
            var dataSourceId = rowValue[nameof(DashboardQueryState.DataSourceId)]?.ToString();
			var dataSource = await context.DataSource.Where(l => l.Name == dataSourceId).AsNoTracking().IgnoreQueryFilters().FirstOrDefaultAsync();
			if(dataSource == null) {
				errorValidation += $"Data Source is not valid.; ";
			}
			else
			{
				rowValue[nameof(DashboardQueryState.DataSourceId)] = dataSource?.Id;
			}
			if (!string.IsNullOrEmpty(dataSourceId))
			{
				var dataSourceIdMaxLength = 450;
				if (dataSourceId.Length > dataSourceIdMaxLength)
				{
					errorValidation += $"Data Source should be less than {dataSourceIdMaxLength} characters.;";
				}
			}
			var name = rowValue[nameof(DashboardQueryState.Name)]?.ToString();
			if (!string.IsNullOrEmpty(name))
			{
				var nameMaxLength = 150;
				if (name.Length > nameMaxLength)
				{
					errorValidation += $"Name should be less than {nameMaxLength} characters.;";
				}
				if (await context.DashboardQuery.Where(l => l.Name == name).AsNoTracking().IgnoreQueryFilters().AnyAsync())
				{
					errorValidation += $"Name already exist from the database.";
				}
			}
			var description = rowValue[nameof(DashboardQueryState.Description)]?.ToString();
			if (!string.IsNullOrEmpty(description))
			{
				var descriptionMaxLength = 450;
				if (description.Length > descriptionMaxLength)
				{
					errorValidation += $"Description should be less than {descriptionMaxLength} characters.;";
				}
			}
			
			var queryHash = rowValue[nameof(DashboardQueryState.QueryHash)]?.ToString();
			if (!string.IsNullOrEmpty(queryHash))
			{
				var queryHashMaxLength = 64;
				if (queryHash.Length > queryHashMaxLength)
				{
					errorValidation += $"Hash Of Normalized Sql Text, Used For Cache Keying And Dedupe should be less than {queryHashMaxLength} characters.;";
				}
				if (await context.DashboardQuery.Where(l => l.QueryHash == queryHash).AsNoTracking().IgnoreQueryFilters().AnyAsync())
				{
					errorValidation += $"Hash Of Normalized Sql Text, Used For Cache Keying And Dedupe already exist from the database.";
				}
			}
			
			
			var validationStatus = rowValue[nameof(DashboardQueryState.ValidationStatus)]?.ToString();
			if (!string.IsNullOrEmpty(validationStatus))
			{
				var validationStatusMaxLength = 50;
				if (validationStatus.Length > validationStatusMaxLength)
				{
					errorValidation += $"Valid, Invalid, Or NotYetValidated should be less than {validationStatusMaxLength} characters.;";
				}
			}
			
			
			
			var lastExecutionErrorRemarks = rowValue[nameof(DashboardQueryState.LastExecutionErrorRemarks)]?.ToString();
			if (!string.IsNullOrEmpty(lastExecutionErrorRemarks))
			{
				var lastExecutionErrorRemarksMaxLength = 450;
				if (lastExecutionErrorRemarks.Length > lastExecutionErrorRemarksMaxLength)
				{
					errorValidation += $"Last Execution Error should be less than {lastExecutionErrorRemarksMaxLength} characters.;";
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
								nameof(DashboardQueryState.Name),
												nameof(DashboardQueryState.QueryHash),
																																								
			];
			return listOfKeys.Count > 0 ? DictionaryHelper.FindDuplicateRowNumbersPerKey(records, listOfKeys) : [];
		}
    }
}
