using FBSC.Common.Core.Base.Models;

namespace FBSC.ApiHub.Models;
public record WebhookApiState : BaseEntity
{
    public string ClientId { get; init; } = "";
    public string Name { get; init; } = "";
    public string GrantType { get; init; } = "";
    public bool WithinPrivateNetwork { get; init; }
    public string ClientSecret { get; private set; } = "";
    public string? Scope { get; init; }
    public string BaseUrl { get; init; } = "";
    public string? AuthenticationUrl { get; init; }
    public string? HMAC { get; private set; } = "";
    public bool Active { get; init; } = true;
    public string? AdditionalConfigurationJson { get; init; }
    public IList<WebhookEventAssignmentState>? WebhookEventAssignmentList { get; set; }
    public void EncryptClientSecret(string encryptionDecryptionKeyPrefix)
    {
        this.ClientSecret = Common.Utility.Helpers.EncryptionHelper.EncryptPassword(this.ClientSecret ?? "", $"{encryptionDecryptionKeyPrefix}{this.Id}");
    }
    public string GetDecryptedClientSecret(string encryptionDecryptionKeyPrefix)
    {
        return Common.Utility.Helpers.EncryptionHelper.DecryptPassword(this.ClientSecret ?? "", $"{encryptionDecryptionKeyPrefix}{this.Id}");
    }
    public void EncryptHMAC(string encryptionDecryptionKeyPrefix)
    {
        this.HMAC = Common.Utility.Helpers.EncryptionHelper.EncryptPassword(this.HMAC ?? "", $"{encryptionDecryptionKeyPrefix}{this.Id}");
    }
    public string GetDecryptedHMAC(string encryptionDecryptionKeyPrefix)
    {
        return Common.Utility.Helpers.EncryptionHelper.DecryptPassword(this.HMAC ?? "", $"{encryptionDecryptionKeyPrefix}{this.Id}");
    }
    public bool IsAccessTokenExpired()
    {
        var expiresInSeconds = (ExpiresIn == null ? 0 : ExpiresIn.Value) - 60;
        return RequestDateTime == null || expiresInSeconds < 0 || DateTime.UtcNow > RequestDateTime.Value.AddSeconds(expiresInSeconds);
    }
    public bool IsRefreshTokenExpired()
    {
        var refreshExpiresInSeconds = (RefreshExpiresIn == null ? 0 : RefreshExpiresIn.Value) - 60;
        return RequestDateTime == null || DateTime.UtcNow > RequestDateTime.Value.AddSeconds(refreshExpiresInSeconds);
    }
    public void UpdateJwToken(int expiresIn, int refreshExpiredIn, DateTime? requestDateTime,
        string accessToken, string refreshToken)
    {
        this.ExpiresIn = expiresIn;
        this.RefreshExpiresIn = refreshExpiredIn;
        this.RequestDateTime = requestDateTime;
        this.AccessToken = EncryptAccessToken(accessToken, requestDateTime);
        this.RefreshToken = EncryptRefreshToken(refreshToken, requestDateTime);
    }
    private string EncryptAccessToken(string accessToken, DateTime? requestDateTime)
    {
        if (!string.IsNullOrEmpty(accessToken) && requestDateTime != null)
        {
            return Common.Utility.Helpers.EncryptionHelper.EncryptPassword(accessToken, this.Id + requestDateTime.Value.ToString("yyyyMMddHHmmss"));
        }
        else
        {
            return "";
        }
    }
    public string GetDecryptedAccessToken()
    {
        if (!string.IsNullOrEmpty(this.AccessToken) && RequestDateTime != null)
        {
            return Common.Utility.Helpers.EncryptionHelper.DecryptPassword(this.AccessToken, this.Id + this.RequestDateTime.Value.ToString("yyyyMMddHHmmss"));
        }
        else
        {
            return "";
        }
    }
    private string EncryptRefreshToken(string refreshToken, DateTime? requestDateTime)
    {
        if (!string.IsNullOrEmpty(refreshToken) && requestDateTime != null)
        {
            return Common.Utility.Helpers.EncryptionHelper.EncryptPassword(refreshToken, this.Id + requestDateTime.Value.ToString("yyyyMMddHHmmss"));
        }
        else
        {
            return "";
        }
    }
    public string GetDecryptedRefreshToken()
    {
        if (!string.IsNullOrEmpty(this.RefreshToken) && RequestDateTime != null)
        {
            return Common.Utility.Helpers.EncryptionHelper.DecryptPassword(this.RefreshToken, this.Id + this.RequestDateTime.Value.ToString("yyyyMMddHHmmss"));
        }
        else
        {
            return "";
        }
    }
	public void SetClientSecret(string clientSecret)
    {
        this.ClientSecret = clientSecret;
    }
    #region JwToken
    public int? ExpiresIn { get; private set; }
    public int? RefreshExpiresIn { get; private set; }
    public DateTime? RequestDateTime { get; private set; }
    public string? AccessToken { get; private set; } = "";
    public string? RefreshToken { get; private set; } = "";
    #endregion
}
public static class GrantType
{
    public const string ClientCredentials = "client_credentials";
    public const string BasicAuthentication = "basic_authentication";
    public const string BearerToken = "bearer_token";
    public const string None = "n/a";
    public const string ApiKey = "ApiKey";
}

