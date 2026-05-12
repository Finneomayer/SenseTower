using Assets.Scripts.API;
using Assets.Scripts.Client;
using Assets.Scripts.Models;
using Assets.Scripts.WebUtils;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Cinema
{
    public class CinemaService : ICinemaService
    {
        private IServerApiService _serverApiService;
        private IClientData _clientData;

        [Inject]
        private void Construct(IServerApiService serverApiService, IClientData clientData) 
        {
            _serverApiService = serverApiService;
            _clientData = clientData;
        }

        public async UniTask<Cinema> GetById(string id)
        {
            Cinema cinema = await _serverApiService.GetCinemaById(id);
            return cinema;
        }

        public async UniTask<Cinema[]> GetCinemas()
        {
            var utcs = new UniTaskCompletionSource<Cinema[]>();

            await UniTask.WaitUntil(() => APIService.GetCinemasUrl != string.Empty);

            string url = APIService.AddLanguageParameter(APIService.GetCinemasUrl);
            HttpResponse<Cinema[]> httpResponse = await WebRequestFunctions.GetWithDeserialization<Cinema[]>
                (url, _clientData.AccessToken);

            if (httpResponse.ResponseCode != HttpResponse<EmptyResponseData>.SuccessCode)
            {
                Debug.LogWarning($"{nameof(GetCinemas)}. Cannot get Cinemas. " +
                    $"Errorcode:{httpResponse.ResponseCode}");
            }
            utcs.TrySetResult(httpResponse.ResponseData ?? (new Cinema[0]));

            return await utcs.Task;
        }

        public async UniTask<Cinema> GetBySpaceId(string spaceId)
        {
            var utcs = new UniTaskCompletionSource<Cinema>();

            await UniTask.WaitUntil(() => APIService.GetCinemasUrl != string.Empty);

            string url = $"{APIService.GetCinemasUrl}/byspace/{spaceId}";

            url = APIService.AddLanguageParameter(url);

            HttpResponse<Cinema> httpResponse = await WebRequestFunctions.GetWithDeserialization<Cinema>
                (url, _clientData.AccessToken);

            if (httpResponse.ResponseCode != HttpResponse<EmptyResponseData>.SuccessCode)
            {
                Debug.LogWarning($"{nameof(GetBySpaceId)}. Cannot get Cinema by space id. " +
                    $"Errorcode:{httpResponse.ResponseCode}");
            }
            utcs.TrySetResult(httpResponse.ResponseData);

            return await utcs.Task;
        }
    }

}
