using AutoMapper;
using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Services;
using SC.SenseTower.Tickets.Dto.Accounts;
using SC.SenseTower.Tickets.Settings;

namespace SC.SenseTower.Tickets.Services
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

        public async Task<IEnumerable<UserInfoDto>> GetInfoByIds(string accessToken, Guid[] userIds, CancellationToken cancellationToken)
        {
            var result = await Post<UserInfoDto[]>(accessToken, endpointsSettings.AccountsGetInfoByIdsUrl, new { userIds }, cancellationToken);
            return result ?? Array.Empty<UserInfoDto>();
        }
    }
}
