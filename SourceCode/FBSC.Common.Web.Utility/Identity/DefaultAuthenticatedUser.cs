using FBSC.Common.Identity.Abstractions;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace FBSC.Common.Web.Utility.Identity;

/// <summary>
/// Default implmentation of <see cref="IAuthenticatedUser"/>.
/// </summary>
/// <remarks>
/// Initializes an instance of <see cref="DefaultAuthenticatedUser"/>.
/// </remarks>
/// <param name="httpContextAccessor"></param>
public class DefaultAuthenticatedUser(IHttpContextAccessor httpContextAccessor) : IAuthenticatedUser
{

    /// <summary>
    /// Id of this user.
    /// </summary
    public string? UserId { get; } = httpContextAccessor.HttpContext?.User.FindFirst(Claims.Subject)?.Value;

    /// <summary>
    /// Username of this user.
    /// </summary>
    public string? Username { get; } = httpContextAccessor.HttpContext?.User.FindFirst(Claims.Name)?.Value;

    /// <summary>
    /// Represents the tenant that this user belongs to. Used for multi-tenant support.
    /// </summary>
    public string? Entity { get; } = httpContextAccessor.HttpContext?.User.FindFirst(CustomClaimTypes.Entity)?.Value;

    /// <summary>
    /// Unique identifier for the current request.
    /// </summary>
    public string? TraceId { get; } = Activity.Current?.Id ?? httpContextAccessor.HttpContext?.TraceIdentifier;
    public System.Security.Claims.ClaimsPrincipal? ClaimsPrincipal { get; } = httpContextAccessor?.HttpContext?.User;
}