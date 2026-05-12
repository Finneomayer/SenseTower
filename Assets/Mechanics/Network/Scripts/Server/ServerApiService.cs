using System;
using Assets.Scripts.Cinema;
using Assets.Scripts.Event;
using Assets.Scripts.Models;
using Assets.Scripts.Server;
using Assets.Scripts.Space;
using Assets.Scripts.WebUtils;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Proyecto26;
using UnityEngine;
using Zenject;

public class ServerApiService : IServerApiService
{
    #region For local testing    
    public const string BaseUrl = DevUrl;

    private const string DemoUrl = "http://51.250.90.38";
    private const string DevUrl = "http://51.250.89.118";
    private const string TestUrl = "http://51.250.89.30";
    private const bool LocalDebugMode = false;
    #endregion

    private const string ServerAuthClientId = "vr_unity";
    private const string ServerAuthClientSecretKey = "ClientSecret";
    private const string ServerDiscoveryUrlKey = "ServerDiscoveryUrlKey";

    private static string ServerAuthClientSecret = "ZDlkZTFhYTM3MTczYjM1MzkxNmFlNTU4OTQ3MmVjZjU3NjFlMDJjZTA1NTdlOWQ4MTM5OTI3MDI3NThkN2JhYQ==";

    private static string ServerAuthUrl = LocalDebugMode ? $"{BaseUrl}:7788/api/v1/accounts/identity/clientlogon" : null;
    private static string PlaceUsersPostUrl = LocalDebugMode ? $"{BaseUrl}:7771/v1/appstatereports/users" : null;
    private static string PlaceUserCheckUrl = LocalDebugMode ? $"{BaseUrl}:7771/v1/appstatereports/check" : null;
    private static string GetPlaceBySpaceUrl = LocalDebugMode ? $"{BaseUrl}:7771/v1/spaces" : null;
    public static string GetCinemaUrl = LocalDebugMode ? $"{BaseUrl}:7776/api/v1/cinemas/byspace" : null;
    public static string GetTowerEventsUrl = LocalDebugMode ? $"{BaseUrl}:7779/api/v1/towerevents" : null;
    public static string GetShopsUrl = LocalDebugMode ? $"{BaseUrl}:7771/shops" : null;
    public static string GetSpaceStaticObjectsUrl = LocalDebugMode ? $"{BaseUrl}:7771/StaticObjects" : null;
    public static string GetSpaceObjectsNewEndPoint = LocalDebugMode ? $"{BaseUrl}:7771/spaces/{{spaceId}}?includes=Objects" : null;
    public static string CheckClientsAccessToSpacesUrl = LocalDebugMode ? $"{BaseUrl}:7771/v1/SessionAuthorities/UsersAccess/spaces" : null;
    public static string GetFriendsEndPoint = LocalDebugMode ? $"{BaseUrl}:7771/Accounts/friends" : null;
    public static string GetMafiaGameEndPoint = LocalDebugMode ? $"{BaseUrl}:7760/mafia" : null;
    public static string RemoteContentUrl = null; //https://storage.yandexcloud.net/st-scenes/dev";
    public static string RemoteServerContentUrl = null; //https://storage.yandexcloud.net/st-scenes/dev/dev/Server";
    public static string GetAvatarRecorderEndPoint = LocalDebugMode ? $"{BaseUrl}:7756/metarecorder/meta-records": null;

    private IServerApiData _serverData;
    private bool _isFirstAuth = true;
    public event Action ServerAuth;

    public ServerApiService()
    {
    }

    [Inject]
    public void Construct(IServerApiData serverData)
    {
        _serverData = serverData;
    }

