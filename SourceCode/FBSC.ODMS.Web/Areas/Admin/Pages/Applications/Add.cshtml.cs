using FBSC.Common.Web.Utility.Authorization;
using FBSC.Common.Web.Utility.Extensions;
using FBSC.ODMS.Core.Oidc;
using FBSC.ODMS.Infrastructure.Data;
using FBSC.ODMS.Web.Areas.Admin.Models;
using FBSC.ODMS.Web.Models;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Core;
using System.Text.Json;
using static LanguageExt.Prelude;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace FBSC.ODMS.Web.Areas.Admin.Pages.Applications;

[Authorize(Policy = Permission.Applications.Create)]
public class AddModel(OpenIddictApplicationManager<OidcApplication> manager, IdentityContext context) : BasePageModel<AddModel>
{
    [BindProperty]
    public ApplicationViewModel Application { get; set; } = new();

    [BindProperty]
    public IList<ResourcePermissionViewModel> ApplicationPermissions { get; set; } = [];

    public async Task<IActionResult> OnGet()
    {
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

						CreatePermission = createCode,

						EditPermission = editCode,

						DeletePermission = deleteCode,

						UploadPermission = uploadCode,

						HistoryPermission = historyCode,

						ApprovePermission = approveCode,
					};
				})
				.OrderBy(vm => vm.ResourceName)];
        Application.Entities = await context.GetEntitiesList(Application.Entity);
        return Page();
    }


    public async Task<IActionResult> OnPost()
    {
        Application.Entities = await context.GetEntitiesList(Application.Entity);
        if (!ModelState.IsValid)
        {
            return Page();
        }
        return (await CreateApplication())
            .ToActionResult(
            success: application =>
            {
                NotyfService.Success(Localizer["Record saved successfully"]);
                Logger.LogInformation("Created Application. Client ID: {ClientId}, Application: {Application}", application.ClientId, application.ToString());
                TempData.Put("Application", Application);
                return RedirectToPage("Details");
            },
            fail: errors =>
            {
                errors.Iter(error => ModelState.AddModelError("", error.ToString()));
                Logger.LogError("Error in OnPostAddAsync. Error: {Errors}", string.Join(",", errors));
                return Page();
            });
    }

  
    async Task<Validation<Error, ApplicationViewModel>> CreateApplication()
    {
        return await TryAsync(async () =>
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
            Application.ClientId = Guid.NewGuid().ToString();
            Application.ClientSecret = Guid.NewGuid().ToString();
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

            var application = new OidcApplication
            {
                ClientId = Application.ClientId,
                DisplayName = Application.DisplayName,
                RedirectUris = JsonSerializer.Serialize(redirectUris),
                PostLogoutRedirectUris = JsonSerializer.Serialize(postLogoutRedirectUris),
                Permissions = JsonSerializer.Serialize(permissions),
                Entity = Application.Entity,
				WebhookHmacSecret = Application.WebhookHmacSecret
            };
            await manager.CreateAsync(application, Application.ClientSecret);
            return Success<Error, ApplicationViewModel>(Application);
        }).IfFail(ex =>
        {
            Logger.LogError(ex, "Exception in OnPostAddAsync");
            return Fail<Error, ApplicationViewModel>(Localizer[$"Something went wrong. Please contact the system administrator."] + $" TraceId = {HttpContext.TraceIdentifier}");
        });
    }
}
