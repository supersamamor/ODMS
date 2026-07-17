using OpenIddict.EntityFrameworkCore.Models;

namespace FBSC.ODMS.Core.Oidc
{
    public class OidcApplication : OpenIddictEntityFrameworkCoreApplication<string, OidcAuthorization, OidcToken>
    {
        public string Entity { get; set; } = "";
		public string? WebhookHmacSecret { get; set; }
    }
    public class OidcAuthorization : OpenIddictEntityFrameworkCoreAuthorization<string, OidcApplication, OidcToken> { }
    public class OidcScope : OpenIddictEntityFrameworkCoreScope<string> { }
    public class OidcToken : OpenIddictEntityFrameworkCoreToken<string, OidcApplication, OidcAuthorization> { }

}
