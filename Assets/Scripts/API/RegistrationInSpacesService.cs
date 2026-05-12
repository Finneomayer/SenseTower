using Assets.Scripts.Client;
using Assets.Scripts.Models;
using Assets.Scripts.Server;
using Assets.Scripts.WebUtils;
using Cysharp.Threading.Tasks;
using Models;
using Proyecto26;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.API
{
    public class RegistrationInSpacesService : IRegistrationInSpacesService
    {
        private IApiService _apiService;
        private IClientData _clientData;
        private IServerApiData _serverData;

        [Inject]
        private void Construct(IApiService apiService, IClientData clientData, IServerApiData serverData)
        {
            _apiService = apiService;
            _clientData = clientData;
            _serverData = serverData;
        }

        public async UniTask<bool> Register(string spaceId)
        {
            var utcs = new UniTaskCompletionSource<bool>();

            await UniTask.WaitUntil(() => APIService.RegistrationInSpacesUrl != string.Empty);

            bool result =  await _apiService.PostWithAuthToken($"{APIService.RegistrationInSpacesUrl}/{spaceId}");
            utcs.TrySetResult(result);
            return await utcs.Task;
        }

        public async UniTask<AccessResultDto> CheckAccess(string spaceId)
        {
            var utcs = new UniTaskCompletionSource<AccessResultDto>();

            await UniTask.WaitUntil(() => APIService.CheckUserAccessToSpacesUrl != string.Empty);

            string url = $"{APIService.CheckUserAccessToSpacesUrl}/spaces/{spaceId}/users/{_clientData.UserId}";
            url = APIService.AddLanguageParameter(url);
            
            RequestHelper options = new();
            options.Uri = url;
            options.BodyString = spaceId;
            if (!string.IsNullOrEmpty(_clientData.AccessToken))
                options.Headers["Authorization"] = $"Bearer {_clientData.AccessToken}";
            
            HttpResponse<ScResult<AccessResultDto>> httpResponse =
                await WebRequestFunctions.GetWithDeserialization<ScResult<AccessResultDto>>(options);

            bool success = httpResponse.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode;
            if (success)
            {
                if (httpResponse.ResponseData == null || httpResponse.ResponseData.Result == null)
                {
                    utcs.TrySetResult(null);
                }
                else
                {
                    utcs.TrySetResult(httpResponse.ResponseData.Result);
                }
            }
            else
            {
                Debug.LogWarning($"{nameof(CheckAccess)}. Cannot check access to spaces. " +
                    $"Errorcode:{httpResponse.ResponseCode}");
                utcs.TrySetResult(null);
            }

            return await utcs.Task;
        }

        public async UniTask<AccessResultDto[]> CheckAccess(string[] spaceIds)
        {
            var utcs = new UniTaskCompletionSource<AccessResultDto[]>();

            await UniTask.WaitUntil(() => APIService.CheckUserAccessToSpacesUrl != string.Empty);

            string url = $"{APIService.CheckUserAccessToSpacesUrl}/{_clientData.UserId}/spaces";
            url = APIService.AddLanguageParameter(url);

            HttpResponse<ScResult<AccessResultDto[]>> httpResponse = 
                await WebRequestFunctions.PostWithDeserialization<string[], ScResult<AccessResultDto[]>>
                (url, spaceIds, _clientData.AccessToken);

            bool success = httpResponse.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode;
            if (success)
            {
                if (httpResponse.ResponseData == null || httpResponse.ResponseData.Result == null)
                {
                    utcs.TrySetResult(new AccessResultDto[0]);
                }
                else
                {
                    utcs.TrySetResult(httpResponse.ResponseData.Result);
                }
            }
            else
            {
                Debug.LogWarning($"{nameof(CheckAccess)}. Cannot check access to spaces. " +
                    $"Errorcode:{httpResponse.ResponseCode}");
                utcs.TrySetResult(new AccessResultDto[0]);
            }

            return await utcs.Task;
        }

        public async UniTask<AccessResultDto[]> CheckUsersAccessServer(string spaceId, CheckingTokenDto[] usersData)
        {
            var utcs = new UniTaskCompletionSource<AccessResultDto[]>();

            await UniTask.WaitUntil(() => ServerApiService.CheckClientsAccessToSpacesUrl != string.Empty);

            string url = $"{ServerApiService.CheckClientsAccessToSpacesUrl}/{spaceId}/users";

            HttpResponse<RequestDto<AccessResultDto[]>> httpResponse =
                await WebRequestFunctions.PostWithDeserialization<CheckingTokenDto[], RequestDto<AccessResultDto[]>>
                (url, usersData, _serverData.AccessToken);

            bool success = httpResponse.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode;
            if (success)
            {
                if (httpResponse.ResponseData == null || httpResponse.ResponseData.Result == null)
                {
                    utcs.TrySetResult(new AccessResultDto[0]);
                }
                else
                {
                    utcs.TrySetResult(httpResponse.ResponseData.Result);
                }
            }
            else
            {
                Debug.LogWarning($"{nameof(CheckUsersAccessServer)}. Cannot check access to spaces. " +
                    $"Errorcode:{httpResponse.ResponseCode}");
                utcs.TrySetResult(new AccessResultDto[0]);
            }

            return await utcs.Task;
        }
    }
}