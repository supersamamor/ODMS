using FBSC.Common.Web.Utility.Extensions;
using FBSC.ODMS.Web.Areas.Admin.Models;
using FBSC.ODMS.Web.Areas.Admin.Queries.Users;
using FBSC.ODMS.Infrastructure.Data;
using FBSC.ODMS.Web.Models;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;
using static FBSC.ODMS.Web.Areas.Identity.IdentityExtensions;
using FBSC.ODMS.Core.Identity;

namespace FBSC.ODMS.Web.Areas.Admin.Pages.Users;

[Authorize(Policy = Permission.Users.Edit)]
public class EditModel(IdentityContext context,
                     UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager) : BasePageModel<EditModel>
{
    [BindProperty]
    public UserViewModel UserModel { get; set; } = new() { IsEdit = true };
    public async Task<IActionResult> OnGet(string id)
    {
        if (id == null)
        {
            return NotFound();
        }
        return await Mediatr.Send(new GetUserByIdQuery(id))
                            .ToActionResult(async user =>
                            {
                                UserModel = await GetViewModel(user);
                                UserModel.Roles = await GetRolesForUser(user);
                                return Page();
                            }, none: null);
    }

    async Task<UserViewModel> GetViewModel(ApplicationUser user)
    {
        var userModel = new UserViewModel
        {
            Id = user.Id,
            Email = user.Email!,
            Name = user.Name!,         
            EntityId = user.EntityId!,
            IsActive = user.IsActive,
            IsEdit = true,
        };
        userModel.Entities = await context.GetEntitiesList(userModel.EntityId);
        return userModel;
    }

    public async Task<IActionResult> OnPost()
    {
        UserModel.Entities = await context.GetEntitiesList(UserModel.EntityId);
        if (!ModelState.IsValid)
        {
            return Page();
        }
        return await Mediatr.Send(new GetUserByIdQuery(UserModel.Id))
            .ToActionResult(
            async user =>
            {
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                return await UpdateUser(user).BindT(async u => await UpdateRolesForUser(u))
                .ToActionResult(
                    user =>
                    {
                        scope.Complete();
                        Logger.LogInformation("Updated User. ID: {ID}, User: {User}", user.Id, user.ToString());
                        NotyfService.Success(Localizer["Record saved successfully"]);
                        return RedirectToPage("View", new { id = user.Id });
                    },
                    errors =>
                    {
                        Logger.LogError("Error in OnPost. Error: {Errors}", string.Join(",", errors));
                        errors.Iter(error => ModelState.AddModelError("", error.ToString()));
                        return Page();
                    });
            }, none: null);
    }
    public IActionResult OnPostChangeFormValue()
    {
        ModelState.Clear();

        return Partial("_InputFieldsPartial", UserModel);
    }
    async Task<IList<UserRoleViewModel>> GetRolesForUser(ApplicationUser user)
    {
        var userRoles = await userManager.GetRolesAsync(user);
        return [.. roleManager.Roles.Map(r => new UserRoleViewModel
		{
			Id = r.Id,
			Name = r.Name!,
			Selected = userRoles.Any(c => c == r.Name)
		})];
    }

    async Task<Validation<Error, ApplicationUser>> UpdateUser(ApplicationUser user)
    {
        user.Name = UserModel.Name;    
        user.EntityId = UserModel.EntityId;
        user.IsActive = UserModel.IsActive;
        return await ToValidation<ApplicationUser>(userManager.UpdateAsync)(user);
    }

    async Task<Validation<Error, ApplicationUser>> UpdateRolesForUser(ApplicationUser user) =>
        await userManager.RemoveAllRoles(user)
                          .BindT(async u => await AddRolesToUser(u));

    async Task<Validation<Error, ApplicationUser>> AddRolesToUser(ApplicationUser user)
    {
        var roles = UserModel.Roles.Where(r => r.Selected).Select(r => r.Name);
        return await userManager.AddRoles(user, roles);
    }
}