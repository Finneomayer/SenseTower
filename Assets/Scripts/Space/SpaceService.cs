using System;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using API.Models;
using Assets.Scripts.API;
using Assets.Scripts.Client;
using UnityEngine;
using Zenject;
using Assets.Scripts.WebUtils;
using Assets.Scripts.Models;

namespace Assets.Scripts.Space
{
    public class SpaceService : ISpaceService
    {
        //private const string DefaultUnityServerIp = "51.250.50.39"; //prod server
        //private const string DefaultUnityServerIp = "51.250.90.38"; //dev-demo linux server
        //private const string DefaultUnityServerIp = "51.250.89.118"; //dev linux server
        //private const string DefaultUnityServerIp = "127.0.0.1"; //for local win server
        //private const int DefaultPort = 7790; //for local win server or dev-demo linux server
        
        //private const string DefaultUnityServerIp = "51.250.64.62"; //for dev-demo win server
        //private const int DefaultPort = 7778; //for dev-demo win server
        private IUsersInSpacesService _usersInSpacesService;
        private IApiService _apiService;
        private IClientData _clientData;
        
        private static Dictionary<string, LocalSpace> _spaces = new()
        {
            {SpaceType.EnterScene.ToString(), new LocalSpace
            {
                SpaceType = SpaceType.EnterScene,
                SpaceName = "в начало",
                SpaceConnectionInfo = null,
                SceneName = "TheEnterScene"
            }}
        };

        [Inject]
        public void Construct(IClientData clientData,IApiService apiService ,IUsersInSpacesService usersInSpacesService)
        {
            _clientData = clientData;
            _apiService = apiService;
            _usersInSpacesService = usersInSpacesService;

            _apiService.AuthSuccess += AsyncInitSpaces;
            _apiService.ServerInitializedSuccess += AsyncInitSpaces;
        }

        private async void AsyncInitSpaces()
        {
            if (_spaces.Count > 2) return;
            await InitSpaces();
        }
        
        private async UniTask InitSpaces()
        {
            await UniTask.WaitUntil(() => APIService.GetSpacesUrl != string.Empty);
            await UniTask.WaitUntil(() => _clientData != null);

            string url = APIService.AddLanguageParameter(APIService.GetSpacesUrl);

            List<LocalSpace> localSpaces = null;

            while (true)
            {
                HttpResponse<List<LocalSpace>> response =
                    await WebRequestFunctions.GetWithDeserialization<List<LocalSpace>>(url, _clientData.AccessToken);

                if (response != null)
                {
                    if (response.ResponseCode == HttpResponse<EmptyResponseData>.SuccessCode && response.ResponseData != null)
                    {
                        localSpaces = response.ResponseData;
                        break;
                    }

                    if (response.ResponseCode == HttpResponse<EmptyResponseData>.NotAuthorizedCode)
                    {
                        localSpaces = await _apiService.GetWithAuthToken<List<LocalSpace>>(url);
                        break;
                    }
                }

                await UniTask.Delay(1000);
            }

            if (localSpaces == null)
            {
                return;
            }

            foreach (var space in localSpaces)
            {
                string key = GetKey(space.SpaceType, space.Id.ToString());


                //if (space.Id == Guid.Parse("E81E71B9-E685-40C7-9D4E-49F9A328C1DB")) //Hall
                //{
                //    //space.SpaceConnectionInfo.Ip = "127.0.0.1";
                //    space.SpaceConnectionInfo.Port = 7799;
                //}
                //if (space.Id == Guid.Parse("17585806-0BE3-4BB2-8F0E-0DFFF489843C")) //Cinema
                //{
                //    //space.SpaceConnectionInfo.Ip = "127.0.0.1";
                //    space.SpaceConnectionInfo.Port = 7798;
                //}
                //if (space.Id == Guid.Parse("423A299C-7E8A-46B3-B672-70DCC40E32EA")) //Office1
                //{
                //    //space.SpaceConnectionInfo.Ip = "127.0.0.1";
                //    space.SpaceConnectionInfo.Port = 7800;
                //}

                //if (space.SpaceType == SpaceType.HallScene)
                //{
                //    space.SpaceConnectionInfo.Port = 7798;
                //    space.SpaceConnectionInfo.Ip = "127.0.0.1";
                //}

                _spaces[key] = space;
            }
        }

        public async void ReloadSpaces()
        {
            await InitSpaces();
        }

        public LocalSpace[] GetAllSpaces()
        {
            return _spaces.Values.ToArray();
        }

        public LocalSpace Get(SpaceType type, string id)
        {
            string key;
            if (!string.IsNullOrEmpty(id))
            {
                key = GetKey(type, id);
                if (_spaces.ContainsKey(key))
                {
                    return _spaces[key];
                }
            }

            key = GetKey(type, null);

            if (_spaces.ContainsKey(key))
            {
                return _spaces[key];
            }

            Debug.LogError($"wrong space key {key}. Probably {APIService.GetSpacesUrl} doesn't work");
            return null;
        }

        private static string GetKey(SpaceType type, string id)
            => type + (string.IsNullOrWhiteSpace(id) || id == Guid.Empty.ToString() ? string.Empty : ("_" + id.ToLowerInvariant()));
       
        public async UniTask<UsersInSpaceResponse> GetUsersInSpaces(int? getCount)
        {
            var utcs = new UniTaskCompletionSource<UsersInSpaceResponse>();
            
            UsersInSpaceResponse usersInSpaceResponse = await _usersInSpacesService.GetUsersInSpaces(getCount);
            
            utcs.TrySetResult(usersInSpaceResponse);

            return await utcs.Task;
        }
    }
}