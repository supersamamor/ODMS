using FBSC.ODMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.ExcelProcessor.Models;
using FBSC.ODMS.ExcelProcessor.Helper;


namespace FBSC.ODMS.ExcelProcessor.CustomValidation
{
    public static class EmployeeValidator
    {
        public static async Task<Dictionary<string, object?>>  ValidatePerRecordAsync(ApplicationContext context, Dictionary<string, object?> rowValue)
        {
			string errorValidation = "";
            
			var email = rowValue[nameof(EmployeeState.Email)]?.ToString();
			if (!string.IsNullOrEmpty(email))
			{
				var emailMaxLength = 255;
				if (email.Length > emailMaxLength)
				{
					errorValidation += $"Email should be less than {emailMaxLength} characters.;";
				}
			}
			var employeeCode = rowValue[nameof(EmployeeState.EmployeeCode)]?.ToString();
			if (!string.IsNullOrEmpty(employeeCode))
			{
				var employeeCodeMaxLength = 450;
				if (employeeCode.Length > employeeCodeMaxLength)
				{
					errorValidation += $"Employee Code should be less than {employeeCodeMaxLength} characters.;";
				}
			}
			var name = rowValue[nameof(EmployeeState.Name)]?.ToString();
			if (!string.IsNullOrEmpty(name))
			{
				var nameMaxLength = 255;
				if (name.Length > nameMaxLength)
				{
					errorValidation += $"Name should be less than {nameMaxLength} characters.;";
				}
			}
			
			var userId = rowValue[nameof(EmployeeState.UserId)]?.ToString();
			if (!string.IsNullOrEmpty(userId))
			{
				var userIdMaxLength = 36;
				if (userId.Length > userIdMaxLength)
				{
					errorValidation += $"User Id should be less than {userIdMaxLength} characters.;";
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