    public async UniTask Initialize()
    {
        TrySetUrlFromEnvironmentVariable(ref ServerAuthClientSecret, ServerAuthClientSecretKey);

        string serverDiscoveryUrl = null;

        TrySetUrlFromEnvironmentVariable(ref serverDiscoveryUrl, ServerDiscoveryUrlKey);

        if (string.IsNullOrEmpty(serverDiscoveryUrl) && LocalDebugMode)
        {
            serverDiscoveryUrl = $"{BaseUrl}:7763/"; // for local tests
        }

        if (string.IsNullOrEmpty(serverDiscoveryUrl))
        {
            Debug.LogError("Server Api Service initialization error: serverDiscoveryUrl is empty");
            return;
        }

        UnityServerDiscoveryInfo environmentInfo = null;
        while (environmentInfo == null)
        {
            HttpResponse<UnityServerDiscoveryInfo> httpResponse =
                await WebRequestFunctions.GetWithDeserialization<UnityServerDiscoveryInfo>(serverDiscoveryUrl);
            if (httpResponse.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode
                && httpResponse.ResponseData != null)
            {
                environmentInfo = httpResponse.ResponseData;
            }
            else
            {
                await UniTask.Delay(1000);
            }
        }

        Debug.Log("Server Api Service initialization: DiscoveryInfo received");

        if (!LocalDebugMode)
        {
            ServerAuthUrl = environmentInfo.ServerAuthUrl;
            PlaceUsersPostUrl = environmentInfo.PlaceUsersPostUrl;
            PlaceUserCheckUrl = environmentInfo.PlaceUserCheckUrl;
            GetPlaceBySpaceUrl = environmentInfo.GetPlaceBySpaceUrl;
            CheckClientsAccessToSpacesUrl = environmentInfo.CheckClientsAccessToSpacesUrl;
            GetCinemaUrl = environmentInfo.GetCinemaUrl;
            GetTowerEventsUrl = environmentInfo.GetTowerEventsUrl;
            GetShopsUrl = environmentInfo.GetShopsUrl;
            GetSpaceStaticObjectsUrl = environmentInfo.GetSpaceStaticObjectsUrl;
            GetFriendsEndPoint = environmentInfo.GetFriendsEndPoint;
            RemoteContentUrl = environmentInfo.RemoteContent_Url;
            RemoteServerContentUrl = environmentInfo.RemoteServerContent_Url;
            GetMafiaGameEndPoint = environmentInfo.GetMafiaEndPoint;
            GetAvatarRecorderEndPoint = environmentInfo.GetAvatarRecorderEndPoint;
            GetSpaceObjectsNewEndPoint = environmentInfo.GetSpaceObjectsNewEndPoint;
        }

        await AuthServer();
    }

    public async UniTask<bool> AuthServer()
    {
        var data = new WWWForm();
        data.AddField("clientId", ServerAuthClientId);
        data.AddField("clientSecret", ServerAuthClientSecret);

        var utcs = new UniTaskCompletionSource<bool>();
        var url = ServerAuthUrl;

        _serverData.IsRefreshing = true;

        var options = new RequestHelper
        {
            Uri = url,
            FormData = data
        };

        var result = await WebRequestFunctions.PostWithDeserialization<GetTokenResponse>(options);

        if (result == null || result.ResponseData == null)
        {
            utcs.TrySetResult(false);
        }
        else
        {
            utcs.TrySetResult(!string.IsNullOrEmpty(result.ResponseData.AccessToken));
            SetServerData(result.ResponseData);
        }

        _serverData.IsRefreshing = false;

        return await utcs.Task;
    }

    public async UniTask SendServerUsers(RegisterUsersInSpaceData usersInSpaceData)
    {
        if (usersInSpaceData == null || usersInSpaceData.ClientsData == null)
        {
            return;
        }

        string jsonString = JsonConvert.SerializeObject(usersInSpaceData.ClientsData);
        string url = $"{PlaceUsersPostUrl}/{usersInSpaceData.SpaceId}";

        var options = new RequestHelper
        {
            BodyString = jsonString,
            Uri = url,
        };

        await AuthorizedServerRequest(options, WebRequestFunctions.PostWithDeserialization<EmptyResponseData>);
    }

