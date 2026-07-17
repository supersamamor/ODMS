using System.Reflection;
namespace FBSC.ODMS.Web;

public static class Permission
{
    public static IEnumerable<string> GenerateAllPermissions()
	{
		var permissions = new List<string>();
		// Get all nested classes in the Permissions class
		var nestedClasses = typeof(Permission).GetNestedTypes().Concat(typeof(HTMLTemplate.HTMLTemplatePermission).GetNestedTypes()
			.Concat(typeof(ApiHub.WebhookPermission).GetNestedTypes()));
		foreach (var nestedClass in nestedClasses)
		{
			// Get all public static string fields in the nested class
			var permissionsInClass = nestedClass.GetFields(BindingFlags.Public | BindingFlags.Static)
				.Where(f => f.FieldType == typeof(string))
				.Select(f => f.GetValue(null)!.ToString());

			permissions.AddRange(permissionsInClass!);
		}
		return permissions.OrderBy(l => l);
	}

	public static string GetPermissionDescription(string permissionValue)
	{
		if (string.IsNullOrEmpty(permissionValue)) return "Unknown";

		// Get all nested classes within the Permission class
		var nestedClasses = typeof(Permission).GetNestedTypes().Concat(typeof(HTMLTemplate.HTMLTemplatePermission).GetNestedTypes()
			.Concat(typeof(ApiHub.WebhookPermission).GetNestedTypes()));

		foreach (var nestedClass in nestedClasses)
		{
			// Get all public static string fields (your constants)
			var fields = nestedClass.GetFields(BindingFlags.Public | BindingFlags.Static)
									.Where(f => f.FieldType == typeof(string));

			foreach (var field in fields)
			{
				var value = field.GetValue(null)?.ToString();

				// If the value matches (e.g., "P.Ad.V"), return "ClassName.FieldName"
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

		// 1. Get all nested classes within both Permission and HTMLTemplatePermission
		var nestedClasses = typeof(Permission).GetNestedTypes(BindingFlags.Public | BindingFlags.Static)
			.Concat(typeof(HTMLTemplate.HTMLTemplatePermission).GetNestedTypes(BindingFlags.Public | BindingFlags.Static))
			.Concat(typeof(ApiHub.WebhookPermission).GetNestedTypes(BindingFlags.Public | BindingFlags.Static));

		foreach (var nestedClass in nestedClasses)
		{
			// 2. Get all public static string fields in the nested class
			var fields = nestedClass.GetFields(BindingFlags.Public | BindingFlags.Static)
									.Where(f => f.FieldType == typeof(string));

			foreach (var field in fields)
			{
				var value = field.GetValue(null)?.ToString();

				if (!string.IsNullOrEmpty(value))
				{
					// Key: "P.Ad.V" 
					// Value: (Resource: "Admin", Permission: "View")
					permissionDictionary[value] = (Resource: nestedClass.Name, Permission: field.Name);
				}
			}
		}

		return permissionDictionary;
	}

	public static IEnumerable<string> GeneratePermissionsForModule(string module)
	{
		var permissions = new List<string>();
		// Get the nested class for the specified module
		var moduleType = typeof(Permission).GetNestedType(module);
		if (moduleType != null)
		{
			// Get all public static string fields in the module class
			var modulePermissions = moduleType.GetFields(BindingFlags.Public | BindingFlags.Static)
				.Where(f => f.FieldType == typeof(string))
				.Select(f => f.GetValue(null)!.ToString());
			permissions.AddRange(modulePermissions!);
		}     
		return permissions.OrderBy(l => l);
	}

    public static class Admin
	{
		public const string View = "P.Ad.V";
	}

	public static class Entities
	{
		public const string View = "P.En.V";
		public const string Create = "P.En.C";
		public const string Edit = "P.En.E";   
	}

	public static class Roles
	{
		public const string View = "P.Ro.V";
		public const string Create = "P.Ro.C";
		public const string Edit = "P.Ro.E";     
	}

	public static class Users
	{
		public const string View = "P.Usr.V";
		public const string Create = "P.Usr.C";
		public const string Edit = "P.Usr.E";  
	}

	public static class Apis
	{
		public const string View = "P.Api.V";
		public const string Create = "P.Api.C";
		public const string Edit = "P.Api.E";
	}

	public static class Applications
	{
		public const string View = "P.App.V";
		public const string Create = "P.App.C";
		public const string Edit = "P.App.E";     
	}

	public static class AuditTrail
	{
		public const string View = "P.AT.V";
	}
	public static class Report
	{
		public const string View = "P.Rep.V";
	}
	public static class AIDrivenDataAnalysisAndInsights
	{
		public const string View = "P.AIDrivenDataAnalysisAndInsights.V";
	}
	public static class ReportSetup
	{
		public const string View = "P.RepS.V";
		public const string Create = "P.RepS.C";
		public const string Edit = "P.RepS.E";
		public const string Delete = "P.RepS.D";
	  
	}
    public static class DataSource
	{
		public const string View = "P.DataSource.V";
		public const string Create = "P.DataSource.C";
		public const string Edit = "P.DataSource.E";
		public const string Delete = "P.DataSource.D";
		public const string Upload = "P.DataSource.U";
		public const string History = "P.DataSource.H";
		public const string Approve = "P.DataSource.A";
	}
	public static class DataSourceSchemaCache
	{
		public const string View = "P.DataSourceSchemaCache.V";
		public const string Create = "P.DataSourceSchemaCache.C";
		public const string Edit = "P.DataSourceSchemaCache.E";
		public const string Delete = "P.DataSourceSchemaCache.D";
		public const string Upload = "P.DataSourceSchemaCache.U";
		public const string History = "P.DataSourceSchemaCache.H";
	}
	public static class DataUploadBatch
	{
		public const string View = "P.DataUploadBatch.V";
		public const string Create = "P.DataUploadBatch.C";
		public const string Edit = "P.DataUploadBatch.E";
		public const string Delete = "P.DataUploadBatch.D";
		public const string Upload = "P.DataUploadBatch.U";
		public const string History = "P.DataUploadBatch.H";
	}
	public static class DataUploadColumn
	{
		public const string View = "P.DataUploadColumn.V";
		public const string Create = "P.DataUploadColumn.C";
		public const string Edit = "P.DataUploadColumn.E";
		public const string Delete = "P.DataUploadColumn.D";
		public const string Upload = "P.DataUploadColumn.U";
		public const string History = "P.DataUploadColumn.H";
	}
	public static class ReportType
	{
		public const string View = "P.ReportType.V";
		public const string Create = "P.ReportType.C";
		public const string Edit = "P.ReportType.E";
		public const string Delete = "P.ReportType.D";
		public const string Upload = "P.ReportType.U";
		public const string History = "P.ReportType.H";
		public const string Approve = "P.ReportType.A";
	}
	public static class DashboardQuery
	{
		public const string View = "P.DashboardQuery.V";
		public const string Create = "P.DashboardQuery.C";
		public const string Edit = "P.DashboardQuery.E";
		public const string Delete = "P.DashboardQuery.D";
		public const string Upload = "P.DashboardQuery.U";
		public const string History = "P.DashboardQuery.H";
		public const string Approve = "P.DashboardQuery.A";
	}
	public static class DashboardQueryParameter
	{
		public const string View = "P.DashboardQueryParameter.V";
		public const string Create = "P.DashboardQueryParameter.C";
		public const string Edit = "P.DashboardQueryParameter.E";
		public const string Delete = "P.DashboardQueryParameter.D";
		public const string Upload = "P.DashboardQueryParameter.U";
		public const string History = "P.DashboardQueryParameter.H";
	}
	public static class DashboardQueryResultColumn
	{
		public const string View = "P.DashboardQueryResultColumn.V";
		public const string Create = "P.DashboardQueryResultColumn.C";
		public const string Edit = "P.DashboardQueryResultColumn.E";
		public const string Delete = "P.DashboardQueryResultColumn.D";
		public const string Upload = "P.DashboardQueryResultColumn.U";
		public const string History = "P.DashboardQueryResultColumn.H";
	}
	public static class DashboardQueryResultCache
	{
		public const string View = "P.DashboardQueryResultCache.V";
		public const string Create = "P.DashboardQueryResultCache.C";
		public const string Edit = "P.DashboardQueryResultCache.E";
		public const string Delete = "P.DashboardQueryResultCache.D";
		public const string Upload = "P.DashboardQueryResultCache.U";
		public const string History = "P.DashboardQueryResultCache.H";
	}
	public static class DashboardTheme
	{
		public const string View = "P.DashboardTheme.V";
		public const string Create = "P.DashboardTheme.C";
		public const string Edit = "P.DashboardTheme.E";
		public const string Delete = "P.DashboardTheme.D";
		public const string Upload = "P.DashboardTheme.U";
		public const string History = "P.DashboardTheme.H";
	}
	public static class Dashboard
	{
		public const string View = "P.Dashboard.V";
		public const string Create = "P.Dashboard.C";
		public const string Edit = "P.Dashboard.E";
		public const string Delete = "P.Dashboard.D";
		public const string Upload = "P.Dashboard.U";
		public const string History = "P.Dashboard.H";
		public const string Approve = "P.Dashboard.A";
	}
	public static class DashboardWidget
	{
		public const string View = "P.DashboardWidget.V";
		public const string Create = "P.DashboardWidget.C";
		public const string Edit = "P.DashboardWidget.E";
		public const string Delete = "P.DashboardWidget.D";
		public const string Upload = "P.DashboardWidget.U";
		public const string History = "P.DashboardWidget.H";
	}
	public static class AiSqlPromptTemplate
	{
		public const string View = "P.AiSqlPromptTemplate.V";
		public const string Create = "P.AiSqlPromptTemplate.C";
		public const string Edit = "P.AiSqlPromptTemplate.E";
		public const string Delete = "P.AiSqlPromptTemplate.D";
		public const string Upload = "P.AiSqlPromptTemplate.U";
		public const string History = "P.AiSqlPromptTemplate.H";
	}
	public static class AiSqlGenerationRequest
	{
		public const string View = "P.AiSqlGenerationRequest.V";
		public const string Create = "P.AiSqlGenerationRequest.C";
		public const string Edit = "P.AiSqlGenerationRequest.E";
		public const string Delete = "P.AiSqlGenerationRequest.D";
		public const string Upload = "P.AiSqlGenerationRequest.U";
		public const string History = "P.AiSqlGenerationRequest.H";
	}
	public static class DashboardRefreshJob
	{
		public const string View = "P.DashboardRefreshJob.V";
		public const string Create = "P.DashboardRefreshJob.C";
		public const string Edit = "P.DashboardRefreshJob.E";
		public const string Delete = "P.DashboardRefreshJob.D";
		public const string Upload = "P.DashboardRefreshJob.U";
		public const string History = "P.DashboardRefreshJob.H";
	}
	public static class DashboardAccess
	{
		public const string View = "P.DashboardAccess.V";
		public const string Create = "P.DashboardAccess.C";
		public const string Edit = "P.DashboardAccess.E";
		public const string Delete = "P.DashboardAccess.D";
		public const string Upload = "P.DashboardAccess.U";
		public const string History = "P.DashboardAccess.H";
	}
	
	public static class ApproverSetup
	{
		public const string Create = "P.ApproverSetup.C";
		public const string View = "P.ApproverSetup.V";
		public const string Edit = "P.ApproverSetup.E";
	}public static class PendingApprovals
	{
		public const string View = "P.PendingApprovals.V";
	}
}
