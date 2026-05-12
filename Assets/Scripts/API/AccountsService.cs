using Assets.Scripts.Space;
using Cysharp.Threading.Tasks;
using System;
using Assets.Scripts.Client;
using Zenject;
using Assets.Mechanics.Mafia;
using Assets.Scripts.Models;
using Assets.Scripts.WebUtils;
using Models;
using Newtonsoft.Json;
using UnityEngine;
using Proyecto26;

namespace Assets.Scripts.API
{
    public class AccountsService : IAccountsService
    {
        public event Action BecameFullFledged;
        private IApiService _apiService;
        private IClientData _clientData;
        private bool _isRequestInProgress;
        private bool _isAuthorized = true;

        [Inject]
        public void Construct(IApiService apiService, IClientData clientData)
        {
            _apiService = apiService;
            _clientData = clientData;
            _apiService.AuthSuccess += OnAuth;
        }

        private void OnAuth()
        {
            _isAuthorized = true;
        }

        public async UniTask<UserLookupInfo[]> GetUserLookupInfo(string userId)
        {
            return await GetUsersLookupInfo(new string[] { userId });
        }

        public async UniTask<UserLookupInfo[]> GetUsersLookupInfo(string[] userIds = null)
        {
            var utcs = new UniTaskCompletionSource<UserLookupInfo[]>();

            if (userIds != null && userIds.Length == 0)
            {
                utcs.TrySetResult(Array.Empty<UserLookupInfo>());
                return await utcs.Task;
            }

            await UniTask.WaitUntil(() => !string.IsNullOrEmpty(APIService.GetAllUsersUrl));

            if (_isRequestInProgress)
            {
                await UniTask.WaitWhile(() => _isRequestInProgress);
            }

            _isRequestInProgress = true;

            string url = APIService.GetAllUsersUrl;

            if (userIds != null && userIds.Length > 0)
            {
                url = $"{url}?UserIds={string.Join("&UserIds=", userIds)}";
            }

            url = APIService.AddLanguageParameter(url);

            UserLookupInfo[] result = await _apiService.PostWithAuthToken<UserLookupInfo[]>(url);

            utcs.TrySetResult(result);

            _isRequestInProgress = false;

            return await utcs.Task;
        }

        public async UniTask<int> GetThisUserHours()
        {
            if (_clientData.UserId == null) return -1;

            var utcs = new UniTaskCompletionSource<int>();

            await UniTask.WaitUntil(() => !string.IsNullOrEmpty(APIService.GetUserHoursEndPoint));
            string url = APIService.GetUserHoursEndPoint.Replace("{userId}",_clientData.UserId.ToString());

            await UniTask.WaitUntil(() => _isAuthorized);

            HttpResponse <ScResult<int>> httpResponse =
                await WebRequestFunctions.GetWithDeserialization<ScResult<int>>
                    (url, _clientData.AccessToken);

            if (httpResponse.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode)
            {
                if (httpResponse.ResponseData == null)
                {
                    utcs.TrySetResult(-1);
                }
                else
                {
                    utcs.TrySetResult(httpResponse.ResponseData.Result);
                    Debug.Log($"User time is: {httpResponse.ResponseData.Result} hours");
                }
            }
            else
            {
                Debug.LogWarning($"Can't get user's time. Errorcode:{httpResponse.ResponseCode}");
                utcs.TrySetResult(-1);
            }

            return await utcs.Task;
        }

        public async UniTask<bool> GetIsThisUserFullFledged()
        {
            if (_clientData.UserId == null) return true;

            var utcs = new UniTaskCompletionSource<bool>();

            await UniTask.WaitUntil(() => !string.IsNullOrEmpty(APIService.GetUserFullFledgedStatusEndPoint));
            string url = APIService.GetUserFullFledgedStatusEndPoint.Replace("{userId}", _clientData.UserId.ToString());

            await UniTask.WaitUntil(() => _isAuthorized);

            HttpResponse<ScResult<bool>> httpResponse =
                await WebRequestFunctions.GetWithDeserialization<ScResult<bool>>
                    (url, _clientData.AccessToken);

            if (httpResponse.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode)
            {
                if (httpResponse.ResponseData == null)
                {
                    utcs.TrySetResult(true);
                }
                else
                {
                    utcs.TrySetResult(httpResponse.ResponseData.Result);
                    if (httpResponse.ResponseData.Result) BecameFullFledged?.Invoke();
                    Debug.Log($"User is full fledged user: {httpResponse.ResponseData.Result}");
                }
            }
            else
            {
                Debug.LogWarning($"Can't get full fledged status. Errorcode:{httpResponse.ResponseCode}");
                utcs.TrySetResult(true);
            }
            return await utcs.Task;
        }

