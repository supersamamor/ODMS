using FBSC.Common.Web.Utility.Authorization;
using FBSC.Common.Web.Utility.Extensions;
using FBSC.ODMS.Web.Areas.Admin.Models;
using FBSC.ODMS.Infrastructure.Data;
using FBSC.ODMS.Web.Models;
using FBSC.ODMS.Core.Oidc;
using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using System.Text.Json;
using static LanguageExt.Prelude;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace FBSC.ODMS.Web.Areas.Admin.Pages.Applications;

[Authorize(Policy = Permission.Applications.Edit)]
public class EditModel(OpenIddictApplicationManager<OidcApplication> manager, IdentityContext context) : BasePageModel<EditModel>
{
    [BindProperty]
    public ApplicationViewModel Application { get; set; } = new();

    [BindProperty]
    public IList<ResourcePermissionViewModel> ApplicationPermissions { get; set; } = [];

    public async Task<IActionResult> OnGet(string? id)
    {
        if (id == null)
		{
			return NotFound();
		}
		return await Optional(await manager.FindByClientIdAsync(id))
			.ToActionResult(async application =>
			{
				var descriptor = new OpenIddictApplicationDescriptor();
				await manager.PopulateAsync(descriptor, application!);
				var scopes = descriptor.Permissions.Where(p => p.StartsWith(Permissions.Prefixes.Scope))
												   .Map(p => p[4..])
												   .ToHashSet();
				var allPermissions = Permission.GetResourcePermissionList();
				ApplicationPermissions = [.. allPermissions
					.GroupBy(kvp => kvp.Value.Resource)
					.Select(group =>
					{
						var resourceName = group.Key;

						// Helper function to find the shortcode (Key) for a specific field name (Permission)
						string? GetCode(string fieldName) =>
							group.FirstOrDefault(x => x.Value.Permission == fieldName).Key;

						var viewCode = GetCode("View") ?? GetCode("V");
						var createCode = GetCode("Create") ?? GetCode("C");
						var editCode = GetCode("Edit") ?? GetCode("E");
						var deleteCode = GetCode("Delete") ?? GetCode("D");
						var uploadCode = GetCode("Upload") ?? GetCode("U");
						var historyCode = GetCode("History") ?? GetCode("H");
						var approveCode = GetCode("Approve") ?? GetCode("A");
						return new ResourcePermissionViewModel
						{
							ResourceName = resourceName,

							ViewPermission = viewCode,
							View = viewCode != null && scopes.Contains(viewCode),

							CreatePermission = createCode,
							Create = createCode != null && scopes.Contains(createCode),

							EditPermission = editCode,
							Edit = editCode != null && scopes.Contains(editCode),

							DeletePermission = deleteCode,
							Delete = editCode != null && scopes.Contains(editCode),

							UploadPermission = uploadCode,
							Upload = editCode != null && scopes.Contains(editCode),

							HistoryPermission = historyCode,
							History = editCode != null && scopes.Contains(editCode),

							ApprovePermission = approveCode,
							Approve = editCode != null && scopes.Contains(editCode),
						};
					})
					.OrderBy(vm => vm.ResourceName)];

				Application = new()
				{
					ClientId = descriptor.ClientId ?? "",
					DisplayName = descriptor.DisplayName ?? "",
					RedirectUri = string.Join(" ", descriptor.RedirectUris),
					PostLogoutRedirectUris = string.Join(" ", descriptor.PostLogoutRedirectUris),
					Scopes = string.Join(" ", scopes.Where(s => !s.StartsWith(AutorizationConstants.POLICY_PREFIX))),
					Entity = application!.Entity,
					CompleteScopes = string.Join(" ", scopes.Where(s => s != AuthorizationClaimTypes.Permission)),
                    Entities = await context.GetEntitiesList(application.Entity),
					WebhookHmacSecret = application.WebhookHmacSecret
                };
				return Page();
			}, none: null);
    }

