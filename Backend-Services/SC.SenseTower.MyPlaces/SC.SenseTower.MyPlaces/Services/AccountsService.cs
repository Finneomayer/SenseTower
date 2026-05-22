using AutoMapper;
using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Models;
using SC.SenseTower.Common.Services;
using SC.SenseTower.MyPlaces.Dto.Users;
using SC.SenseTower.MyPlaces.Settings;

namespace SC.SenseTower.MyPlaces.Services
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

        public async Task<UserInfoDto?> GetInfo(string token, CancellationToken cancellationToken)
        {
            var result = await Get<UserInfoDto?>(token, endpointsSettings.AccountsGetInfoUrl, null, cancellationToken);
            return result;
        }

        public async Task<LookupItemDto<Guid>[]?> UsersLookup(string? token, Guid[]? userIds, CancellationToken cancellationToken)
        {
            var result = await Post<LookupItemDto<Guid>[]>(token, endpointsSettings.AccountsLookupUrl, new { userIds }, cancellationToken);
            return result;
        }
    }
}
