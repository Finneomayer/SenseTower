using AutoMapper;
using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Models;
using SC.SenseTower.Common.Services;
using SC.SenseTower.Spaces.Dto.Accounts;
using SC.SenseTower.Spaces.Settings;

namespace SC.SenseTower.Spaces.Services;

public class AccountsService : BaseHttpService
{
    private readonly ServiceEndpointsSettings settings;

    public AccountsService(
        ILogger<AccountsService> logger,
        IMapper mapper,
        HttpClient httpClient,
        IOptions<ServiceEndpointsSettings> options) : base(logger, mapper, httpClient)
    {
        settings = options.Value;
    }

    public async Task<LookupItemDto<Guid>[]?> Lookup(string? accessToken, Guid[] userIds, CancellationToken cancellationToken)
    {
        var result = await Post<LookupItemDto<Guid>[]?>(accessToken, settings.LookupUsersUrl, new { userIds }, cancellationToken);
        return result;
    }

    public async Task<UserInfoDto?> GetInfo(string? accessToken, Guid userId, CancellationToken cancellationToken)
    {
        var result = await Get<UserInfoDto?>(accessToken, string.Format(settings.AccountsGetInfoUrl, userId), null, cancellationToken);
        return result;
    }
}