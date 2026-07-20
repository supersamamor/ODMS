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

	public void SetPassword(string plainTextPassword, string encryptionDecryptionKeyPrefix)
	{
		PasswordEncrypted = Common.Utility.Helpers.EncryptionHelper.EncryptPassword(plainTextPassword, $"{encryptionDecryptionKeyPrefix}{Id}");
	}
	public string GetDecryptedPassword(string encryptionDecryptionKeyPrefix)
	{
		return Common.Utility.Helpers.EncryptionHelper.DecryptPassword(PasswordEncrypted ?? "", $"{encryptionDecryptionKeyPrefix}{Id}");
	}
}
