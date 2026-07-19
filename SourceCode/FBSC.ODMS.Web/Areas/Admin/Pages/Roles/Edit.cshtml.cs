using FBSC.Common.Web.Utility.Extensions;
using FBSC.ODMS.Core.Identity;
using FBSC.ODMS.Web.Areas.Admin.Models;
using FBSC.ODMS.Web.Areas.Admin.Queries.Roles;
using FBSC.ODMS.Web.Models;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;
using FBSC.HTMLTemplate;
using static FBSC.ODMS.Web.Areas.Identity.IdentityExtensions;

namespace FBSC.ODMS.Web.Areas.Admin.Pages.Roles;

[Authorize(Policy = Permission.Roles.Edit)]
public class EditModel(RoleManager<ApplicationRole> roleManager) : BasePageModel<EditModel>
{
    [BindProperty]
    public RoleViewModel Role { get; set; } = new();

    [BindProperty]
    public IList<ResourcePermissionViewModel> Permissions { get; set; } = [];

    public async Task<IActionResult> OnGet(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        return await Mediatr.Send(new GetRoleByIdQuery(id))
                            .ToActionResult(async e =>
                            {
                                Role = Mapper.Map<RoleViewModel>(e);
                                Permissions = await GetPermissionsForRole(e);
                                return Page();
                            }, none: null);
    }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }
        return await Mediatr.Send(new GetRoleByIdQuery(Role.Id)).ToActionResult(
            async r =>
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                return await UpdateRoleFromModel(r)
                .BindT(async r => await UpdatePermissionsForRole(r))
                .ToActionResult(
                    success: role =>
                    {
                        scope.Complete();
                        Logger.LogInformation("Updated Role. ID: {ID}, Role: {Role}", role.Id, role.ToString());
                        NotyfService.Success(Localizer["Record saved successfully"]);
                        return RedirectToPage("View", new { id = role.Id });
                    },
                    fail: errors =>
                    {
                        Logger.LogError("Error in OnPost. Error: {Errors}", string.Join(",", errors));
                        ModelState.AddModelError("", Localizer[$"Something went wrong. Please contact the system administrator."] + $" TraceId = {HttpContext.TraceIdentifier}");
                        return Page();
                    });
            },
            none: null);
    }

    async Task<IList<ResourcePermissionViewModel>> GetPermissionsForRole(ApplicationRole role)
    {
        var roleClaims = (await roleManager.GetClaimsAsync(role)).Map(c => c.Value).ToHashSet();
		var allPermissions = Permission.GetResourcePermissionList();
		return [.. allPermissions
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
					View = viewCode != null && roleClaims.Contains(viewCode),

					CreatePermission = createCode,
					Create = createCode != null && roleClaims.Contains(createCode),

					EditPermission = editCode,
					Edit = editCode != null && roleClaims.Contains(editCode),

					DeletePermission = deleteCode,
					Delete = editCode != null && roleClaims.Contains(editCode),

					UploadPermission = uploadCode,
					Upload = editCode != null && roleClaims.Contains(editCode),

					HistoryPermission = historyCode,
					History = editCode != null && roleClaims.Contains(editCode),

					ApprovePermission = approveCode,
					Approve = editCode != null && roleClaims.Contains(editCode),
				};
			})
			.OrderBy(vm => vm.ResourceName)];
    }

    async Task<Validation<Error, ApplicationRole>> UpdateRoleFromModel(ApplicationRole roleToUpdate)
    {
        roleToUpdate.Name = Role.Name;
        roleToUpdate.NormalizedName = Role.Name.ToUpper();
        return await UpdateRole(roleToUpdate);
    }

    async Task<Validation<Error, ApplicationRole>> UpdatePermissionsForRole(ApplicationRole role) =>
        await roleManager.RemoveAllPermissionClaims(role)
                          .BindT(async r => await AddPermissionsToRole(r));

    async Task<Validation<Error, ApplicationRole>> AddPermissionsToRole(ApplicationRole role)
    {
        var permissions = Permissions
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

        return await roleManager.AddPermissionClaims(role, permissions);
    }

    Func<ApplicationRole, Task<Validation<Error, ApplicationRole>>> UpdateRole =>
        ToValidation<ApplicationRole>(roleManager.UpdateAsync);
}