        public async UniTask<bool> GetIsThisUserSeller()
        {
            if (_clientData.UserId == null) return false;

            var utcs = new UniTaskCompletionSource<bool>();

            await UniTask.WaitUntil(() => !string.IsNullOrEmpty(APIService.GetUserSellerStatus));
            string url = APIService.GetUserSellerStatus.Replace("{userId}", _clientData.UserId.ToString());
            
            await UniTask.WaitUntil(() => _isAuthorized);

            HttpResponse<ScResult<bool>> httpResponse =
                await WebRequestFunctions.GetWithDeserialization<ScResult<bool>>(url, _clientData.AccessToken);

            if (httpResponse.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode)
            {
                if (httpResponse.ResponseData == null)
                {
                    utcs.TrySetResult(true);
                }
                else
                {
                    utcs.TrySetResult(httpResponse.ResponseData.Result);
                    Debug.Log($"User is seller: {httpResponse.ResponseData.Result}");
                }
            }
            else
            {
                Debug.LogWarning($"Can't get seller status. Errorcode:{httpResponse.ResponseCode}");
                utcs.TrySetResult(true);
            }
            return await utcs.Task;
        }

        public async UniTask<bool> GetHasThisUserInitialBonus()
        {
            if (_clientData.UserId == null) return true;

            var utcs = new UniTaskCompletionSource<bool>();

            await UniTask.WaitUntil(() => !string.IsNullOrEmpty(APIService.GetUserInitialBonusEndPoint));
            string url = APIService.GetUserInitialBonusEndPoint.Replace("{userId}", _clientData.UserId.ToString());

            await UniTask.WaitUntil(() => _isAuthorized);

            HttpResponse<ScResult<bool>> httpResponse =
                await WebRequestFunctions.GetWithDeserialization<ScResult<bool>>
                    (url, _clientData.AccessToken);

            if (httpResponse.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode)
            {
                if (httpResponse.ResponseData == null)
                {
                    utcs.TrySetResult(true);
                }
                else
                {
                    utcs.TrySetResult(httpResponse.ResponseData.Result);
                    Debug.Log($"User has initial bonus: {httpResponse.ResponseData.Result}");
                }
            }
            else
            {
                Debug.LogWarning($"Can't get user's initial bonus having. Errorcode:{httpResponse.ResponseCode}");
                utcs.TrySetResult(true);
            }

            return await utcs.Task;
        }

        public async UniTask<bool> SetThisUserHasInitialBonus()
        {
            var utcs = new UniTaskCompletionSource<bool>();

            await UniTask.WaitUntil(() => !string.IsNullOrEmpty(APIService.GetUserInitialBonusEndPoint));
            string url = APIService.GetUserInitialBonusEndPoint.Replace("{userId}", _clientData.UserId.ToString());

            await UniTask.WaitUntil(() => _isAuthorized);

            RequestHelper options = new RequestHelper();
            options.Uri = url;
            options.Headers["Authorization"] = $"Bearer {_clientData.AccessToken}";

            HttpResponse<ScResult<bool>> httpResponse =
                await WebRequestFunctions.PostWithDeserialization<ScResult<bool>>(options);

            if (httpResponse.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode)
            {
                if (httpResponse.ResponseData == null)
                {
                    utcs.TrySetResult(false);
                }
                else
                {
                    utcs.TrySetResult(httpResponse.ResponseData.Result);
                    Debug.Log($"User has initial bonus set: {httpResponse.ResponseData.Result}");
                }
            }
            else
            {
                Debug.LogWarning($"Can't set user's initial bonus having. Errorcode:{httpResponse.ResponseCode}");
                utcs.TrySetResult(false);
            }

            return await utcs.Task;
        }

        public async UniTask<DateTimeOffset> GetBonusInitialDate()
        {
            var utcs = new UniTaskCompletionSource<DateTimeOffset>();

            await UniTask.WaitUntil(() => !string.IsNullOrEmpty(APIService.GetDateWhenInitialBonusStarted));
            string url = APIService.GetDateWhenInitialBonusStarted;

            await UniTask.WaitUntil(() => _isAuthorized);

            HttpResponse<ScResult<DateTimeOffset>> httpResponse =
                await WebRequestFunctions.GetWithDeserialization<ScResult<DateTimeOffset>>
                    (url, _clientData.AccessToken);

            if (httpResponse.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode)
            {
                if (httpResponse.ResponseData == null)
                {
                    utcs.TrySetResult(DateTimeOffset.MinValue);
                }
                else
                {
                    utcs.TrySetResult(httpResponse.ResponseData.Result);
                    Debug.Log($"User has initial bonus: {httpResponse.ResponseData.Result}");
                }
            }
            else
            {
                Debug.LogWarning($"Can't get user's initial bonus having. Errorcode:{httpResponse.ResponseCode}");
                utcs.TrySetResult(DateTimeOffset.MinValue);
            }

            return await utcs.Task;
        }
    }
}