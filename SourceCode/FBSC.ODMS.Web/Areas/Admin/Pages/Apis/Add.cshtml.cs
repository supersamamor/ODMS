using FBSC.Common.Web.Utility.Extensions;
using FBSC.ODMS.Web.Areas.Admin.Models;
using FBSC.ODMS.Web.Models;
using FBSC.ODMS.Core.Oidc;
using LanguageExt;
using LanguageExt.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using static LanguageExt.Prelude;

namespace FBSC.ODMS.Web.Areas.Admin.Pages.Apis;

[Authorize(Policy = Permission.Apis.Create)]
public class AddModel(OpenIddictScopeManager<OidcScope> manager) : BasePageModel<AddModel>
{
    [BindProperty]
    public ScopeViewModel Scope { get; set; } = new();

    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }
        return await Optional(await manager.FindByNameAsync(Scope.Name))
            .MatchAsync(
            scope => Fail<Error, ScopeViewModel>($"API with name {scope!.Name} already exists"),
            async () => await CreateApi())
            .ToActionResult(
            success: scope =>
            {
                NotyfService.Success(Localizer["Record saved successfully"]);
                Logger.LogInformation("Created Scope. Name: {Name}, Scope: {Scope}", scope.Name, scope.ToString());
                return RedirectToPage("View", new { name = Scope.Name });
            },
            fail: errors =>
            {
                errors.Iter(error => ModelState.AddModelError("", error.ToString()));
                Logger.LogError("Error in OnPostAddAsync. Error: {Errors}", string.Join(",", errors));
                return Page();
            });
    }

    async Task<Validation<Error, ScopeViewModel>> CreateApi()
    {
        return await TryAsync(async () =>
        {
            await manager.CreateAsync(new OpenIddictScopeDescriptor
            {
                DisplayName = Scope.DisplayName,
                Name = Scope.Name,
                Resources =
                    {
                        Scope.Resources
                    }
            });
            return Success<Error, ScopeViewModel>(Scope);
        }).IfFail(ex =>
        {
            Logger.LogError(ex, "Exception in OnPostAddAsync");
            return Fail<Error, ScopeViewModel>(Localizer[$"Something went wrong. Please contact the system administrator."] + $" TraceId = {HttpContext.TraceIdentifier}");
        });
    }
}
