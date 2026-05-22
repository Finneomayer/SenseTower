using AutoMapper;
using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Services;
using SC.SenseTower.Images.Dto.Accounts;
using SC.SenseTower.Images.Settings;

namespace SC.SenseTower.Images.Services
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

        public async Task<UserInfoDto?> GetInfo(string accessToken, Guid userId, CancellationToken cancellationToken)
        {
            //var result = await Get<UserInfoDto>(accessToken, endpointsSettings.)
            throw new NotImplementedException();
        }
    }
}
