using FBSC.Common.Services.Shared.Interfaces;
using FBSC.ODMS.Web.Areas.Admin.Commands.AuditTrail;
using FBSC.ODMS.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using FBSC.ODMS.Core.Identity;
using FBSC.ODMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using FBSC.Common.Web.Utility.Identity;

namespace FBSC.ODMS.Web.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class ExternalLoginModel(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    ILogger<ExternalLoginModel> logger,
    IMailService emailSender,
    IMediator mediator,
    IdentityContext context) : BasePageModel<ExternalLoginModel>
{
    [BindProperty]
    public InputModel? Input { get; set; }

    public string? ProviderDisplayName { get; set; }

    public string? ReturnUrl { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Full name")]
        public string? Name { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "Please agree to the terms")]
        public bool TermsAccepted { get; set; }
    }

    public IActionResult OnGetAsync()
    {
        return RedirectToPage("./Login");
    }

    public IActionResult OnPost(string provider, string? returnUrl = null)
    {
        // Request a redirect to the external login provider.
        var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
        var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return new ChallengeResult(provider, properties);
    }

    public async Task<IActionResult> OnGetCallbackAsync(string? returnUrl = null, string? remoteError = null)
    {
        returnUrl ??= Url.Content("~/");

        if (remoteError != null)
        {
            ErrorMessage = $"Error from external provider: {remoteError}";
            if (logger.IsEnabled(LogLevel.Error))
            {
                logger.LogError("Error from external provider: {RemoteError}", remoteError);
            }
            return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
        }

        var info = await signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            ErrorMessage = "Error loading external login information.";
            if (logger.IsEnabled(LogLevel.Error))
            {
                logger.LogError("Error loading external login information.");
            }
            return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
        }

        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        var fullName = info.Principal.FindFirstValue(ClaimTypes.Name);
		var fullNameFromOpenIdConnect = info.Principal.FindFirstValue(CustomClaimTypes.OpenIdConnectName);
        var user = await userManager.FindByNameAsync(email!);

        if (user != null && !user.IsActive)
        {
            await mediator.Send(new AddAuditLogCommand { UserId = user.Id, Type = "User is not active", TraceId = TraceId });
            if (logger.IsEnabled(LogLevel.Warning))
            {
                logger.LogWarning("User is not active, Email = {Email}", email);
            }
            return RedirectToPage("./NotActive");
        }

        // Try external login sign-in
        var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

        if (result.Succeeded)
        {
            // Redirect unconfirmed users to RegisterConfirmation
            if (userManager.Options.SignIn.RequireConfirmedAccount && !await userManager.IsEmailConfirmedAsync(user!))
            {
                return RedirectToPage("./RegisterConfirmation", new { Input?.Email });
            }

            return await SuccessRedirect(email!, user!, info, returnUrl);
        }

        if (result.IsLockedOut)
        {
            await mediator.Send(new AddAuditLogCommand { UserId = user!.Id, Type = "User account locked out", TraceId = TraceId });
            if (logger.IsEnabled(LogLevel.Warning))
            {
                logger.LogWarning("User account locked out, Email = {Email}", email);
            }
            return RedirectToPage("./Lockout");
        }

        // Create new user if not exists
        var defaultEntity = context.Entities.Where(l => l.Name == Core.Constants.Entities.Default).AsNoTracking().FirstOrDefault();
        user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            Name = fullName == null || fullName == string.Empty ? fullNameFromOpenIdConnect : fullName,
            EntityId = defaultEntity!.Id,
        };

        var createUserResult = await userManager.CreateAsync(user);
        if (createUserResult.Succeeded)
        {
            var userId = await userManager.GetUserIdAsync(user);
            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId, code },
            protocol: Request.Scheme);

            await emailSender.SendAsync(new()
            {
                To = email!,
                Subject = "Confirm your email",
                Body = $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl!)}'>clicking here</a>."
            });
        }
        _ = await userManager.AddToRoleAsync(user, Core.Constants.Roles.User);

        var addLoginResult = await userManager.AddLoginAsync(user, info);

        // Redirect unconfirmed users after creation
        if (userManager.Options.SignIn.RequireConfirmedAccount && !await userManager.IsEmailConfirmedAsync(user))
        {
            return RedirectToPage("./RegisterConfirmation", new { email });
        }

        // Sign in the user and redirect
        result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
        return await SuccessRedirect(email!, user!, info, returnUrl);
    }

    private async Task<IActionResult> SuccessRedirect(string email, ApplicationUser user, ExternalLoginInfo? info, string returnUrl = "")
    {
        await mediator.Send(new AddAuditLogCommand() { UserId = user!.Id, Type = "User logged in", TraceId = TraceId });
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("User logged in, Email = {Email}, Provider = {LoginProvider}", email, info?.LoginProvider);
        }
        NotyfService.Success($"Logged in as {email}");
        return LocalRedirect(returnUrl);
    }
    public async Task<IActionResult> OnPostConfirmationAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        // Get the information about the user from the external login provider
        var info = await signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            ErrorMessage = "Error loading external login information during confirmation.";
            return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
        }

        if (ModelState.IsValid)
        {
            var defaultEntity = context.Entities.Where(l => l.Name == Core.Constants.Entities.Default).AsNoTracking().FirstOrDefault();
            var user = new ApplicationUser
            {
                UserName = Input?.Email,
                Email = Input?.Email,
                Name = Input?.Name,
                EntityId = defaultEntity!.Id
            };

            var result = await userManager.CreateAsync(user);
            _ = await userManager.AddToRoleAsync(user, Core.Constants.Roles.User);
            if (result.Succeeded)
            {
                result = await userManager.AddLoginAsync(user, info);
                if (result.Succeeded)
                {
                    if (logger.IsEnabled(LogLevel.Information))
                    {
                        logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                    }

                    var userId = await userManager.GetUserIdAsync(user);
                    var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId, code },
                        protocol: Request.Scheme);

                    await emailSender.SendAsync(new()
                    {
                        To = Input!.Email!,
                        Subject = "Confirm your email",
                        Body = $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl!)}'>clicking here</a>."
                    });

                    // If account confirmation is required, we need to show the link if we don't have a real email sender
                    if (userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("./RegisterConfirmation", new { Input.Email });
                    }

                    await signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);

                    return LocalRedirect(returnUrl);
                }
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        ProviderDisplayName = info.ProviderDisplayName;
        ReturnUrl = returnUrl;
        return Page();
    }
}