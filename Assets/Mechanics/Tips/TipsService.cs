using System.Collections.Generic;
using Assets.Mechanics.Tips.Model;
using Assets.Scripts.API;
using Assets.Scripts.Client;
using Assets.Scripts.Models;
using Assets.Scripts.WebUtils;
using Cysharp.Threading.Tasks;
using Models;
using Newtonsoft.Json;
using Proyecto26;
using UnityEngine;

namespace Assets.Mechanics.Tips
{
    public class TipsService : ITipsService
    {
        private readonly IClientData _clientData;

        public TipsService(IClientData clientData)
        {
            _clientData = clientData;
        }

        public async UniTask<string> GetTipsFromID(string tipsId)
        {
            var utcs = new UniTaskCompletionSource<string>();
            Debug.Log("send get tips from id" + tipsId);
            RequestHelper options = new RequestHelper();
            string url = $"{APIService.GetTipsEndPoint}/{tipsId}";
            options.Uri = $"{APIService.AddLanguageParameter(url)}";
            options.Headers["Authorization"] = $"Bearer {_clientData.AccessToken}";
            
            var result = await WebRequestFunctions.GetWithDeserialization<ScResult<string>>(options);
            if (result.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode &&
                result.ResponseData.Error == null)
            {
                utcs.TrySetResult(result.ResponseData.Result);
            }
            else
            {
				string resultInString = string.Empty;
				
                utcs.TrySetResult(resultInString);
            }

            return await utcs.Task;
        }

        public async UniTask<List<TipsItemDto>> GetTipsFromIDs(List<string> tipsIds)
        {
            var utcs = new UniTaskCompletionSource<List<TipsItemDto>>();

            if (tipsIds.Count == 0)
            {
                utcs.TrySetResult(new List<TipsItemDto>());
                return await utcs.Task;
            }

            RequestHelper options = new RequestHelper();
            string url = $"{APIService.GetTipsEndPoint}/byIds";
            options.Uri = $"{APIService.AddLanguageParameter(url)}";
            options.Headers["Authorization"] = $"Bearer {_clientData.AccessToken}";
            options.BodyString = JsonConvert.SerializeObject(tipsIds);
            
            var result = await WebRequestFunctions.PostWithDeserialization<ScResult<List<TipsItemDto>>>(options);
            if (result.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode &&
                result.ResponseData.Error == null)
            {
                utcs.TrySetResult(result.ResponseData.Result);
            }
            else
            {
                utcs.TrySetResult(result.ResponseData.Result);
            }

            return await utcs.Task;
        }
    }
}