using FBSC.ODMS.Core.Constants;
using FBSC.ODMS.Core.ODMS;
using FBSC.ODMS.Infrastructure.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace FBSC.ODMS.Infrastructure.Services.Dashboard;

/// <summary>
/// Builds live connections to a <see cref="DataSourceState"/>, regardless of whether it's an
/// ExternalDatabase (a remote server) or an UploadedFile (staged locally in this app's own
/// database, under the `uploads` schema - see UploadedFileIngestionService). This is the only
/// place in the solution that turns a DataSourceState into an open SqlConnection - every other
/// service (schema discovery, query execution) goes through here, so a DashboardWidget never
/// needs to know or care which kind of connection it's querying.
/// </summary>
public class DataSourceConnectionFactory(IConfiguration configuration)
{
    private string EncryptionDecryptionKeyPrefix => configuration.GetValue<string>("EncryptionDecryptionKeyPrefix")
        ?? throw new InvalidOperationException("EncryptionDecryptionKeyPrefix is missing from configuration.");

    public async Task<SqlConnection> OpenAsync(DataSourceState dataSource, CancellationToken cancellationToken = default)
    {
        var connectionString = dataSource.ConnectionKind == DataSourceConnectionKind.UploadedFile
            ? configuration.GetConnectionString("ApplicationContext")!
            : dataSource.BuildConnectionString(EncryptionDecryptionKeyPrefix);

        var connection = new SqlConnection(connectionString);
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
