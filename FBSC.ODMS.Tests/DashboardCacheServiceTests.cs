using FBSC.ODMS.Infrastructure.Services.Dashboard;

namespace FBSC.ODMS.Tests;

public class DashboardCacheServiceTests
{
    [Fact]
    public void ComputeParameterSetHash_IsDeterministic()
    {
        var parameters = new Dictionary<string, string?> { ["DepartmentId"] = "5", ["Year"] = "2026" };

        var hash1 = DashboardCacheService.ComputeParameterSetHash(parameters);
        var hash2 = DashboardCacheService.ComputeParameterSetHash(parameters);

        Assert.Equal(hash1, hash2);
    }

    [Fact]
    public void ComputeParameterSetHash_IsOrderIndependent()
    {
        var inOrderA = new Dictionary<string, string?> { ["DepartmentId"] = "5", ["Year"] = "2026" };
        var inOrderB = new Dictionary<string, string?> { ["Year"] = "2026", ["DepartmentId"] = "5" };

        Assert.Equal(
            DashboardCacheService.ComputeParameterSetHash(inOrderA),
            DashboardCacheService.ComputeParameterSetHash(inOrderB));
    }

    [Fact]
    public void ComputeParameterSetHash_DifferentValuesProduceDifferentHashes()
    {
        var a = new Dictionary<string, string?> { ["DepartmentId"] = "5" };
        var b = new Dictionary<string, string?> { ["DepartmentId"] = "6" };

        Assert.NotEqual(
            DashboardCacheService.ComputeParameterSetHash(a),
            DashboardCacheService.ComputeParameterSetHash(b));
    }

    [Fact]
    public void ComputeParameterSetHash_EmptyParameterSetIsStable()
    {
        var empty = new Dictionary<string, string?>();

        var hash1 = DashboardCacheService.ComputeParameterSetHash(empty);
        var hash2 = DashboardCacheService.ComputeParameterSetHash(empty);

        Assert.Equal(hash1, hash2);
        Assert.False(string.IsNullOrEmpty(hash1));
    }

    [Fact]
    public void ComputeParameterSetHash_FitsDashboardQueryResultCacheColumnLength()
    {
        // DashboardQueryResultCacheState.ParameterSetHash / DashboardQueryState.QueryHash are
        // both configured with HasMaxLength(64) in ApplicationContext - a SHA-256 hex string
        // is exactly 64 characters, so this must never silently grow (e.g. if the hash
        // algorithm were ever changed to something with a longer digest).
        var hash = DashboardCacheService.ComputeParameterSetHash(new Dictionary<string, string?> { ["X"] = "1" });

        Assert.Equal(64, hash.Length);
    }
}
