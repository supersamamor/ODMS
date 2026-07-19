using FBSC.Common.Web.Utility.Extensions;
using FBSC.ODMS.Web.Areas.Admin.Models;
using FBSC.ODMS.Web.Models;
using FBSC.ODMS.Core.Oidc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using static LanguageExt.Prelude;

namespace FBSC.ODMS.Web.Areas.Admin.Pages.Apis;

[Authorize(Policy = Permission.Apis.Edit)]
public class EditModel(OpenIddictScopeManager<OidcScope> manager) : BasePageModel<EditModel>
{
    [BindProperty]
    public ScopeViewModel Scope { get; set; } = new();

    public async Task<IActionResult> OnGet(string? name)
    {
        if (name == null)
        {
            return NotFound();
        }
        return await Optional(await manager.FindByNameAsync(name))
            .ToActionResult(async scope =>
            {
                var descriptor = new OpenIddictScopeDescriptor();
                await manager.PopulateAsync(descriptor, scope!);
                Scope = new()
                {
                    Name = descriptor.Name ?? "",
                    DisplayName = descriptor.DisplayName ?? "",
                    Resources = string.Join(" ", descriptor.Resources)
                };
                return Page();
            }, none: null);
    }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }
        return await Optional(await manager.FindByNameAsync(Scope.Name))
            .ToActionResult(async scope => await UpdateApi(scope!), none: null);
    }

    async Task<IActionResult> UpdateApi(OidcScope scope)
    {
        return await TryAsync<IActionResult>(async () =>
        {
            await manager.UpdateAsync(scope, new()
            {
                DisplayName = Scope.DisplayName,
                Name = Scope.Name,
                Resources =
                    {
                        Scope.Resources
                    }
            }, new());
            NotyfService.Success(Localizer["Record saved successfully"]);
            Logger.LogInformation("Updated Scope. Name: {Name}, Scope: {Scope}", scope.Name, scope.ToString());
            return RedirectToPage("View", new { name = Scope.Name });
        }).IfFail(ex =>
        {
            ModelState.AddModelError("", Localizer[$"Something went wrong. Please contact the system administrator."] + $" TraceId = {HttpContext.TraceIdentifier}");
            Logger.LogError(ex, "Exception in OnPostEditAsync");
            return Page();
        });
    }
}
