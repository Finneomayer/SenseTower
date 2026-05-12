using Assets.Scripts.API;
using Assets.Scripts.Models;
using Assets.Scripts.WebUtils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.News
{
    public class TowerNewsService : ITowerNewsService
    {
        public async UniTask<TowerNews[]> GetNews(int? getCount)
        {
            if (!getCount.HasValue || getCount.Value <= 0)
            {
                var utcs = new UniTaskCompletionSource<TowerNews[]>();
                utcs.TrySetResult(new TowerNews[0]);
                return await utcs.Task;
            }
            return await RequestNews(getCount);
        }

        private async UniTask<TowerNews[]> RequestNews(int? getCount)
        {
            await UniTask.WaitUntil(() => APIService.GetTowerNewsUrl != string.Empty);

            var utcs = new UniTaskCompletionSource<TowerNews[]>();

            string getCountFormValue = getCount.HasValue ? getCount.Value.ToString() : null;

            WWWForm myPlaceUpdateData = new();
            myPlaceUpdateData.AddField("getCount", getCountFormValue);

            string url = APIService.AddLanguageParameter(APIService.GetTowerNewsUrl);
            HttpResponse<TowerNews[]> httpResponse = await WebRequestFunctions.GetWithDeserialization<TowerNews[]>
                (url, myPlaceUpdateData);

            bool success = httpResponse.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode;
            if (success)
            {
                if (httpResponse.ResponseData == null)
                {
                    utcs.TrySetResult(new TowerNews[0]);
                }
                else
                {
                    utcs.TrySetResult(httpResponse.ResponseData);
                }
            }
            else
            {
                Debug.LogWarning($"{nameof(RequestNews)}. Cannot get News. " +
                    $"Errorcode:{httpResponse.ResponseCode}");
                utcs.TrySetResult(new TowerNews[0]);
            }
          
            return await utcs.Task;
        }
    }
}