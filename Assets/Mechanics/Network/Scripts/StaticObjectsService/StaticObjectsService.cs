using System;
using System.Collections.Generic;
using Assets.Scripts.API;
using Assets.Scripts.Client;
using Assets.Scripts.Data;
using Assets.Scripts.Models;
using Assets.Scripts.Server;
using Assets.Scripts.WebUtils;
using Cysharp.Threading.Tasks;
using Data;
using Mechanics.LoadSceneObjects.Models;
using Mechanics.VideoService.Models;
using Models;
using Newtonsoft.Json;
using Proyecto26;
using UnityEngine;
using Zenject;

namespace Mechanics.Network.Scripts.StaticObjectsService
{
    public class StaticObjectsService : IStaticObjectsService
    {
        private IApiService _clientApiService;
        private IServerApiData _serverApiData;
        private IClientData _clientData;

        [Inject]
        private void Construct(IApiService clientApiService, IServerApiData serverApiData, IClientData clientData)
        {
            _serverApiData = serverApiData;
            _clientData = clientData;
            _clientApiService = clientApiService;
        }

        public async UniTask<bool> SaveSceneStaticObjects(List<StaticObject> objects, string spaceId)
        {
            Debug.LogWarning("SAVE");

            UniTaskCompletionSource<bool> unts = new();

            RequestHelper options = new RequestHelper();
            string url = string.Empty;
#if UNITY_SERVER
            url = await GetStaticObjectsUrl();
            options.Headers["Authorization"] = $"Bearer {_serverApiData.AccessToken}";
#endif
#if !UNITY_SERVER
            url = APIService.GetSpaceStaticObjectsEndPoint;    
            options.Headers["Authorization"] = $"Bearer {_clientData.AccessToken}";
#endif
            options.Uri = $"{url}/{spaceId}";
            options.BodyString = JsonConvert.SerializeObject(objects);

            var result = await WebRequestFunctions.Post(options);
            unts.TrySetResult(result.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode);

            return await unts.Task;
        }

        //public async UniTask<List<StaticObject>> GetClientSceneStaticObjects(string spaceId)
        //{
        //    throw new System.NotImplementedException();

        //    //UniTaskCompletionSource<List<StaticObject>> unts = new();
        //    //string url =$"{APIService.GetSpaceStaticObjectsEndPoint}/{spaceId}?RemoteObjectType=2"; ; 

        //    //var result = await _clientApiService.GetWithAuthToken<ScResult<List<StaticObject>>>(url);
        //    //unts.TrySetResult(result.Result);

        //    //return await unts.Task;
        //}

        //public async UniTask<List<StaticObject>> GetServerSceneStaticObjects(string spaceId)
        //{
        //    Debug.Log("!!! Start getting static objects");

        //    UniTaskCompletionSource<List<StaticObject>> unts = new();
        //    List<StaticObject> staticObjects = new();

        //    RequestHelper options = new RequestHelper();
        //    string baseUrl = await GetStaticObjectsUrl();
        //    options.Uri = $"{baseUrl}/{spaceId}";
        //    options.Headers["Authorization"] = $"Bearer {_serverApiData.AccessToken}";

        //    var result = await WebRequestFunctions.GetWithDeserialization<ScResult<List<StaticObject>>>(options);
        //    if (result.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode)
        //    {
        //        if (result.ResponseData.Result != null) 
        //            staticObjects.AddRange(result.ResponseData.Result);
        //    }
        //    else
        //    {
        //        unts.TrySetResult(new List<StaticObject>());
        //    }
            
        //    options.Uri = $"{baseUrl}/{spaceId}?IsActive=false";

        //    var inActiveResult = await WebRequestFunctions.GetWithDeserialization<ScResult<List<StaticObject>>>(options);
        //    if (inActiveResult.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode)
        //    {
        //        if (inActiveResult.ResponseData.Result != null) 
        //            staticObjects.AddRange(inActiveResult.ResponseData.Result);
        //    }

        //    Debug.Log($"!!! End getting static objects count: {staticObjects.Count}");


        //    unts.TrySetResult(staticObjects);
            
        //    return await unts.Task;
        //}

        public async UniTask<List<StaticObject>> GetAllSceneStaticObjects(string spaceId)
        {
            List<StaticObject> staticObjects = new();
            UniTaskCompletionSource<List<StaticObject>> unts = new();
            string url = string.Empty;

#if UNITY_SERVER
            RequestHelper options = new RequestHelper();
            string baseUrl = await GetStaticObjectsUrl();
            options.Uri = $"{baseUrl}/{spaceId}";
            options.Headers["Authorization"] = $"Bearer {_serverApiData.AccessToken}";

            var result = await WebRequestFunctions.GetWithDeserialization<ScResult<List<StaticObject>>>(options);
            if (result.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode)
            {
                if (result.ResponseData.Result != null)
                    staticObjects.AddRange(result.ResponseData.Result);
            }
            else
            {
                unts.TrySetResult(new List<StaticObject>());
            }

            options.Uri = $"{baseUrl}/{spaceId}?IsActive=false";

            var inActiveResult = await WebRequestFunctions.GetWithDeserialization<ScResult<List<StaticObject>>>(options);
            if (inActiveResult.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode)
            {
                if (inActiveResult.ResponseData.Result != null)
                    staticObjects.AddRange(inActiveResult.ResponseData.Result);
            }

            unts.TrySetResult(staticObjects);

            return await unts.Task;
#endif
#if !UNITY_SERVER
            url = APIService.GetSpaceStaticObjectsEndPoint;      

            url = $"{url}/{spaceId}";
            var activeResult = await _clientApiService.GetWithAuthToken<ScResult<List<StaticObject>>>(url);
            foreach (StaticObject staticObject in activeResult.Result)
            {
                if (!string.IsNullOrEmpty(staticObject.ObjectKey) && staticObject.ObjectKey.Equals("VideoPlayer"))
                    staticObject.PrefabObjectType = Enumenators.PrefabObjectType.Video;
            }
            if (activeResult == null || activeResult.Result == null)
            {
                unts.TrySetResult(new List<StaticObject>());
            }
            else
            {
                staticObjects.AddRange(activeResult.Result);
            }
            
            var inActiveResult = await _clientApiService.GetWithAuthToken<ScResult<List<StaticObject>>>($"{url}?IsActive=false");

            if (inActiveResult == null || inActiveResult.Result == null)
            {
                unts.TrySetResult(new List<StaticObject>());
            }
            else
            {
                staticObjects.AddRange(inActiveResult.Result);
            }
            unts.TrySetResult(staticObjects);
            return await unts.Task;
#endif
        }

        private async UniTask<string> GetStaticObjectsUrl()
        {
            UniTaskCompletionSource<string> unts = new();
            await UniTask.WaitUntil(() => !string.IsNullOrEmpty(ServerApiService.GetSpaceStaticObjectsUrl));
            unts.TrySetResult(ServerApiService.GetSpaceStaticObjectsUrl);
            return await unts.Task;
        }
    }
}