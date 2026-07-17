using FBSC.Common.Identity.Abstractions;

namespace FBSC.ODMS.Tests.TestHelpers;

public class FakeAuthenticatedUser : IAuthenticatedUser
{
    public string? Entity => "Default";
    public string? TraceId => "test-trace-id";
    public string? UserId => "test-user-id";
    public string? Username => "test-user";
    public System.Security.Claims.ClaimsPrincipal? ClaimsPrincipal => null;
}
