using Assets.Scripts.API;
using Assets.Scripts.Client;
using Assets.Scripts.Models;
using Assets.Scripts.Server;
using Assets.Scripts.Space;
using Assets.Scripts.WebUtils;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Event
{
    public class TowerEventService : ITowerEventService
    {
        private IServerApiData _serverApiData;
        private IClientData _clientData;

        [Inject]
        public void Construct(IClientData clientData, IServerApiData serverData)
        {
            _serverApiData = serverData;
            _clientData = clientData;
        }

        public async UniTask<TowerEvent[]> GetEvents(int? getCount)
        {
            if (getCount.HasValue && getCount.Value <= 0)
            {
                var utcs = new UniTaskCompletionSource<TowerEvent[]>();
                utcs.TrySetResult(new TowerEvent[0]);
                return await utcs.Task;
            }
            return await RequestEvents(getCount);
        }

        public async UniTask<TowerEvent[]> GetEvents(TowerEventsFilter filter)
        {
            return await RequestEvents(filter);
        }

        public async UniTask<TowerEvent> GetEvent(Guid eventId)
        {
            var utcs = new UniTaskCompletionSource<TowerEvent>();
#if !UNITY_SERVER
            await UniTask.WaitUntil(() => APIService.GetTowerEventsUrl != string.Empty);
            string url = $"{APIService.GetTowerEventsUrl}/{eventId}";
            url = APIService.AddLanguageParameter(url);
            HttpResponse<TowerEvent> httpResponse = await WebRequestFunctions.GetWithDeserialization<TowerEvent>
                (url, _clientData.AccessToken);
#endif

#if UNITY_SERVER
            await UniTask.WaitUntil(() => ServerApiService.GetTowerEventsUrl != string.Empty);
            string url = $"{ServerApiService.GetTowerEventsUrl}/{eventId}";
            HttpResponse<TowerEvent> httpResponse = await WebRequestFunctions.GetWithDeserialization<TowerEvent>
               (url, _serverApiData.AccessToken);
#endif
            bool success = httpResponse.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode;
            if (success)
            {
                if (httpResponse.ResponseData == null)
                {
                    utcs.TrySetResult(null);
                }
                else
                {
                    utcs.TrySetResult(httpResponse.ResponseData);
                }
            }
            else
            {
                Debug.LogWarning($"{nameof(GetEvent)}. Cannot get Event {eventId}. " +
                    $"Errorcode:{httpResponse.ResponseCode}");
                utcs.TrySetResult(null);
            }

            return await utcs.Task;
        }

        private async UniTask<TowerEvent[]> RequestEvents(int? getCount)
        {
            await UniTask.WaitUntil(() => APIService.GetTowerEventsUrl != string.Empty);

            var utcs = new UniTaskCompletionSource<TowerEvent[]>();

            string url = $"{APIService.GetTowerEventsUrl}";

            if (getCount.HasValue)
            {
                url += $"?getCount={getCount.Value}";
            }
            url = APIService.AddLanguageParameter(url);
            HttpResponse<TowerEvent[]> httpResponse = await WebRequestFunctions.GetWithDeserialization<TowerEvent[]>
                (url, _clientData.AccessToken);

            bool success = httpResponse.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode;
            if (success)
            {
                if (httpResponse.ResponseData == null)
                {
                    utcs.TrySetResult(new TowerEvent[0]);
                }
                else
                {
                    utcs.TrySetResult(httpResponse.ResponseData);
                }
            }
            else
            {
                Debug.LogWarning($"{nameof(RequestEvents)}. Cannot get Events. " +
                    $"Errorcode:{httpResponse.ResponseCode}");
                utcs.TrySetResult(new TowerEvent[0]);
            }

            return await utcs.Task;
        }

        private async UniTask<TowerEvent[]> RequestEvents(TowerEventsFilter filter)
        {
            var utcs = new UniTaskCompletionSource<TowerEvent[]>();
#if !UNITY_SERVER
            await UniTask.WaitUntil(() => APIService.GetTowerEventsUrl != string.Empty);
            string url = $"{APIService.GetTowerEventsUrl}/list";
            url = APIService.AddLanguageParameter(url);
            HttpResponse<TowerEvent[]> httpResponse = await WebRequestFunctions.PostWithDeserialization<TowerEventsFilter, TowerEvent[]>
                (url, filter, _clientData.AccessToken);
#endif

#if UNITY_SERVER
            await UniTask.WaitUntil(() => ServerApiService.GetTowerEventsUrl != string.Empty);
            string url = $"{ServerApiService.GetTowerEventsUrl}/list";
            HttpResponse<TowerEvent[]> httpResponse = await WebRequestFunctions.PostWithDeserialization<TowerEventsFilter, TowerEvent[]>
                (url, filter, _serverApiData.AccessToken);
#endif
            bool success = httpResponse.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode;
            if (success)
            {
                if (httpResponse.ResponseData == null)
                {
                    utcs.TrySetResult(new TowerEvent[0]);
                }
                else
                {
                    utcs.TrySetResult(httpResponse.ResponseData);
                }
            }
            else
            {
                Debug.LogWarning($"{nameof(RequestEvents)}. Cannot get Events. " +
                    $"Errorcode:{httpResponse.ResponseCode}");
                utcs.TrySetResult(new TowerEvent[0]);
            }

            return await utcs.Task;
        }
    }
}