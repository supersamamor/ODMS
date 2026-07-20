using FBSC.ODMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.ExcelProcessor.Models;
using FBSC.ODMS.ExcelProcessor.Helper;


namespace FBSC.ODMS.ExcelProcessor.CustomValidation
{
    public static class BusinessUnitValidator
    {
        public static async Task<Dictionary<string, object?>>  ValidatePerRecordAsync(ApplicationContext context, Dictionary<string, object?> rowValue)
        {
			string errorValidation = "";
            var name = rowValue[nameof(BusinessUnitState.Name)]?.ToString();
			if (!string.IsNullOrEmpty(name))
			{
				var nameMaxLength = 150;
				if (name.Length > nameMaxLength)
				{
					errorValidation += $"Name should be less than {nameMaxLength} characters.;";
				}
				if (await context.BusinessUnit.Where(l => l.Name == name).AsNoTracking().IgnoreQueryFilters().AnyAsync())
				{
					errorValidation += $"Name already exist from the database.";
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
				nameof(BusinessUnitState.Name),
								
			];
			return listOfKeys.Count > 0 ? DictionaryHelper.FindDuplicateRowNumbersPerKey(records, listOfKeys) : [];
		}
    }
}
