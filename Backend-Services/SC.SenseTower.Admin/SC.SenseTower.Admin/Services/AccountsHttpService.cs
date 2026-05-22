using AutoMapper;
using Microsoft.Extensions.Options;
using SC.SenseTower.Admin.Dto.Identity;
using SC.SenseTower.Admin.Dto.Images;
using SC.SenseTower.Admin.Settings;
using SC.SenseTower.Common.Models;
using SC.SenseTower.Common.Services;

namespace SC.SenseTower.Admin.Services
{
    public class AccountsHttpService : BaseHttpService
    {
        private readonly ServiceEndpointsSettings settings;

        public AccountsHttpService(
            ILogger<AccountsHttpService> logger,
            IMapper mapper,
            HttpClient httpClient,
            IOptions<ServiceEndpointsSettings> options) : base(logger, mapper, httpClient)
        {
            settings = options.Value;
        }

        public async Task<LogonResponseDto?> Logon(string? userName, string? password, CancellationToken cancellationToken)
        {
            return await Post<LogonResponseDto>(null, settings.AccountsLogonUrl, new { Login = userName, Password = password }, cancellationToken);
        }

        public async Task<bool> Delete(Guid userId, string? accessToken, CancellationToken cancellationToken)
        {
            return await Post<bool>(accessToken, settings.AccountsDeleteUrl, new { UserId = userId }, cancellationToken);
        }

        public async Task<IEnumerable<LookupItemDto<Guid>>> Lookup(string accessToken, string refreshToken, Guid[]? userIds, string? roleName, CancellationToken cancellationToken)
        {
            return await Post<LookupItemDto<Guid>[]>(accessToken, settings.AccountsLookupUrl, new { userIds, roleName }, cancellationToken) ?? Array.Empty<LookupItemDto<Guid>>();
        }
    }
}
