using FBSC.Common.Web.Utility.Extensions;
using FBSC.ODMS.Core.Identity;
using FBSC.ODMS.Web.Areas.Admin.Models;
using FBSC.ODMS.Web.Models;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;
using FBSC.HTMLTemplate;
using static FBSC.ODMS.Web.Areas.Identity.IdentityExtensions;
using static LanguageExt.Prelude;

namespace FBSC.ODMS.Web.Areas.Admin.Pages.Roles;

[Authorize(Policy = Permission.Roles.Create)]
public class AddModel(RoleManager<ApplicationRole> roleManager) : BasePageModel<AddModel>
{
    [BindProperty]
    public RoleViewModel Role { get; set; } = new();

    [BindProperty]
    public IList<ResourcePermissionViewModel> Permissions { get; set; } = [];

    public IActionResult OnGet()
    {
        var allPermissions = Permission.GetResourcePermissionList();

		Permissions = [.. allPermissions
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
		return Page();
    }
    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        return await Optional(await roleManager.FindByNameAsync(Role.Name))
            .MatchAsync(
                Some: role => Fail<Error, ApplicationRole>($"Role with name {role.Name} already exists"),
                None: async () => await CreateRole(new ApplicationRole(Role.Name)).BindT(
                    async r => await AddPermissionsToRole(r))
                )
            .ToActionResult(
            success: role =>
            {
                scope.Complete();
                Logger.LogInformation("Created Role. ID: {ID}, Role: {Role}", role.Id, role.ToString());
                NotyfService.Success(Localizer["Record saved successfully"]);
                return RedirectToPage("View", new { id = role.Id });
            },
            fail: errors =>
            {
                Logger.LogError("Error in OnPost. Error: {Errors}", string.Join(",", errors));
                ModelState.AddModelError("", Localizer[$"Something went wrong. Please contact the system administrator."] + $" TraceId = {HttpContext.TraceIdentifier}");
                return Page();
            });
    }

    async Task<Validation<Error, ApplicationRole>> CreateRole(ApplicationRole role)
    {
        return await TryAsync<Validation<Error, ApplicationRole>>(async () =>
        {
            var result = await roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                return result.Errors.Select(e => e.Description).Map(e => Error.New(e)).ToSeq();
            }
            return role;
        }).IfFail(ex =>
        {
            Logger.LogError(ex, "Exception in OnPost");
            return Error.New(ex.Message);
        });
    }

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
}
