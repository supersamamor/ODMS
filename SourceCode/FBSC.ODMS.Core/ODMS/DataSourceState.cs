using FBSC.Common.Core.Base.Models;
using System.ComponentModel;

namespace FBSC.ODMS.Core.ODMS;

public record DataSourceState : BaseEntity
{
	public string Name { get; init; } = "";
	public string? ServerAddress { get; init; }
	public string? DatabaseName { get; init; }
	public string? AuthenticationType { get; init; }
	public string? Username { get; init; }
	public string? PasswordEncrypted { get; private set; }
	public string? ConnectionStringEncrypted { get; init; }
	public string? Description { get; init; }
	public bool IsActive { get; init; }

	public string DataSourceType { get; init; } = Constants.DataSourceTypes.SqlServer;
	public string? UploadedFilePath { get; init; }
	public string? GeneratedTableName { get; private set; }
	public string? ImportStatus { get; private set; }
	public string? ImportErrorMessage { get; private set; }
	public DateTime? LastImportedDate { get; private set; }

	public void SetPassword(string plainTextPassword, string encryptionDecryptionKeyPrefix)
	{
		PasswordEncrypted = Common.Utility.Helpers.EncryptionHelper.EncryptPassword(plainTextPassword, $"{encryptionDecryptionKeyPrefix}{Id}");
	}
	public string GetDecryptedPassword(string encryptionDecryptionKeyPrefix)
	{
		return Common.Utility.Helpers.EncryptionHelper.DecryptPassword(PasswordEncrypted ?? "", $"{encryptionDecryptionKeyPrefix}{Id}");
	}
	public void SetImportResult(string generatedTableName, bool success, string? errorMessage)
	{
		GeneratedTableName = success ? generatedTableName : GeneratedTableName;
		ImportStatus = success ? Constants.FileUploadStatus.Done : Constants.FileUploadStatus.Failed;
		ImportErrorMessage = success ? null : errorMessage;
		LastImportedDate = DateTime.UtcNow;
	}
}
