using AutoMapper;
using Microsoft.Extensions.Options;
using SC.SenseTower.Cinemas.Dto.Users;
using SC.SenseTower.Cinemas.Settings;
using SC.SenseTower.Common.Models;
using SC.SenseTower.Common.Services;

namespace SC.SenseTower.Cinemas.Services
{
    public class AccountsService : BaseHttpService
    {
        private readonly ServiceEndpointsSettings endpointsSettings;

        public AccountsService(
            ILogger<AccountsService> logger,
            IMapper mapper,
            HttpClient httpClient,
            IOptions<ServiceEndpointsSettings> options) : base(logger, mapper, httpClient)
        {
            endpointsSettings = options.Value;
        }

        public async Task<bool> Exists(string accessToken, Guid userId, CancellationToken cancellationToken)
        {
            var result = await Get<UserInfoResponseDto>(accessToken, string.Format(endpointsSettings.GetUserInfoUrl, userId), null, cancellationToken);
            return result != null;
        }

        public async Task<UserInfoResponseDto?> GetUserInfo(string accessToken, Guid userId, CancellationToken cancellationToken)
        {
            var result = await Get<UserInfoResponseDto>(accessToken, string.Format(endpointsSettings.GetUserInfoUrl, userId), null, cancellationToken);
            return result;
        }

        public async Task<IEnumerable<LookupItemDto<Guid>>> Lookup(string accessToken, Guid[] userIds, CancellationToken cancellationToken)
        {
            var result = await Post<LookupItemDto<Guid>[]>(accessToken, endpointsSettings.AccountsLookupUrl, new { userIds }, cancellationToken);
            return result ?? Array.Empty<LookupItemDto<Guid>>();
        }

        public async Task<IEnumerable<UserInfoResponseDto>> GetByIds(string accessToken, Guid[] userIds, CancellationToken cancellationToken)
        {
            var result = await Post<UserInfoResponseDto[]>(accessToken, endpointsSettings.AccountsGetByIdsUrl, new { userIds }, cancellationToken);
            return result ?? Array.Empty<UserInfoResponseDto>();
        }
    }
}
