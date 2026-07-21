using FBSC.Common.Utility.Extensions;
using FBSC.ODMS.Core.Identity;
using LanguageExt;
using Microsoft.AspNetCore.Identity;

namespace FBSC.ODMS.Web.Areas.Identity.Data;

public static class DefaultRole
{
    public static async Task Seed(IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        var adminRole = new ApplicationRole(Core.Constants.Roles.Admin);

        if (!await roleManager.RoleExistsAsync(adminRole.Name!))
        {
            _ = await roleManager.CreateAsync(adminRole);
        }

        adminRole = await roleManager.FindByNameAsync(adminRole.Name!);

        // Changed to use GenerateAdminPermissions() instead of GenerateAllPermissions()
        var result = await roleManager.AddPermissionClaims(adminRole!, Permission.GenerateAdminPermissions());

        result.IfFail(e => logger.LogError("Error in DefaultRole.Seed. Errors = {Errors}", e.Join().ToString()));

        // Seed the additional business roles with their specific permission sets.
        await SeedRole(serviceProvider, logger, roleManager, Core.Constants.Roles.ProjectManager, new[]
        {
            Permission.Project.View, Permission.Project.Create, Permission.Project.Edit,
            Permission.ProjectStatusReport.View, Permission.ProjectStatusReport.Create, Permission.ProjectStatusReport.Edit
        });

        await SeedRole(serviceProvider, logger, roleManager, Core.Constants.Roles.ExecutiveLeadership, new[]
        {
            Permission.Project.View,
            Permission.ProjectStatusReport.View
        });

        await SeedRole(serviceProvider, logger, roleManager, Core.Constants.Roles.DeliveryPortfolioLead, new[]
        {
            Permission.Project.View, Permission.Project.Create, Permission.Project.Edit,
            Permission.ProjectStatusReport.Edit, Permission.ProjectStatusReport.View,
            Permission.PendingApprovals.Approve
        });

        await SeedRole(serviceProvider, logger, roleManager, Core.Constants.Roles.PmoAdmin, new[]
        {
            Permission.Project.View, Permission.Project.Create, Permission.Project.Edit,
            Permission.ProjectStatusReport.View
        });
    }

    private static async Task SeedRole(
        IServiceProvider serviceProvider,
        ILogger<Program> logger,
        RoleManager<ApplicationRole> roleManager,
        string roleName,
        IEnumerable<string> permissions)
    {
        var role = new ApplicationRole(roleName);

        if (!await roleManager.RoleExistsAsync(role.Name!))
        {
            _ = await roleManager.CreateAsync(role);
        }

        role = await roleManager.FindByNameAsync(role.Name!);

        var result = await roleManager.AddPermissionClaims(role!, permissions);

        result.IfFail(e => logger.LogError("Error seeding role {Role}. Errors = {Errors}", roleName, e.Join().ToString()));
    }
}