using FBSC.Common.Web.Utility.Extensions;
using FBSC.ODMS.Web.Areas.Admin.Models;
using FBSC.ODMS.Web.Areas.Admin.Queries.Users;
using FBSC.ODMS.Infrastructure.Data;
using FBSC.ODMS.Web.Models;
using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using FBSC.ODMS.Core.Identity;

namespace FBSC.ODMS.Web.Areas.Admin.Pages.Users;

[Authorize(Policy = Permission.Users.View)]
public class ViewModel(IdentityContext context, RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager) : BasePageModel<ViewModel>
{
    [BindProperty]
    public UserViewModel UserModel { get; set; } = new() { IsView = true };
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

    async Task<UserViewModel> GetViewModel(ApplicationUser user) =>
        await context.GetEntityName(user.EntityId!).Match(
            entity => new UserViewModel
            {
                Id = user.Id,
                Name = user.Name ?? "",                
                Email = user.Email!,
                Entity = entity,
                IsActive = user.IsActive,
                IsView = true,
            },
            () => new UserViewModel
            {
                Id = user.Id,
                Name = user.Name ?? "",               
                Email = user.Email!,
                Entity = Core.Constants.Entities.Default,
                IsActive = user.IsActive,
                IsView = true,
            });

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
}