using Assets.Scripts.API;
using Assets.Scripts.Client;
using Assets.Scripts.Models;
using Assets.Scripts.WebUtils;
using Cysharp.Threading.Tasks;
using System;
using Models;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Hall
{
    [Serializable]
    internal class HallResponse
    {
        public Hall[] Result { get; set; }
    }

    internal class HallsService : IHallsService
    {
        private IApiService _apiService;
        private IClientData _clientData;

        private bool _isRequestInProgress;

        [Inject]
        public void Construct(IApiService apiService, IClientData clientData)
        {
            _apiService = apiService;
            _clientData = clientData;
        }

        public async UniTask<Hall[]> GetHalls()
        {
            var utcs = new UniTaskCompletionSource<Hall[]>();

            var hallResponse = await GetHallsResponse();
            var halls = hallResponse.Result;

            utcs.TrySetResult(halls);
            return await utcs.Task;
        }

        private async UniTask<HallResponse> GetHallsResponse()
        {
            var utcs = new UniTaskCompletionSource<HallResponse>();

            utcs.TrySetResult(await RequestHalls());
            return await utcs.Task;
        }

        public async UniTask<Hall> FindHallBySpace(Guid spaceId)
        {
            var utcs = new UniTaskCompletionSource<Hall>();

            Hall[] halls = await GetHalls();

            Hall spaceHall = null;
            foreach (Hall hall in halls)
            {
                foreach (var hallSpace in hall.Spaces)
                {
                    if (hallSpace.Id == spaceId)
                    {
                        spaceHall = hall;
                    }
                }
            }

            utcs.TrySetResult(spaceHall);
            return await utcs.Task;
        }

        private async UniTask<HallResponse> RequestHalls()
        {
            await UniTask.WaitUntil(() => APIService.GetHallsUrl != string.Empty);
            await UniTask.WaitUntil(() => _clientData.AccessToken != string.Empty);

            if (_isRequestInProgress)
            {
                await UniTask.WaitWhile(() => _isRequestInProgress);
            }

            _isRequestInProgress = true;

            var utcs = new UniTaskCompletionSource<HallResponse>();

            string url = APIService.AddLanguageParameter(APIService.GetHallsUrl);
            HttpResponse<HallResponse> httpResponse = await WebRequestFunctions.GetWithDeserialization<HallResponse>
                (url, _clientData.AccessToken);

            bool success = httpResponse.ResponseCode == HttpResponse<Hall[]>.SuccessCode;
            if (success)
            {
                if (httpResponse.ResponseData == null)
                {
                    utcs.TrySetResult(new HallResponse());
                }
                else
                {
                    utcs.TrySetResult(httpResponse.ResponseData);
                }
            }
            else
            {
                Debug.LogWarning($"{nameof(RequestHalls)}. Cannot get Halls. " +
                                 $"Errorcode:{httpResponse.ResponseCode}");
                utcs.TrySetResult(new HallResponse());
            }

            _isRequestInProgress = false;

            return await utcs.Task;
        }
    }
}