    public async Task<IActionResult> OnPostGenerateAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }
        return await Optional(await manager.FindByClientIdAsync(Application.ClientId))
            .ToActionResult(async application => await GenerateNewSecret(application!), none: null);
    }

    public async Task<IActionResult> OnPost()
    {
        Application.Entities = await context.GetEntitiesList(Application.Entity);
        if (!ModelState.IsValid)
        {
            return Page();
        }
        return await Optional(await manager.FindByClientIdAsync(Application.ClientId))
            .ToActionResult(async application => await UpdateApplication(application!), none: null);
    }

    async Task<IActionResult> GenerateNewSecret(OidcApplication application)
    {
        return await TryAsync<IActionResult>(async () =>
        {
            Application.ClientSecret = Guid.NewGuid().ToString();
            var descriptor = new OpenIddictApplicationDescriptor();
            await manager.PopulateAsync(descriptor, application);
            descriptor.ClientSecret = Application.ClientSecret;
            await manager.UpdateAsync(application, descriptor, new());
            NotyfService.Success(Localizer["Generated new client secret"]);
            Logger.LogInformation("Updated Client Secret. Client ID: {ClientId}, Application: {Application}", application.ClientId, application.ToString());
            TempData.Put("Application", Application);
            return RedirectToPage("Details");
        }).IfFail(ex =>
        {
            ModelState.AddModelError("", Localizer[$"Something went wrong. Please contact the system administrator."] + $" TraceId = {HttpContext.TraceIdentifier}");
            Logger.LogError(ex, "Exception in OnPostEditAsync");
            return Page();
        });
    }

    async Task<IActionResult> UpdateApplication(OidcApplication application)
    {
        return await TryAsync<IActionResult>(async () =>
        {
            var redirectUris = new System.Collections.Generic.HashSet<Uri> { new(Application.RedirectUri) };
            var postLogoutRedirectUris = new System.Collections.Generic.HashSet<Uri> { new(Application.PostLogoutRedirectUris) };
            var permissions = new System.Collections.Generic.HashSet<string>
        {
            Permissions.Endpoints.Authorization,
            Permissions.Endpoints.DeviceAuthorization,
            Permissions.Endpoints.EndSession,
            Permissions.Endpoints.Token,
            Permissions.GrantTypes.AuthorizationCode,
            Permissions.GrantTypes.ClientCredentials,
            Permissions.GrantTypes.DeviceCode,
            Permissions.GrantTypes.Password,
            Permissions.GrantTypes.RefreshToken,
            Permissions.ResponseTypes.Code,
            Permissions.Prefixes.Scope + AuthorizationClaimTypes.Permission,
        };
            permissions =
            [
                .. permissions,
            .. Application.Scopes.Split(" ")
            .Map(e => Permissions.Prefixes.Scope + e),
        ];

            // Extract enabled permissions from ResourcePermissionViewModel
            var enabledPermissions = ApplicationPermissions
                .SelectMany(resource => new[]
                {
                resource.View && !string.IsNullOrEmpty(resource.ViewPermission)
                    ? resource.ViewPermission : null,
                resource.Create && !string.IsNullOrEmpty(resource.CreatePermission)
                    ? resource.CreatePermission : null,
                resource.Edit && !string.IsNullOrEmpty(resource.EditPermission)
                    ? resource.EditPermission : null,
                resource.Delete && !string.IsNullOrEmpty(resource.DeletePermission)
                    ? resource.DeletePermission : null,
                resource.Upload && !string.IsNullOrEmpty(resource.UploadPermission)
                    ? resource.UploadPermission : null,
                resource.History && !string.IsNullOrEmpty(resource.HistoryPermission)
                    ? resource.HistoryPermission : null,
                resource.Approve && !string.IsNullOrEmpty(resource.ApprovePermission)
                    ? resource.ApprovePermission : null
                })
                .Where(p => p != null)
                .Select(p => p!);

            permissions =
            [
                .. permissions,
            .. enabledPermissions.Map(e => Permissions.Prefixes.Scope + e),
        ];

            application.DisplayName = Application.DisplayName;
            application.RedirectUris = JsonSerializer.Serialize(redirectUris);
            application.PostLogoutRedirectUris = JsonSerializer.Serialize(postLogoutRedirectUris);
            application.Permissions = JsonSerializer.Serialize(permissions);
            application.Entity = Application.Entity;
		    application.WebhookHmacSecret = Application.WebhookHmacSecret;
            await manager.UpdateAsync(application);
            NotyfService.Success(Localizer["Record saved successfully"]);
            Logger.LogInformation("Updated Application. Client ID: {ClientId}, Application: {Application}", application.ClientId, application.ToString());
            return RedirectToPage("View", new { id = Application.ClientId });
        }).IfFail(ex =>
        {
            ModelState.AddModelError("", Localizer[$"Something went wrong. Please contact the system administrator."] + $" TraceId = {HttpContext.TraceIdentifier}");
            Logger.LogError(ex, "Exception in OnPostEditAsync");
            return Page();
        });
    }
}
