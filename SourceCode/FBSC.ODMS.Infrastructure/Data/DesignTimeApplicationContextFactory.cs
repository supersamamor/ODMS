using FBSC.Common.Identity.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Security.Claims;

namespace FBSC.ODMS.Infrastructure.Data;

/// <summary>
/// Design-time factory so `dotnet ef migrations add` can materialize
/// ApplicationContext without a running host - lets any project (e.g. the
/// Scheduler) act as the startup project when the Web app's bin is locked.
/// Scaffolding never opens a connection, so no connection string is needed.
/// </summary>
public class DesignTimeApplicationContextFactory : IDesignTimeDbContextFactory<ApplicationContext>
{
    public ApplicationContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
        optionsBuilder.UseSqlServer();
        return new ApplicationContext(optionsBuilder.Options, new DesignTimeUser());
    }

    private sealed class DesignTimeUser : IAuthenticatedUser
    {
        public string? Entity => null;
        public string? TraceId => null;
        public string? UserId => "design-time";
        public string? Username => "design-time";
        public ClaimsPrincipal? ClaimsPrincipal => null;
    }
}
