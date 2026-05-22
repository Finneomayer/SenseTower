using AutoMapper;
using MediatR;
using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Cqrs.Handlers;
using SC.SenseTower.Common.Models;
using SC.SenseTower.Common.Services;
using SC.SenseTower.Spaces.Services;
using SC.SenseTower.Spaces.Settings;
using System.Net;

namespace SC.SenseTower.Spaces.State.GetUsersInSpaces;

public class GetUsersInSpacesRequestHandler:  BaseHandler, IRequestHandler<GetUsersInSpacesRequest, GetUsersInSpacesResponse>
{
    private readonly IUserLocationService userLocationService;
    private readonly ISpaceService spaceService;
    private readonly IBaseHttpService httpService;
    private readonly AccountsService accountsService;
    private readonly ServiceEndpointsSettings settings;

    public GetUsersInSpacesRequestHandler(ILogger<GetUsersInSpacesRequestHandler> logger, IMapper mapper,
        IUserLocationService userLocationService, ISpaceService spaceService, IBaseHttpService httpService,
        IOptions<ServiceEndpointsSettings> settings, AccountsService accountsService):
        base(logger, mapper)
    {
        this.userLocationService = userLocationService;
        this.spaceService = spaceService;
        this.httpService = httpService;
        this.accountsService = accountsService;
        this.settings = settings.Value;
    }

    public async Task<GetUsersInSpacesResponse> Handle(GetUsersInSpacesRequest request,
        CancellationToken cancellationToken)
    {
        ServicePointManager
                .ServerCertificateValidationCallback +=
            (sender, cert, chain, sslPolicyErrors) => true;

        List<(Guid userId, Guid spaceId)> usersInSpaces = userLocationService.GetUsersInSpaces();
        var spaces = await spaceService.GetAllSpaces();
        Logger.LogInformation($"Info spaces: {spaces.Length}");
        var usersIds = usersInSpaces.Select(us => us.userId).ToArray();
        Logger.LogInformation($"Info usersIds: {usersIds.Length}");
        var users = await accountsService.Lookup(request.AccessToken, usersIds, cancellationToken);
        Logger.LogInformation($"Info users: {users.Length}");
        //var users = await httpService.Post<LookupItemDto<Guid>[]?>(request.AccessToken,
        //    settings.AccountsRootUrl + settings.LookupUsersUrl, new { usersIds }, cancellationToken);
        var result = usersInSpaces.Select(us =>
                new UserInSpaceInfoDto(us.spaceId, us.userId, users.FirstOrDefault(u => u.Id == us.userId)?.Name ?? "Не найден",
                spaces.FirstOrDefault(s => s.Id == us.spaceId)?.SpaceName ?? "Не найдено")
        ).OrderBy(u => u.UserName).ToArray();
        var totalCount = usersIds.Length;
        if (request.GetCount != null)
        {
            result = result.Take(request.GetCount.Value).ToArray();
        }
        Logger.LogInformation($"Info result {result.Length}");

        return new  GetUsersInSpacesResponse(result, request.GetCount, totalCount);
    }
}