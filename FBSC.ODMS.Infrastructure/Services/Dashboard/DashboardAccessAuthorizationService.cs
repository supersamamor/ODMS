using FBSC.ODMS.Core.Constants;
using FBSC.ODMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FBSC.ODMS.Infrastructure.Services.Dashboard;

public enum DashboardAccessRank
{
    None = 0,
    View = 1,
    Edit = 2,
    Owner = 3,
}

/// <summary>
/// Resolves a user's effective access to a specific Dashboard through DashboardAccessState
/// (DashboardShare) rather than the blanket Permission.Dashboard.* policy alone - so a user
/// with the generic "can view dashboards" permission still can't open a private dashboard
/// they were never granted access to. Routes entirely through the existing
/// DashboardAccessState/RBAC data instead of a parallel permission system.
/// </summary>
public class DashboardAccessAuthorizationService(ApplicationContext context)
{
    public async Task<DashboardAccessRank> GetEffectiveAccessAsync(
        string dashboardId,
        string? userId,
        IReadOnlyCollection<string> roleNames,
        CancellationToken cancellationToken = default)
    {
        var dashboard = await context.Dashboard.AsNoTracking()
            .SingleOrDefaultAsync(d => d.Id == dashboardId, cancellationToken);
        if (dashboard is null)
        {
            return DashboardAccessRank.None;
        }
        if (!string.IsNullOrEmpty(userId) && dashboard.OwnerUserId == userId)
        {
            return DashboardAccessRank.Owner;
        }

        var grantedLevels = await context.DashboardAccess.AsNoTracking()
            .Where(a => a.DashboardId == dashboardId
                && ((a.GranteeType == DashboardGranteeType.User && a.GranteeId == userId)
                    || (a.GranteeType == DashboardGranteeType.Role && roleNames.Contains(a.GranteeId))))
            .Select(a => a.AccessLevel)
            .ToListAsync(cancellationToken);

        var rank = grantedLevels.Select(ToRank).DefaultIfEmpty(DashboardAccessRank.None).Max();
        if (rank == DashboardAccessRank.None && dashboard.IsPublic)
        {
            rank = DashboardAccessRank.View;
        }
        return rank;
    }

    public async Task<bool> CanViewAsync(string dashboardId, string? userId, IReadOnlyCollection<string> roleNames, CancellationToken cancellationToken = default) =>
        await GetEffectiveAccessAsync(dashboardId, userId, roleNames, cancellationToken) >= DashboardAccessRank.View;

    public async Task<bool> CanEditAsync(string dashboardId, string? userId, IReadOnlyCollection<string> roleNames, CancellationToken cancellationToken = default) =>
        await GetEffectiveAccessAsync(dashboardId, userId, roleNames, cancellationToken) >= DashboardAccessRank.Edit;

    public async Task<bool> IsOwnerAsync(string dashboardId, string? userId, IReadOnlyCollection<string> roleNames, CancellationToken cancellationToken = default) =>
        await GetEffectiveAccessAsync(dashboardId, userId, roleNames, cancellationToken) == DashboardAccessRank.Owner;

    private static DashboardAccessRank ToRank(string accessLevel) => accessLevel switch
    {
        DashboardAccessLevel.Owner => DashboardAccessRank.Owner,
        DashboardAccessLevel.Edit => DashboardAccessRank.Edit,
        DashboardAccessLevel.View => DashboardAccessRank.View,
        _ => DashboardAccessRank.None,
    };
}
