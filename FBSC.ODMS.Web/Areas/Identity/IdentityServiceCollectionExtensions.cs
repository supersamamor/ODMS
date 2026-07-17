using FBSC.Common.Identity.Abstractions;
using FBSC.Common.Web.Utility.Authorization;
using FBSC.Common.Web.Utility.Identity;
using FBSC.ODMS.Infrastructure.Data;
using FBSC.ODMS.Core.Oidc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static OpenIddict.Abstractions.OpenIddictConstants;
using FBSC.ODMS.Core.Identity;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace FBSC.ODMS.Web.Areas.Identity;

public static class IdentityServiceCollectionExtensions
{
    public static IServiceCollection ConfigureIdentityServices(this IServiceCollection services, IConfiguration configuration)
	{

		if (configuration.GetValue<bool>("UseInMemoryDatabase"))
		{
			services.AddDbContext<IdentityContext>(options =>
			{
				options.UseInMemoryDatabase("IdentityContext");
				options.UseOpenIddict<OidcApplication, OidcAuthorization, OidcScope, OidcToken, string>();
			});
		}
		else
		{
			services.AddDbContext<IdentityContext>(options =>
			{
				options.UseSqlServer(configuration.GetConnectionString("IdentityContext"));
				options.UseOpenIddict<OidcApplication, OidcAuthorization, OidcScope, OidcToken, string>();
			});
		}

		services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
				.AddRoles<ApplicationRole>()
				.AddEntityFrameworkStores<IdentityContext>()
				.AddDefaultTokenProviders();

		var microsoftClientId = configuration["Authentication:Microsoft:ClientId"];
		if (!string.IsNullOrEmpty(microsoftClientId))
		{
			services.AddAuthentication().AddOpenIdConnect("Microsoft", "Microsoft", options =>
			{
				options.Authority = "https://login.microsoftonline.com/common/v2.0/";
				options.ClientId = microsoftClientId;
				options.ClientSecret = configuration["Authentication:Microsoft:ClientSecret"]!;
				options.ResponseType = OpenIdConnectResponseType.Code;
				options.CallbackPath = "/signin-microsoft";
				options.TokenValidationParameters.IssuerValidator = Microsoft.IdentityModel.Validators.AadIssuerValidator.GetAadIssuerValidator("https://login.microsoftonline.com/common").Validate;
			});
		}

		var googleClientId = configuration["Authentication:Google:ClientId"];
		if (!string.IsNullOrEmpty(googleClientId))
		{
			services.AddAuthentication()
				.AddGoogle(options =>
				{
					options.ClientId = googleClientId;
					options.ClientSecret = configuration["Authentication:Google:ClientSecret"]!;
				});
		}

		services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, CustomUserClaimsPrincipalFactory>();
		services.AddTransient<IAuthenticatedUser, DefaultAuthenticatedUser>();

		services.Configure<IdentityOptions>(options =>
		{
			options.ClaimsIdentity.UserNameClaimType = Claims.Name;
			options.ClaimsIdentity.UserIdClaimType = Claims.Subject;
			options.ClaimsIdentity.RoleClaimType = Claims.Role;
			options.ClaimsIdentity.EmailClaimType = Claims.Email;
		});

		if (configuration.GetValue<bool>("IsIdentityServerEnabled"))
		{
			services.AddOpenIddict()
			   .AddCore(options =>
			   {
				   options.UseEntityFrameworkCore()
						  .UseDbContext<IdentityContext>()
						  .ReplaceDefaultEntities<OidcApplication, OidcAuthorization, OidcScope, OidcToken, string>();
				   options.UseQuartz();
			   })
			   .AddServer(options =>
			   {
				   // Enable the authorization, device, logout, token, userinfo and verification endpoints.
				   options.SetAuthorizationEndpointUris("/connect/authorize")
						  .SetDeviceAuthorizationEndpointUris("/connect/device")
						  .SetEndSessionEndpointUris("/connect/logout")
						  .SetTokenEndpointUris("/connect/token")
						  .SetUserInfoEndpointUris("/connect/userinfo")
						  .SetEndUserVerificationEndpointUris("/connect/verify");

				   // For SSO across web apps, authorization code flow is sufficient
				   options.AllowAuthorizationCodeFlow()
						  .AllowRefreshTokenFlow(); // For maintaining sessions

				   // Add other flows only if needed by specific clients
				   var allowDeviceFlow = configuration.GetValue<bool>("OpenIddict:AllowDeviceFlow");
				   if (allowDeviceFlow)
					   options.AllowDeviceAuthorizationFlow();

				   var allowClientCredentials = configuration.GetValue<bool>("OpenIddict:AllowClientCredentials");
				   if (allowClientCredentials)
					   options.AllowClientCredentialsFlow();

				   // Supported scopes
				   options.RegisterScopes(
					   Scopes.OpenId,        // Always needed for OIDC
					   Scopes.Email,
					   Scopes.Profile,
					   Scopes.Phone,
					   Scopes.Roles,
					   Scopes.OfflineAccess, // For refresh tokens
					   CustomClaimTypes.Entity,
					   AuthorizationClaimTypes.Permission
				   );

				   // Register the signing and encryption credentials.
				   if (string.IsNullOrEmpty(configuration.GetValue<string>("SslThumbprint")))
				   {
					   options.AddDevelopmentEncryptionCertificate()
							  .AddDevelopmentSigningCertificate();
				   }
				   else
				   {
					   options.AddEncryptionCertificate(configuration.GetValue<string>("SslThumbprint")!)
							  .AddSigningCertificate(configuration.GetValue<string>("SslThumbprint")!);
				   }

				   // Force client applications to use Proof Key for Code Exchange (PKCE).
				   options.RequireProofKeyForCodeExchange();

				   // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
				   options.UseAspNetCore()
						  .EnableStatusCodePagesIntegration()
						  .EnableAuthorizationEndpointPassthrough()
						  .EnableEndSessionEndpointPassthrough()
						  .EnableTokenEndpointPassthrough()						
						  .EnableUserInfoEndpointPassthrough()
						  .EnableEndUserVerificationEndpointPassthrough();

				   // IMPORTANT FOR SSO: Enable proper validation     
				   // Secure settings:
				   if (configuration.GetValue<bool>("OpenIddict:AcceptAnonymousClients"))
				   {
					   options.AcceptAnonymousClients();
				   }

				   if (configuration.GetValue<bool>("OpenIddict:DisableScopeValidation"))
				   {
					   options.DisableScopeValidation();
				   }

				   if (configuration.GetValue<bool>("OpenIddict:DisableAccessTokenEncryption"))
				   {
					   options.DisableAccessTokenEncryption();
				   }
			   })
			   .AddValidation(options =>
			   {
				   // Import the configuration from the local OpenIddict server instance.
				   options.UseLocalServer();

				   // Register the ASP.NET Core host.
				   options.UseAspNetCore();
			   });
		}
		return services;
	}
}
