using System.Security.Claims;

namespace Hub.Infrastructure.Architecture.OAuth.Interfaces
{
    /// <summary>
    /// Validates access tokes that have been submitted as part of a request.
    /// </summary>
    public interface IAccessTokenProvider
    {
        AccessTokenResult ValidateToken(string token, string issuerToken = null);

        AccessTokenResult ValidateExternalToken(string token, string externalIssuerToken);

        string GenerateToken(IEnumerable<Claim> claims, double expiryInMinutes = 30, string issuerToken = null);

        Claim RetriveTokenData(AccessTokenResult tokenResult, string claimKeyName);

        void ValidateTokenStatus(AccessTokenResult tokenResult);
    }
}
