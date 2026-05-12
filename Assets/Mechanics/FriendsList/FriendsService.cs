using Assets.Mechanics.FriendsList.Models;
using Assets.Scripts.API;
using Assets.Scripts.Client;
using Assets.Scripts.Models;
using Assets.Scripts.Server;
using Assets.Scripts.WebUtils;
using Cysharp.Threading.Tasks;
using Models;
using Newtonsoft.Json;
using Proyecto26;

namespace Assets.Mechanics.FriendsList
{
    public class FriendsService : IFriendsService
    {
        private readonly IClientData _clientData;
        private readonly IServerApiData _serverApiData;
        private bool _isBusy;

        public FriendsService(IClientData clientData, IServerApiData serverApiData)
        {
            _serverApiData = serverApiData;
            _clientData = clientData;
        }

        public async UniTask<GetFriendDTO[]> GetFriendsList(string userId)
        {
            var utcs = new UniTaskCompletionSource<GetFriendDTO[]>();

            if (_isBusy)
            {
                await UniTask.WaitUntil(() => !_isBusy);
            }

            _isBusy = true;

            RequestHelper options = new RequestHelper();
            options.Uri = string.Format(APIService.FriendsUrl,userId);

            if (!string.IsNullOrEmpty(_clientData.AccessToken))
                options.Headers["Authorization"] = $"Bearer {_clientData.AccessToken}";

            HttpResponse<ScResult<GetFriendDTO[]>> registerResult =
                await WebRequestFunctions.GetWithDeserialization<ScResult<GetFriendDTO[]>>(options);

            if (registerResult.ResponseCode == HttpResponse<bool>.SuccessCode)
            {
                utcs.TrySetResult(registerResult.ResponseData.Result);
            }

            _isBusy = false;

            return await utcs.Task;
        }

        public async UniTask<bool> DeleteFriend(string userId, string friendId)
        {
            var utcs = new UniTaskCompletionSource<bool>();
            string url = string.Format(APIService.FriendsUrl,userId);
            RequestHelper options = new RequestHelper();
            options.Uri = $"{url}/{friendId}";
#if UNITY_SERVER
             if (!string.IsNullOrEmpty(_serverApiData.AccessToken))
                options.Headers["Authorization"] = $"Bearer {_serverApiData.AccessToken}";
#endif
#if !UNITY_SERVER
            if (!string.IsNullOrEmpty(_clientData.AccessToken))
                options.Headers["Authorization"] = $"Bearer {_clientData.AccessToken}";
#endif

            HttpResponse<ScResult<bool>> registerResult =
                await WebRequestFunctions.DeleteWithDeserialization<ScResult<bool>>(options);

            if (registerResult.ResponseCode == HttpResponse<bool>.SuccessCode)
            {
                utcs.TrySetResult(registerResult.ResponseData.Result);
            }

            return await utcs.Task;
        }

        public async UniTask<bool> MakeTwoUsersFriendServer(string firstUserToken, string secondUserToken)
        {
            var utcs = new UniTaskCompletionSource<bool>();

            AddFriendDTO addFriendDto = new();
            addFriendDto.FirstUserToken = firstUserToken;
            addFriendDto.SecondUserToken = secondUserToken;
            
            RequestHelper options = new RequestHelper();

            await UniTask.WaitUntil(() => !string.IsNullOrEmpty(ServerApiService.GetFriendsEndPoint));

            options.Uri = ServerApiService.GetFriendsEndPoint;
            options.BodyString = JsonConvert.SerializeObject(addFriendDto);
#if UNITY_SERVER
             if (!string.IsNullOrEmpty(_serverApiData.AccessToken))
                options.Headers["Authorization"] = $"Bearer {_serverApiData.AccessToken}";
#endif
#if !UNITY_SERVER
            if (!string.IsNullOrEmpty(_clientData.AccessToken))
                options.Headers["Authorization"] = $"Bearer {_clientData.AccessToken}";
#endif

            HttpResponse<ScResult<bool>> registerResult =
                await WebRequestFunctions.PostWithDeserialization<ScResult<bool>>(options);

            if (registerResult.ResponseCode == HttpResponse<bool>.SuccessCode)
            {
                utcs.TrySetResult(true);
            }
            else
            {
                utcs.TrySetResult(false);
            }

            return await utcs.Task;
        }
    }
}