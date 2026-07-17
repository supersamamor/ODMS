using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace FBSC.ODMS.Infrastructure.Services.Dashboard;

/// <summary>
/// Builds live connections to an external <see cref="DataSourceState"/>. This is the only
/// place in the solution that turns a DataSourceState into an open SqlConnection - every
/// other service (schema discovery, query execution) goes through here so connection-string
/// construction and read-intent enforcement stay in one place.
/// </summary>
public class DataSourceConnectionFactory(IConfiguration configuration)
{
    private string EncryptionDecryptionKeyPrefix => configuration.GetValue<string>("EncryptionDecryptionKeyPrefix")
        ?? throw new InvalidOperationException("EncryptionDecryptionKeyPrefix is missing from configuration.");

    public async Task<SqlConnection> OpenAsync(DataSourceState dataSource, CancellationToken cancellationToken = default)
    {
        var connection = new SqlConnection(dataSource.BuildConnectionString(EncryptionDecryptionKeyPrefix));
        try
        {
            await connection.OpenAsync(cancellationToken);
            return connection;
        }
        catch
        {
            await connection.DisposeAsync();
            throw;
        }
    }

    public async Task<(bool Success, string? ErrorMessage)> TestConnectionAsync(DataSourceState dataSource, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = await OpenAsync(dataSource, cancellationToken);
            await using var command = new SqlCommand("SELECT 1", connection) { CommandTimeout = 10 };
            _ = await command.ExecuteScalarAsync(cancellationToken);
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }
}
