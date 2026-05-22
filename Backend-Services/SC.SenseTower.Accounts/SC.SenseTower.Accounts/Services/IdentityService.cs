using AutoMapper;
using IdentityModel.Client;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using SC.SenseTower.Accounts.Constants;
using SC.SenseTower.Accounts.Dto.Identity;
using SC.SenseTower.Accounts.Dto.Server;
using SC.SenseTower.Accounts.Dto.UserInfo;
using SC.SenseTower.Accounts.Settings;
using SC.SenseTower.Common.Exceptions;
using SC.SenseTower.Common.Models;
using SC.SenseTower.Common.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text.Json;

namespace SC.SenseTower.Accounts.Services
{
    public class IdentityService : BaseHttpService
    {
        private const string OPENID_CONFIGURATION_ENDPOINT = ".well-known/openid-configuration";

        private readonly IdentityServerSettings settings;
        private readonly IS4Configuration is4Config;

        private readonly IMemoryCache cache;

        public IdentityService(
            ILogger<IdentityService> logger,
            HttpClient httpClient,
            IOptions<IdentityServerSettings> options,
            IOptions<IS4Configuration> is4Options,
            IMemoryCache cache,
            IMapper mapper) : base(logger, mapper, httpClient)
        {
            this.cache = cache;
            settings = options.Value;
            is4Config = is4Options.Value;
            httpClient.BaseAddress = new Uri(settings.BaseUrl);
        }

