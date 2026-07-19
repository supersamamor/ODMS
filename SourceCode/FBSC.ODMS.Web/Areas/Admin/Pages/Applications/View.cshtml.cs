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
using static LanguageExt.Prelude;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace FBSC.ODMS.Web.Areas.Admin.Pages.Applications;

[Authorize(Policy = Permission.Applications.View)]
public class ViewModel(OpenIddictApplicationManager<OidcApplication> manager, IdentityContext context) : BasePageModel<ViewModel>
{
    public ApplicationViewModel Application { get; set; } = new();
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
					Entity = await context.GetEntityName(application!.Entity).IfNoneAsync(""),
					CompleteScopes = string.Join(" ", scopes.Where(s => s != AuthorizationClaimTypes.Permission)),
					WebhookHmacSecret = application.WebhookHmacSecret ?? string.Empty,
				};
				return Page();
			}, none: null);
    }
}