    public async UniTask<bool> CheckUserInSpace(string spaceId, string userId)
    {
        var utcs = new UniTaskCompletionSource<bool>();

        string url = $"{PlaceUserCheckUrl}/{spaceId}/{userId}";
        var options = new RequestHelper
        {
            Uri = url,
        };

        string resultString = await AuthorizedServerRequest(options, WebRequestFunctions.Get);

        if (bool.TryParse(resultString, out bool result))
        {
            utcs.TrySetResult(result);
        }
        else
        {
            utcs.TrySetResult(false);
        }

        return await utcs.Task;
    }

    public async UniTask<LocalSpace> GetPlaceBySpace(string spaceId)
    {
        var utcs = new UniTaskCompletionSource<LocalSpace>();

        string url = $"{GetPlaceBySpaceUrl}/{spaceId}";
        var options = new RequestHelper
        {
            Uri = url,
        };

        LocalSpace result = await AuthorizedServerRequest(options,
            WebRequestFunctions.GetWithDeserialization<LocalSpace>);

        utcs.TrySetResult(result);
        return await utcs.Task;
    }

    public async UniTask<Cinema> GetCinemaById(string cinemaId)
    {
        var utcs = new UniTaskCompletionSource<Cinema>();

        string url = $"{GetCinemaUrl}/{cinemaId}";
        var options = new RequestHelper
        {
            Uri = url,
        };

        Cinema result = await AuthorizedServerRequest(options,
            WebRequestFunctions.GetWithDeserialization<Cinema>);

        utcs.TrySetResult(result);
        return await utcs.Task;
    }

    public async UniTask<TowerEvent[]> GetTowerEvents(TowerEventsFilter filter)
    {
        var utcs = new UniTaskCompletionSource<TowerEvent[]>();

        await UniTask.WaitUntil(() => GetTowerEventsUrl != string.Empty);

        string url = $"{GetTowerEventsUrl}/list";

        var options = new RequestHelper
        {
            Uri = url,
            BodyString = JsonConvert.SerializeObject(filter),
        };

        TowerEvent[] result = await AuthorizedServerRequest(options,
            WebRequestFunctions.PostWithDeserialization<TowerEvent[]>);

        utcs.TrySetResult(result);
        return await utcs.Task;
    }

    private async UniTask<T> AuthorizedServerRequest<T>(RequestHelper options,
        Func<RequestHelper, UniTask<HttpResponse<T>>> requestFunc)
    {
        var utcs = new UniTaskCompletionSource<T>();

        if (_serverData.IsRefreshing)
        {
            await UniTask.WaitWhile(() => _serverData.IsRefreshing);
        }

        bool successAuthorization = false;
        if (string.IsNullOrEmpty(_serverData.AccessToken))
        {
            successAuthorization = await AuthServer();
        }
        SetServerTokenToRequestOptions(options);

        HttpResponse<T> httpResponse = await requestFunc(options);
        if (httpResponse.ResponseCode == HttpResponse<T>.NotAuthorizedCode)
        {
            successAuthorization = await AuthServer();
            if (successAuthorization)
            {
                SetServerTokenToRequestOptions(options);
                httpResponse = await requestFunc(options);
            }
        }
        utcs.TrySetResult(httpResponse.ResponseData);

        return await utcs.Task;
    }

    private void SetServerData(GetTokenResponse tokenResponse)
    {
        if (tokenResponse != null)
        {
            Debug.Log("server auth");
            _serverData.AccessToken = tokenResponse.AccessToken;
            if (_isFirstAuth)
            {
                ServerAuth?.Invoke();
                _isFirstAuth = false;
            }
        }
    }

    private void SetServerTokenToRequestOptions(RequestHelper options)
    {
        if (options.Headers == null)
        {
            options.Headers = new();
        }

        options.Headers["Authorization"] = $"Bearer {_serverData.AccessToken}";
    }

    private bool TrySetUrlFromEnvironmentVariable(ref string urlReference, string environmentVariableKey)
    {
        string url = Environment.GetEnvironmentVariable(environmentVariableKey);
        if (!string.IsNullOrEmpty(url))
        {
            urlReference = url;
            return true;
        }
        return false;
    }
}
