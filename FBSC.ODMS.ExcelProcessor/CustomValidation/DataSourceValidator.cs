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
					errorValidation += $"Friendly Label For This Connection should be less than {nameMaxLength} characters.;";
				}
				if (await context.DataSource.Where(l => l.Name == name).AsNoTracking().IgnoreQueryFilters().AnyAsync())
				{
					errorValidation += $"Friendly Label For This Connection already exist from the database.";
				}
			}
			var systemType = rowValue[nameof(DataSourceState.SystemType)]?.ToString();
			if (!string.IsNullOrEmpty(systemType))
			{
				var systemTypeMaxLength = 50;
				if (systemType.Length > systemTypeMaxLength)
				{
					errorValidation += $"HRIS, Accounting, CRM, or Custom — free-text category for grouping/filtering should be less than {systemTypeMaxLength} characters.;";
				}
			}
			var connectionMode = rowValue[nameof(DataSourceState.ConnectionMode)]?.ToString();
			if (!string.IsNullOrEmpty(connectionMode))
			{
				var connectionModeMaxLength = 30;
				if (connectionMode.Length > connectionModeMaxLength)
				{
					errorValidation += $"LiveConnection or UploadedDataset should be less than {connectionModeMaxLength} characters.;";
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
			
			
			var lastTestStatus = rowValue[nameof(DataSourceState.LastTestStatus)]?.ToString();
			if (!string.IsNullOrEmpty(lastTestStatus))
			{
				var lastTestStatusMaxLength = 50;
				if (lastTestStatus.Length > lastTestStatusMaxLength)
				{
					errorValidation += $"Last Connection Test Result should be less than {lastTestStatusMaxLength} characters.;";
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
