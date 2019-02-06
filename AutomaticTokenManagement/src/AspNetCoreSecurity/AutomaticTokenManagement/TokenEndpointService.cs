using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityModel.AspNetCore
{
    public class TokenEndpointService
    {
        private readonly AutomaticTokenManagementOptions _managementOptions;
        private readonly IOptionsSnapshot<OpenIdConnectOptions> _oidcOptions;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<TokenEndpointService> _logger;

        public TokenEndpointService(
            IOptions<AutomaticTokenManagementOptions> managementOptions,
            IOptionsSnapshot<OpenIdConnectOptions> oidcOptions,
            IAuthenticationSchemeProvider schemeProvider,
            IHttpClientFactory httpClientFactory,
            ILogger<TokenEndpointService> logger)
        {
            _managementOptions = managementOptions.Value;
            _oidcOptions = oidcOptions;
            _schemeProvider = schemeProvider;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<TokenResponse> RefreshTokenAsync(string refreshToken)
        {
            var oidcOptions = await GetOidcOptionsAsync();
            var configuration = await oidcOptions.ConfigurationManager.GetConfigurationAsync(default(CancellationToken));

            var tokenClient = _httpClientFactory.CreateClient("tokenClient");

            return await tokenClient.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = configuration.TokenEndpoint,

                ClientId = oidcOptions.ClientId,
                ClientSecret = oidcOptions.ClientSecret,
                RefreshToken = refreshToken
            });
        }

        public async Task<TokenRevocationResponse> RevokeTokenAsync(string refreshToken)
        {
            var oidcOptions = await GetOidcOptionsAsync();
            var configuration = await oidcOptions.ConfigurationManager.GetConfigurationAsync(default(CancellationToken));

            var tokenClient = _httpClientFactory.CreateClient("tokenClient");

            return await tokenClient.RevokeTokenAsync(new TokenRevocationRequest
            {
                Address = configuration.AdditionalData[OidcConstants.Discovery.RevocationEndpoint].ToString(),
                ClientId = oidcOptions.ClientId,
                ClientSecret = oidcOptions.ClientSecret,
                Token = refreshToken,
                TokenTypeHint = OidcConstants.TokenTypes.RefreshToken
            });
        }

        private async Task<OpenIdConnectOptions> GetOidcOptionsAsync()
        {
            if (string.IsNullOrEmpty(_managementOptions.Scheme))
            {
                var scheme = await _schemeProvider.GetDefaultChallengeSchemeAsync();
                return _oidcOptions.Get(scheme.Name);
            }
            else
            {
                return _oidcOptions.Get(_managementOptions.Scheme);
            }
        }
    }
}