        public async Task<ClientLogonResultDto> GetClientToken(string clientId, CancellationToken cancellationToken)
        {
            var serverConfig = await GetOpenIdConfiguration(cancellationToken);
            var response = await httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = serverConfig.TokenEndpoint,
                ClientId = clientId
            });
            if (response.IsError)
            {
                if (response.Error == "invalid_grant")
                    throw new ScException("Неверные данные клиента");
                else if (response.Error == "unauthorized_client")
                    throw new ScException("Неавторизованный клиент");
                else
                    throw new Exception(response.Error ?? "Неизвестная ошибка при получении токена");
            }
            return Mapper.Map<ClientLogonResultDto>(response);
        }

        public async Task<CheckIdentityResultDto> CheckIdentity(Guid userId, string accessToken, string tokenToVerify, CancellationToken cancellationToken)
        {
            var result = new CheckIdentityResultDto { UserId = userId, IsTokenValid = true };

            JwtSecurityToken? token = null;
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                token = tokenHandler.ReadJwtToken(tokenToVerify);
            }
            catch
            {
                result.IsTokenValid = false;
                result.Errors.Add("Токен имеет неверный формат.");
            }
            if (token != null)
            {
                if (!Guid.TryParse(token.Subject, out var tokenUserId))
                {
                    result.IsTokenValid = false;
                    result.Errors.Add("Идентификатор пользователя в токене имеет неверный формат.");
                }
                if (tokenUserId != userId)
                {
                    result.IsTokenValid = false;
                    result.Errors.Add("Токен не принадлежит пользователю.");
                }
                if (token.ValidTo < DateTime.UtcNow.AddMinutes(1))
                {
                    result.IsTokenValid = false;
                    result.Errors.Add("Токен просрочен.");
                }
                if (!token.Audiences.Contains(is4Config.Audience) || token.Issuer != is4Config.Authority)
                {
                    result.IsTokenValid = false;
                    result.Errors.Add("Параметры токена не соответствуют ожидаемым.");
                }
                if (result.IsTokenValid)
                {
                    try
                    {
                        var checkTokenResult = await Get<bool>(tokenToVerify, settings.CheckToken, null, cancellationToken);
                        if (!checkTokenResult)
                        {
                            result.IsTokenValid = false;
                            result.Errors.Add("Токен недействителен");
                        }
                    }
                    catch (HttpRequestException ex)
                    {
                        result.IsTokenValid = false;
                        if (ex.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            result.Errors.Add("Токен недействителен");
                        }
                        else
                        {
                            result.Errors.Add($"Ошибка проверки токена: {ex.StatusCode} - {ex.Message}");
                        }
                    }
                    catch (Exception ex)
                    {
                        result.IsTokenValid = false;
                        result.Errors.Add($"Ошибка проверки токена: {ex.Message}");
                    }
                }
            }

            var isBlocked = await Get<bool?>(accessToken, settings.IsUserBlocked, new { userId }, cancellationToken);

            result.IsUserBlocked = isBlocked == true;
            result.IsUserDeleted = isBlocked == null;

            return result;
        }

        public async Task<string> SetPassword(Guid? userId, string? password, string? token, CancellationToken cancellationToken)
        {
            try
            {
                await Post<bool>(null, settings.SetPasswordUrl, new { UserId = userId, Password = password, Code = token }, cancellationToken);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<bool> CheckLoginOrEmail(string loginOrEmail, CancellationToken cancellationToken)
        {
            try
            {
                return await Post<bool>(null, settings.CheckLoginOrEmailUrl, new { loginOrEmail }, cancellationToken);
            }
            catch
            {
                return false;
            }
        }

        public async Task<SendResetPasswordResultDto?> SendResetPasswordMail(string loginOrEmail, CancellationToken cancellationToken)
        {
            return await Post<SendResetPasswordResultDto>(null, settings.SendResetPasswordUrl, new { loginOrEmail }, cancellationToken);
        }

        public async Task<string> ConfirmEmail(Guid userId, string code, CancellationToken cancellationToken)
        {
            try
            {
                await Post<bool>(null, settings.ConfirmEmailUrl, new { userId, code }, cancellationToken);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<bool> DeleteUser(string? token, Guid? userId, CancellationToken cancellationToken)
        {
            var result = await Post<bool>(token, settings.DeleteUserUrl, new { userId }, cancellationToken);
            return result;
        }

        public async Task<LookupItemDto<Guid>[]?> LookupUsers(Guid[]? userIds, string? roleName, string? accessToken, CancellationToken cancellationToken)
        {
            var result = await Post<LookupItemDto<Guid>[]?>(accessToken, settings.LookupUsersUrl, new { userIds, roleName }, cancellationToken);
            return result;
        }

        public async Task<RegisterResultDto> Register(string login, string email, string password, DateTime? accessGrantedTo, CancellationToken cancellationToken)
        {
            var result = await Post<Guid?>(null, settings.RegisterUrl, new { login, email, password, accessGrantedTo }, cancellationToken);
            if (result == null)
                return new RegisterResultDto { Message = "Ошибка регистрации" };
            return new RegisterResultDto { UserId = result };
        }

        public async Task<TokenResponseDto?> Logon(string login, string password, CancellationToken cancellationToken)
        {
            var serverConfig = await GetOpenIdConfiguration(cancellationToken);
            var result = await httpClient.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = serverConfig.TokenEndpoint,
                ClientId = settings.ClientId,
                UserName = login,
                Password = password
            });
            if (result?.IsError ?? true)
            {
                if (result?.Error == "invalid_grant")
                    throw new ScException("Неверные логин и/или пароль");
                else
                    throw new Exception(result?.Error ?? "Неизвестная ошибка при получении токена");
            }

            return Mapper.Map<TokenResponseDto>(result);
        }

        public async Task<UserInfoResponseDto?> GetIdentityInfo(string token, CancellationToken cancellationToken)
        {
            var serverConfig = await GetOpenIdConfiguration(cancellationToken);
            var response = await httpClient.GetUserInfoAsync(new UserInfoRequest
            {
                Address = serverConfig.UserinfoEndpoint,
                Token = token
            });
            if (response.IsError)
                throw new ScException(response.Error);

            return JsonSerializer.Deserialize<UserInfoResponseDto>(response.Raw);
        }

        public async Task<bool> IsLoginFree(string login, CancellationToken cancellationToken)
        {
            try
            {
                var result = await Get<bool>(null, settings.IsLoginFreeUrl, new { login }, cancellationToken);
                return result;
            }
            catch (ScException)
            {
                return false;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> IsEmailFree(string email, CancellationToken cancellationToken)
        {
            try
            {
                var result = await Get<bool>(null, settings.IsEmailFreeUrl, new { email }, cancellationToken);
                return result;
            }
            catch (ScException)
            {
                return false;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> IsEmailConfirmed(Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await Get<bool>(null, settings.IsEmailConfirmedUrl, new { userId }, cancellationToken);
                return result;
            }
            catch (ScException)
            {
                return false;
            }
            catch
            {
                throw;
            }
        }

        public async Task<RefreshUserTokenResultDto> Refresh(string refreshToken, CancellationToken cancellationToken)
        {
            var serverConfig = await GetOpenIdConfiguration(cancellationToken);
            var response = await httpClient.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = serverConfig.TokenEndpoint,
                ClientId = settings.ClientId,
                GrantType = "refresh_token",
                RefreshToken = refreshToken
            }, cancellationToken);
            if (response?.IsError ?? true)
            {
                if (response?.Error == "invalid_grant")
                    throw new ScException("Невалидный токен");
                else
                    throw new Exception(response?.Error ?? "Неизвестная ошибка при получении токена");
            }

            return Mapper.Map<RefreshUserTokenResultDto>(response);
        }

        public async Task<UserInfoDto?> GetUserInfo(string? accessToken, Guid userId, CancellationToken cancellationToken)
        {
            var result = await Get<UserInfoDto>(accessToken, $"{settings.UserInfoUrl}/{userId}", null, cancellationToken);
            return result;
        }

        public async Task<UserInfoDto[]> GetByIds(string accessToken, Guid[]? userIds, CancellationToken cancellationToken)
        {
            var result = await Post<UserInfoDto[]>(accessToken, settings.UsersByIdsUrl, new { userIds }, cancellationToken);
            return result ?? Array.Empty<UserInfoDto>();
        }

        #region Privates

        private async Task<OpenIdConfigurationDto> GetOpenIdConfiguration(CancellationToken cancellationToken)
        {
            var result = await cache.GetOrCreateAsync(CacheKeys.SERVER_CONFIG, async e =>
            {
                e.SlidingExpiration = TimeSpan.FromMinutes(10);
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
            });
            if (result == null)
                throw new Exception("Не удалось прочитать конфигурацию OpenID");
            return result;

            string AdjustScheme(string endpoint, string? scheme)
            {
                var endpointScheme = endpoint.Substring(0, endpoint.IndexOf(':'));
                if (!endpointScheme.Equals(scheme, StringComparison.InvariantCultureIgnoreCase))
                    endpoint = endpoint.Replace(endpointScheme + ':', scheme + ':');
                return endpoint;
            }
        }

        #endregion
    }
}
