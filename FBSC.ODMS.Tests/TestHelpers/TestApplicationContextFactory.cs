using FBSC.ODMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FBSC.ODMS.Tests.TestHelpers;

public static class TestApplicationContextFactory
{
    public static ApplicationContext Create()
    {
        var options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationContext(options, new FakeAuthenticatedUser());
    }

    // No configuration sources needed: UploadedFileIngestionService's GetValue<T> calls
    // fall back to sensible defaults (25MB / 200k rows) when a key is absent, which is fine
    // for the small in-memory fixtures these tests use.
    public static IConfiguration CreateConfiguration() =>
        new ConfigurationBuilder().Build();
}
