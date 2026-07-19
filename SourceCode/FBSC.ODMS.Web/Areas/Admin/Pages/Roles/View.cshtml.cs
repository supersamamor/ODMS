using FBSC.Common.Web.Utility.Extensions;
using FBSC.ODMS.Core.Identity;
using FBSC.ODMS.Web.Areas.Admin.Models;
using FBSC.ODMS.Web.Areas.Admin.Queries.Roles;
using FBSC.ODMS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FBSC.HTMLTemplate;

namespace FBSC.ODMS.Web.Areas.Admin.Pages.Roles;

[Authorize(Policy = Permission.Roles.View)]
public class ViewModel(RoleManager<ApplicationRole> roleManager) : BasePageModel<ViewModel>
{
    public RoleViewModel Role { get; set; } = new();
    public IList<ResourcePermissionViewModel> Permissions { get; set; } = [];

    public async Task<IActionResult> OnGet(string id)
    {
        if (id == null)
		{
			return NotFound();
		}
		return await Mediatr.Send(new GetRoleByIdQuery(id))
							.ToActionResult(async role =>
							{
								Role = Mapper.Map<RoleViewModel>(role);
								var roleClaims = (await roleManager.GetClaimsAsync(role))
									.Map(c => c.Value)
									.ToHashSet();

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
								return Page();
							}, none: null);
    }
}
