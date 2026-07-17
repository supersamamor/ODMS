using FBSC.ODMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.ExcelProcessor.Models;
using FBSC.ODMS.ExcelProcessor.Helper;


namespace FBSC.ODMS.ExcelProcessor.CustomValidation
{
    public static class DataSourceSchemaCacheValidator
    {
        public static async Task<Dictionary<string, object?>>  ValidatePerRecordAsync(ApplicationContext context, Dictionary<string, object?> rowValue)
        {
			string errorValidation = "";
            var dataSourceId = rowValue[nameof(DataSourceSchemaCacheState.DataSourceId)]?.ToString();
			var dataSource = await context.DataSource.Where(l => l.Name == dataSourceId).AsNoTracking().IgnoreQueryFilters().FirstOrDefaultAsync();
			if(dataSource == null) {
				errorValidation += $"Data Source is not valid.; ";
			}
			else
			{
				rowValue[nameof(DataSourceSchemaCacheState.DataSourceId)] = dataSource?.Id;
			}
			if (!string.IsNullOrEmpty(dataSourceId))
			{
				var dataSourceIdMaxLength = 450;
				if (dataSourceId.Length > dataSourceIdMaxLength)
				{
					errorValidation += $"Data Source should be less than {dataSourceIdMaxLength} characters.;";
				}
			}
			var schemaName = rowValue[nameof(DataSourceSchemaCacheState.SchemaName)]?.ToString();
			if (!string.IsNullOrEmpty(schemaName))
			{
				var schemaNameMaxLength = 100;
				if (schemaName.Length > schemaNameMaxLength)
				{
					errorValidation += $"Sql Schema Name should be less than {schemaNameMaxLength} characters.;";
				}
			}
			var tableName = rowValue[nameof(DataSourceSchemaCacheState.TableName)]?.ToString();
			if (!string.IsNullOrEmpty(tableName))
			{
				var tableNameMaxLength = 150;
				if (tableName.Length > tableNameMaxLength)
				{
					errorValidation += $"Sql Table Name should be less than {tableNameMaxLength} characters.;";
				}
			}
			var columnName = rowValue[nameof(DataSourceSchemaCacheState.ColumnName)]?.ToString();
			if (!string.IsNullOrEmpty(columnName))
			{
				var columnNameMaxLength = 150;
				if (columnName.Length > columnNameMaxLength)
				{
					errorValidation += $"Sql Column Name should be less than {columnNameMaxLength} characters.;";
				}
			}
			var sqlDataType = rowValue[nameof(DataSourceSchemaCacheState.SqlDataType)]?.ToString();
			if (!string.IsNullOrEmpty(sqlDataType))
			{
				var sqlDataTypeMaxLength = 50;
				if (sqlDataType.Length > sqlDataTypeMaxLength)
				{
					errorValidation += $"Native Sql Data Type should be less than {sqlDataTypeMaxLength} characters.;";
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
