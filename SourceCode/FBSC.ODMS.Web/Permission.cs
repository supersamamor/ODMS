using System.Reflection;

namespace FBSC.ODMS.Web;

public static class Permission
{
    // New method specifically for Admin seeding
    public static IEnumerable<string> GenerateAdminPermissions()
    {
        var permissions = new List<string>();

        // Define the modules you want to exclude from the Admin role
        var excludedModules = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            nameof(Entities),
            nameof(Apis),
            nameof(Applications),
            nameof(DataSource)
        };

        // Get nested classes, filtering out the excluded ones.
        // Notice we are intentionally NOT concatenating HTMLTemplate or ApiHub here.
        var nestedClasses = typeof(Permission).GetNestedTypes()
            .Where(t => !excludedModules.Contains(t.Name));

        foreach (var nestedClass in nestedClasses)
        {
            var permissionsInClass = nestedClass.GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == typeof(string))
                .Select(f => f.GetValue(null)?.ToString())
                .Where(val => !string.IsNullOrEmpty(val)); // Safeguard against nulls

            permissions.AddRange(permissionsInClass!);
        }

        return permissions.OrderBy(l => l);
    }

    public static IEnumerable<string> GenerateAllPermissions()
    {
        var permissions = new List<string>();

        var nestedClasses = typeof(Permission).GetNestedTypes()
            .Concat(typeof(HTMLTemplate.HTMLTemplatePermission).GetNestedTypes())
            .Concat(typeof(ApiHub.WebhookPermission).GetNestedTypes());

        foreach (var nestedClass in nestedClasses)
        {
            var permissionsInClass = nestedClass.GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == typeof(string))
                .Select(f => f.GetValue(null)?.ToString())
                .Where(val => !string.IsNullOrEmpty(val));

            permissions.AddRange(permissionsInClass!);
        }
        return permissions.OrderBy(l => l);
    }

    public static string GetPermissionDescription(string permissionValue)
    {
        if (string.IsNullOrEmpty(permissionValue)) return "Unknown";

        var nestedClasses = typeof(Permission).GetNestedTypes()
            .Concat(typeof(HTMLTemplate.HTMLTemplatePermission).GetNestedTypes())
            .Concat(typeof(ApiHub.WebhookPermission).GetNestedTypes());

        foreach (var nestedClass in nestedClasses)
        {
            var fields = nestedClass.GetFields(BindingFlags.Public | BindingFlags.Static)
                                    .Where(f => f.FieldType == typeof(string));

            foreach (var field in fields)
            {
                var value = field.GetValue(null)?.ToString();
                if (value == permissionValue)
                {
                    return $"{nestedClass.Name}.{field.Name}";
                }
            }
        }

        return "Permission Not Found";
    }

    public static IDictionary<string, (string Resource, string Permission)> GetResourcePermissionList()
    {
        var permissionDictionary = new Dictionary<string, (string Resource, string Permission)>();

        var nestedClasses = typeof(Permission).GetNestedTypes(BindingFlags.Public | BindingFlags.Static)
            .Concat(typeof(HTMLTemplate.HTMLTemplatePermission).GetNestedTypes(BindingFlags.Public | BindingFlags.Static))
            .Concat(typeof(ApiHub.WebhookPermission).GetNestedTypes(BindingFlags.Public | BindingFlags.Static));

        foreach (var nestedClass in nestedClasses)
        {
            var fields = nestedClass.GetFields(BindingFlags.Public | BindingFlags.Static)
                                    .Where(f => f.FieldType == typeof(string));

            foreach (var field in fields)
            {
                var value = field.GetValue(null)?.ToString();

                if (!string.IsNullOrEmpty(value))
                {
                    permissionDictionary[value] = (Resource: nestedClass.Name, Permission: field.Name);
                }
            }
        }

        return permissionDictionary;
    }

    public static IEnumerable<string> GeneratePermissionsForModule(string module)
    {
        var permissions = new List<string>();
        var moduleType = typeof(Permission).GetNestedType(module);

        if (moduleType != null)
        {
            var modulePermissions = moduleType.GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == typeof(string))
                .Select(f => f.GetValue(null)?.ToString())
                .Where(val => !string.IsNullOrEmpty(val));

            permissions.AddRange(modulePermissions!);
        }
        return permissions.OrderBy(l => l);
    }

    // --- Sub-classes remain unchanged below ---

    public static class Admin { public const string View = "P.Ad.V"; }
    public static class Entities { public const string View = "P.En.V"; public const string Create = "P.En.C"; public const string Edit = "P.En.E"; }
    public static class Roles { public const string View = "P.Ro.V"; public const string Create = "P.Ro.C"; public const string Edit = "P.Ro.E"; }
    public static class Users { public const string View = "P.Usr.V"; public const string Create = "P.Usr.C"; public const string Edit = "P.Usr.E"; }
    public static class Apis { public const string View = "P.Api.V"; public const string Create = "P.Api.C"; public const string Edit = "P.Api.E"; }
    public static class Applications { public const string View = "P.App.V"; public const string Create = "P.App.C"; public const string Edit = "P.App.E"; }
    public static class AuditTrail { public const string View = "P.AT.V"; }
    public static class Report { public const string View = "P.Rep.V"; }
    public static class AIDrivenDataAnalysisAndInsights { public const string View = "P.AIDrivenDataAnalysisAndInsights.V"; }
    public static class ReportSetup { public const string View = "P.RepS.V"; public const string Create = "P.RepS.C"; public const string Edit = "P.RepS.E"; public const string Delete = "P.RepS.D"; }
    public static class DataSource { public const string View = "P.DataSource.V"; public const string Create = "P.DataSource.C"; public const string Edit = "P.DataSource.E"; public const string Delete = "P.DataSource.D"; public const string History = "P.DataSource.H"; public const string Approve = "P.DataSource.A"; }
    public static class ApproverSetup { public const string Create = "P.ApproverSetup.C"; public const string View = "P.ApproverSetup.V"; public const string Edit = "P.ApproverSetup.E"; }
    public static class PendingApprovals { public const string View = "P.PendingApprovals.V"; public const string Approve = "P.PendingApprovals.A"; }
    public static class ProjectStatusReport { public const string View = "P.ProjectStatusReport.V"; public const string Create = "P.ProjectStatusReport.C"; public const string Edit = "P.ProjectStatusReport.E"; }
    public static class BusinessUnit { public const string View = "P.BusinessUnit.V"; public const string Create = "P.BusinessUnit.C"; public const string Edit = "P.BusinessUnit.E"; public const string Delete = "P.BusinessUnit.D"; public const string Upload = "P.BusinessUnit.U"; public const string History = "P.BusinessUnit.H"; }
    public static class Project { public const string View = "P.Project.V"; public const string Create = "P.Project.C"; public const string Edit = "P.Project.E"; public const string Delete = "P.Project.D"; public const string Upload = "P.Project.U"; public const string History = "P.Project.H"; }
    public static class Employee { public const string View = "P.Employee.V"; public const string Create = "P.Employee.C"; public const string Edit = "P.Employee.E"; public const string Delete = "P.Employee.D"; public const string Upload = "P.Employee.U"; public const string History = "P.Employee.H"; }
    public static class Milestone { public const string View = "P.Milestone.V"; public const string Create = "P.Milestone.C"; public const string Edit = "P.Milestone.E"; public const string Delete = "P.Milestone.D"; public const string Upload = "P.Milestone.U"; public const string History = "P.Milestone.H"; }
}