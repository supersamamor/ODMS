using Microsoft.AspNetCore.Authorization;

namespace FBSC.Common.Web.Utility.Authorization;

/// <summary>
/// The permission authorization requirement.
/// </summary>
/// <remarks>
/// Initializes an instance of <see cref="PermissionRequirement"/> with the required permission.
/// </remarks>
/// <param name="permission">The required permission</param>
public class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    /// <summary>
    /// The name of the permission.
    /// </summary>
    public string Permission { get; } = permission;
}