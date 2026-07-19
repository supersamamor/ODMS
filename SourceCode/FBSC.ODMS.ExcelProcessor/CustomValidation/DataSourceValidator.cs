using FBSC.ODMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.ExcelProcessor.Models;
using FBSC.ODMS.ExcelProcessor.Helper;


namespace FBSC.ODMS.ExcelProcessor.CustomValidation
{
    public static class DataSourceValidator
    {
        public static async Task<Dictionary<string, object?>>  ValidatePerRecordAsync(ApplicationContext context, Dictionary<string, object?> rowValue)
        {
			string errorValidation = "";
            var name = rowValue[nameof(DataSourceState.Name)]?.ToString();
			if (!string.IsNullOrEmpty(name))
			{
				var nameMaxLength = 150;
				if (name.Length > nameMaxLength)
				{
					errorValidation += $"Connection Name should be less than {nameMaxLength} characters.;";
				}
				if (await context.DataSource.Where(l => l.Name == name).AsNoTracking().IgnoreQueryFilters().AnyAsync())
				{
					errorValidation += $"Connection Name already exist from the database.";
				}
			}
			var serverAddress = rowValue[nameof(DataSourceState.ServerAddress)]?.ToString();
			if (!string.IsNullOrEmpty(serverAddress))
			{
				var serverAddressMaxLength = 200;
				if (serverAddress.Length > serverAddressMaxLength)
				{
					errorValidation += $"MS SQL Server Host Or Instance should be less than {serverAddressMaxLength} characters.;";
				}
			}
			var databaseName = rowValue[nameof(DataSourceState.DatabaseName)]?.ToString();
			if (!string.IsNullOrEmpty(databaseName))
			{
				var databaseNameMaxLength = 150;
				if (databaseName.Length > databaseNameMaxLength)
				{
					errorValidation += $"Target Database Name should be less than {databaseNameMaxLength} characters.;";
				}
			}
			var authenticationType = rowValue[nameof(DataSourceState.AuthenticationType)]?.ToString();
			if (!string.IsNullOrEmpty(authenticationType))
			{
				var authenticationTypeMaxLength = 50;
				if (authenticationType.Length > authenticationTypeMaxLength)
				{
					errorValidation += $"SQL, WindowsIntegrated, or AzureAD should be less than {authenticationTypeMaxLength} characters.;";
				}
			}
			var username = rowValue[nameof(DataSourceState.Username)]?.ToString();
			if (!string.IsNullOrEmpty(username))
			{
				var usernameMaxLength = 100;
				if (username.Length > usernameMaxLength)
				{
					errorValidation += $"Connection Username should be less than {usernameMaxLength} characters.;";
				}
			}
			var passwordEncrypted = rowValue[nameof(DataSourceState.PasswordEncrypted)]?.ToString();
			if (!string.IsNullOrEmpty(passwordEncrypted))
			{
				var passwordEncryptedMaxLength = 450;
				if (passwordEncrypted.Length > passwordEncryptedMaxLength)
				{
					errorValidation += $"Encrypted Connection Password should be less than {passwordEncryptedMaxLength} characters.;";
				}
			}
			var connectionStringEncrypted = rowValue[nameof(DataSourceState.ConnectionStringEncrypted)]?.ToString();
			if (!string.IsNullOrEmpty(connectionStringEncrypted))
			{
				var connectionStringEncryptedMaxLength = 450;
				if (connectionStringEncrypted.Length > connectionStringEncryptedMaxLength)
				{
					errorValidation += $"Full Encrypted Connection String should be less than {connectionStringEncryptedMaxLength} characters.;";
				}
			}
			var description = rowValue[nameof(DataSourceState.Description)]?.ToString();
			if (!string.IsNullOrEmpty(description))
			{
				var descriptionMaxLength = 450;
				if (description.Length > descriptionMaxLength)
				{
					errorValidation += $"Description should be less than {descriptionMaxLength} characters.;";
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
				nameof(DataSourceState.Name),
																																								
			];
			return listOfKeys.Count > 0 ? DictionaryHelper.FindDuplicateRowNumbersPerKey(records, listOfKeys) : [];
		}
    }
}
