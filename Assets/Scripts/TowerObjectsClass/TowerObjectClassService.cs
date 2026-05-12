using Assets.Scripts.API;
using Assets.Scripts.Client;
using Assets.Scripts.Models;
using Assets.Scripts.TowerObjectsClass.Models;
using Assets.Scripts.WebUtils;
using Cysharp.Threading.Tasks;
using Models;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.TowerObjectsClass
{
    public class TowerObjectClassService : ITowerObjectsClassService
    {
        private IClientData _clientData;

        [Inject]
        private void Construct(IClientData clientData)
        {
            _clientData = clientData;
        }

        public async UniTask<TowerObjectClass[]> GetAllTowerObjectClass()
        {
            var utcs = new UniTaskCompletionSource<TowerObjectClass[]>();

            await UniTask.WaitUntil(() => APIService.GetTowerObjectsClassEndPoint != null);

            string url = $"{APIService.GetTowerObjectsClassEndPoint}";

            HttpResponse<ScResult<TowerObjectClass[]>> httpResponse =
                await WebRequestFunctions.GetWithDeserialization<ScResult<TowerObjectClass[]>>
                    (url, _clientData.AccessToken);
            if (httpResponse.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode && httpResponse.ResponseData.Error == null)
            {
                utcs.TrySetResult(httpResponse.ResponseData.Result);
            }
            else
            {
                utcs.TrySetResult(new TowerObjectClass[1]);
            }

            return await utcs.Task;
        }
    }
}