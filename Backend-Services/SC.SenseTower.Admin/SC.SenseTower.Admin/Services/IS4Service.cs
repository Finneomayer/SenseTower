using AutoMapper;
using IdentityModel.Client;
using Microsoft.Extensions.Options;
using SC.SenseTower.Admin.Dto.Identity;
using SC.SenseTower.Common.Exceptions;
using SC.SenseTower.Common.Models;
using SC.SenseTower.Common.Services;

namespace SC.SenseTower.Admin.Services
{
    public class IS4Service : BaseHttpService
    {
        private const string OPENID_CONFIGURATION_ENDPOINT = ".well-known/openid-configuration";

        private readonly IS4Configuration config;

        public IS4Service(
            ILogger<IS4Service> logger,
            IMapper mapper,
            HttpClient httpClient,
            IOptions<IS4Configuration> options) : base(logger, mapper, httpClient)
        {
            config = options.Value;
        }

        public async Task<RefreshTokenResultDto> Refresh(string? refreshToken, CancellationToken cancellationToken)
        {
            var serverConfig = await GetOpenIdConfiguration(cancellationToken);
            var response = await httpClient.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = serverConfig.TokenEndpoint,
                ClientId = config.ClientId,
                GrantType = "refresh_token",
                RefreshToken = refreshToken
            }, cancellationToken);
            if (response?.IsError ?? true)
            {
                if (response?.Error == "invalid_grant")
                    throw new ScException("Неверные логин и/или пароль");
                else
                    throw new Exception(response?.Error ?? "Неизвестная ошибка при получении токена");
            }

            return Mapper.Map<RefreshTokenResultDto>(response);
        }

        private async Task<OpenIdConfigurationDto?> GetOpenIdConfiguration(CancellationToken cancellationToken)
        {
            var config = await Get<OpenIdConfigurationDto>(null, OPENID_CONFIGURATION_ENDPOINT, null, cancellationToken);
            if (config != null)
            {
                var scheme = httpClient?.BaseAddress?.Scheme;
                config.AuthorizationEndpoint = AdjustScheme(config.AuthorizationEndpoint, scheme);
                config.CheckSessionIframe = AdjustScheme(config.CheckSessionIframe, scheme);
                config.DeviceAuthorizationEndpoint = AdjustScheme(config.DeviceAuthorizationEndpoint, scheme);
                config.EndSessionEndpoint = AdjustScheme(config.EndSessionEndpoint, scheme);
                config.IntrospectionEndpoint = AdjustScheme(config.IntrospectionEndpoint, scheme);
                config.Issuer = AdjustScheme(config.Issuer, scheme);
                config.JwksUri = AdjustScheme(config.JwksUri, scheme);
                config.RevocationEndpoint = AdjustScheme(config.RevocationEndpoint, scheme);
                config.TokenEndpoint = AdjustScheme(config.TokenEndpoint, scheme);
                config.UserinfoEndpoint = AdjustScheme(config.UserinfoEndpoint, scheme);
            }
            return config;

            string AdjustScheme(string endpoint, string? scheme)
            {
                var endpointScheme = endpoint.Substring(0, endpoint.IndexOf(':'));
                if (!endpointScheme.Equals(scheme, StringComparison.InvariantCultureIgnoreCase))
                    endpoint = endpoint.Replace(endpointScheme + ':', scheme + ':');
                return endpoint;
            }
        }
    }
}
