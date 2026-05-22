using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;

namespace SC.SenseTower.Auth.Generators
{
    public class LoggingTokenResponseGenerator : TokenResponseGenerator
    {
        private readonly ILogger<TokenResponseGenerator> logger;

        public LoggingTokenResponseGenerator(
            ISystemClock clock,
            ITokenService tokenService,
            IRefreshTokenService refreshTokenService,
            IScopeParser scopeParser,
            IResourceStore resources,
            IClientStore clients,
            ILogger<TokenResponseGenerator> logger) : base(clock, tokenService, refreshTokenService, scopeParser, resources, clients, logger)
        {
            this.logger = logger;
        }

        public override async Task<TokenResponse> ProcessAsync(TokenRequestValidationResult request)
        {
            var result = await base.ProcessAsync(request);
            try
            {
                var log = new
                {
                    request.ValidatedRequest.AccessTokenLifetime,
                    request.ValidatedRequest.AccessTokenType,
                    request.ValidatedRequest.AuthorizationCode,
                    request.ValidatedRequest.AuthorizationCodeHandle,
                    request.ValidatedRequest.ClientId,
                    request.ValidatedRequest.CodeVerifier,
                    request.ValidatedRequest.Confirmation,
                    request.ValidatedRequest.DeviceCode,
                    request.ValidatedRequest.GrantType,
                    request.ValidatedRequest.Options,
                    request.ValidatedRequest.Raw,
                    RefreshToken = request.ValidatedRequest.RefreshToken == null
                        ? null
                        : new
                            {
                                request.ValidatedRequest.RefreshToken.AccessToken,
                                request.ValidatedRequest.RefreshToken.ClientId,
                                request.ValidatedRequest.RefreshToken.ConsumedTime,
                                request.ValidatedRequest.RefreshToken.CreationTime,
                                request.ValidatedRequest.RefreshToken.Description,
                                request.ValidatedRequest.RefreshToken.Lifetime,
                                request.ValidatedRequest.RefreshToken.Scopes,
                                request.ValidatedRequest.RefreshToken.SessionId,
                                request.ValidatedRequest.RefreshToken.SubjectId,
                                request.ValidatedRequest.RefreshToken.Version
                            },
                    request.ValidatedRequest.RefreshTokenHandle,
                    request.ValidatedRequest.RequestedScopes,
                    request.ValidatedRequest.Secret,
                    request.ValidatedRequest.SessionId,
                    //request.ValidatedRequest.Subject,
                    request.ValidatedRequest.UserName,
                    request.ValidatedRequest.ValidatedResources
                };
                var message = $"LoggingTokenResponseGenerator.ProcessAsync(): Error = {request.Error}, " +
                    $"ValidatedRequest = {JsonConvert.SerializeObject(log)}"
                    ;
                logger.LogDebug(message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка логирования результата генерации токена");
            }
            return result;
        }
    }
}
