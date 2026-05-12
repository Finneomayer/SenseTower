using Assets.Scripts.API;
using Assets.Scripts.Client;
using Assets.Scripts.Models;
using Assets.Scripts.TowerObjectsClass.Models;
using Assets.Scripts.TowerObjectsRevision.Interfaces;
using Assets.Scripts.TowerObjectsRevision.Models;
using Assets.Scripts.WebUtils;
using Cysharp.Threading.Tasks;
using Models;
using Newtonsoft.Json;
using Proyecto26;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.TowerObjectsRevision
{
    public class TowerObjectRevisionService : ITowerObjectsRevision
    {
        private IClientData _clientData;

        [Inject]
        private void Construct(IClientData clientData)
        {
            _clientData = clientData;
        }

        public async UniTask<string> SaveTowerObject(SaveTowerObjectRequestDTO towerObjectRequestDto)
        {
            var utcs = new UniTaskCompletionSource<string>();
            await UniTask.WaitUntil(() => APIService.GetTowerObjectsRevisionEndPoint != null);

            RequestHelper options = new RequestHelper();

            options.Uri = $"{APIService.GetTowerObjectsRevisionEndPoint}";
            options.Headers["Authorization"] = $"Bearer {_clientData.AccessToken}";
            options.BodyString = JsonConvert.SerializeObject(towerObjectRequestDto);

            var result = await WebRequestFunctions.PostWithDeserialization<ScResult<string>>(options);
            if (result.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode &&
                result.ResponseData.Error == null)
            {
                utcs.TrySetResult(result.ResponseData.Result);
            }
            else
            {
                utcs.TrySetResult("");
            }

            return await utcs.Task;
        }

        public async UniTask<TowerObjectRevisionDTO[]> GetAllTowerObjects(string id)
        {
            var utcs = new UniTaskCompletionSource<TowerObjectRevisionDTO[]>();

            await UniTask.WaitUntil(() => APIService.GetTowerObjectsRevisionEndPoint != null);

            string url = $"{APIService.GetTowerObjectsRevisionEndPoint}/{id}";

            HttpResponse<ScResult<TowerObjectRevisionDTO[]>> httpResponse =
                await WebRequestFunctions.GetWithDeserialization<ScResult<TowerObjectRevisionDTO[]>>
                    (url, _clientData.AccessToken);
            
            if (httpResponse.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode &&
                httpResponse.ResponseData.Error == null)
            {
                utcs.TrySetResult(httpResponse.ResponseData.Result);
            }
            else
            {
                utcs.TrySetResult(null);
            }

            return await utcs.Task;
        }
    }
}