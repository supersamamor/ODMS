using FBSC.Common.Utility.Helpers;
using FBSC.ODMS.Core.Constants;
using FBSC.ODMS.Core.ODMS;
using Microsoft.Data.SqlClient;

namespace FBSC.ODMS.Infrastructure.Extensions;

/// <summary>
/// Encryption-at-rest and connection-string construction for <see cref="DataSourceState"/>.
/// Uses the same AES helper + per-record key convention as FBSC.ApiHub's WebhookApiState
/// (EncryptionHelper, key = EncryptionDecryptionKeyPrefix + entity Id) instead of a second
/// encryption mechanism.
/// </summary>
public static class DataSourceStateExtensions
{
    public static DataSourceState EncryptSecrets(this DataSourceState dataSource, string encryptionDecryptionKeyPrefix) =>
        dataSource with
        {
            PasswordEncrypted = string.IsNullOrEmpty(dataSource.PasswordEncrypted)
                ? dataSource.PasswordEncrypted
                : EncryptionHelper.EncryptPassword(dataSource.PasswordEncrypted, $"{encryptionDecryptionKeyPrefix}{dataSource.Id}"),
            ConnectionStringEncrypted = string.IsNullOrEmpty(dataSource.ConnectionStringEncrypted)
                ? dataSource.ConnectionStringEncrypted
                : EncryptionHelper.EncryptPassword(dataSource.ConnectionStringEncrypted, $"{encryptionDecryptionKeyPrefix}{dataSource.Id}"),
        };

    public static string GetDecryptedPassword(this DataSourceState dataSource, string encryptionDecryptionKeyPrefix) =>
        string.IsNullOrEmpty(dataSource.PasswordEncrypted)
            ? ""
            : EncryptionHelper.DecryptPassword(dataSource.PasswordEncrypted, $"{encryptionDecryptionKeyPrefix}{dataSource.Id}");

    public static string GetDecryptedConnectionString(this DataSourceState dataSource, string encryptionDecryptionKeyPrefix) =>
        string.IsNullOrEmpty(dataSource.ConnectionStringEncrypted)
            ? ""
            : EncryptionHelper.DecryptPassword(dataSource.ConnectionStringEncrypted, $"{encryptionDecryptionKeyPrefix}{dataSource.Id}");

    /// <summary>
    /// Builds a read-intent connection string for this data source. ApplicationIntent=ReadOnly
    /// is a best-effort hint (only enforced against AlwaysOn readable secondaries) - the actual
    /// read-only guarantee comes from the SELECT-only query validator plus a least-privilege
    /// DB login, both of which are enforced independently by the execution engine.
    /// </summary>
    public static string BuildConnectionString(this DataSourceState dataSource, string encryptionDecryptionKeyPrefix)
    {
        if (dataSource.ConnectionMode == DataSourceConnectionMode.ConnectionString)
        {
            var builder = new SqlConnectionStringBuilder(dataSource.GetDecryptedConnectionString(encryptionDecryptionKeyPrefix))
            {
                ApplicationIntent = ApplicationIntent.ReadOnly,
                ConnectTimeout = 15,
            };
            return builder.ConnectionString;
        }

        var connectionStringBuilder = new SqlConnectionStringBuilder
        {
            DataSource = dataSource.ServerAddress,
            InitialCatalog = dataSource.DatabaseName,
            ApplicationIntent = ApplicationIntent.ReadOnly,
            ConnectTimeout = 15,
            TrustServerCertificate = true,
        };

        if (dataSource.AuthenticationType == DataSourceAuthenticationType.Windows)
        {
            connectionStringBuilder.IntegratedSecurity = true;
        }
        else
        {
            connectionStringBuilder.UserID = dataSource.Username;
            connectionStringBuilder.Password = dataSource.GetDecryptedPassword(encryptionDecryptionKeyPrefix);
        }

        return connectionStringBuilder.ConnectionString;
    }
}
