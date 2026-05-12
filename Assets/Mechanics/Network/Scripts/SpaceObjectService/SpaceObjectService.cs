using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.API;
using Assets.Scripts.Client;
using Assets.Scripts.Hall;
using Assets.Scripts.Server;
using Assets.Scripts.WebUtils;
using Cysharp.Threading.Tasks;
using Mechanics.LoadSceneObjects.Models;
using Mechanics.VideoService.Models;
using Models;
using Newtonsoft.Json;
using Proyecto26;
using UnityEngine;
using Zenject;

namespace Mechanics.Network.Scripts.SpaceObjectsService
{
    [Serializable]
    internal class SpaceResponse
    {
        public Space Result { get; set; }
    }

    public class SpaceObjectService : ISpaceObjectService
    {
        private IApiService _clientApiService;
        private IServerApiData _serverApiData;
        private IClientData _clientData;
        private int _debugCounter = 0;

        [Inject]
        private void Construct(IApiService clientApiService, IServerApiData serverApiData, IClientData clientData)
        {
            _serverApiData = serverApiData;
            _clientData = clientData;
            _clientApiService = clientApiService;
        }

        /// <summary>
        /// Get Space with all data for client & server
        /// </summary>
        /// <param name="spaceId"></param>
        /// <returns></returns>
        public async UniTask<Space> GetSpaceWithAllObjects(string spaceId)
        {
            Debug.Log($"GetSpaceWithAllObjects {spaceId}");
            //Space space = new();
            var unts = new UniTaskCompletionSource<Space>();
            string url = string.Empty;

            url = await GetEndpoint();

            RequestHelper options = new RequestHelper();
            options.Uri = url.Replace("{spaceId}", spaceId);
            Debug.Log($"options.Uri {options.Uri}");
#if !UNITY_SERVER
            options.Headers["Authorization"] = $"Bearer {_clientData.AccessToken}";
#endif
#if UNITY_SERVER
            options.Headers["Authorization"] = $"Bearer {_serverApiData.AccessToken}";
#endif
            var result = await WebRequestFunctions.Get(options);
            Debug.Log(result.ResponseCode);
            if (result.ResponseData != null)
            {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new SpaceObjectConverter());
                settings.Converters.Add(new TowerObjectConverter());

                if (_debugCounter == 0)
                {
                    Debug.Log(result.ResponseData);
                    _debugCounter = 1;
                }
                var data = JsonConvert.DeserializeObject<ScResult<Space>>(result.ResponseData, settings);
                unts.TrySetResult(data.Result);
            }

            return await unts.Task;
        }

        private async UniTask<string> GetEndpoint()
        {
            UniTaskCompletionSource<string> unts = new();
#if !UNITY_SERVER
            unts.TrySetResult(APIService.GetSpaceObjectsNewEndPoint);
            return await unts.Task;
#endif
#if UNITY_SERVER
            await UniTask.WaitUntil(() => !string.IsNullOrEmpty(ServerApiService.GetSpaceObjectsNewEndPoint));
            unts.TrySetResult(ServerApiService.GetSpaceObjectsNewEndPoint);
            return await unts.Task;
#endif
        }
    }
}