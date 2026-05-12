using System;
using Assets.Scripts.API;
using Cysharp.Threading.Tasks;
using Infrastructure.AssetManagement;
using Infrastructure.Factory;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using UnityEngine;
using Zenject;
using Assets.Scripts.Hall;

namespace Assets.Scripts.Space
{
    public class SpaceManager : ISpaceManager
    {
        private ISpaceService _spaceService = new SpaceService(); // temporary initialization, must be removed
        private IRegistrationInSpacesService _registrationInSpacesService;
        public Action<SpaceType> RequestToChangeSpace { get; set; }
        public Action<bool> IsConnectedToServer { get; set; }
        private static ISceneFactory _sceneFactory;
        private IApiService _apiService;
        private IHallsService _hallService;

        [Inject]
        public void Construct(IApiService apiService,ISpaceService spaceService, IHallsService hallService,
            IRegistrationInSpacesService registrationInSpacesService)
        {
            _apiService = apiService;
            _apiService.ServerInitializedSuccess += ApiServiceOnServerInitializedSuccess;
            _spaceService = spaceService;
            _hallService = hallService;
            _registrationInSpacesService = registrationInSpacesService;
        }

        private void ApiServiceOnServerInitializedSuccess()
        {
            if(_sceneFactory == null)
                _sceneFactory = new SceneFactory(new AddressableResourcesLocation());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="key"></param>
        /// <param name="reload">needs to set non additive loading for correct restarting</param>
        public async void ChangeSpace(SpaceType type, string key, bool reload = false)
        {
            if (string.IsNullOrEmpty(key) && type == SpaceType.HallScene)
            {
                Guid hallId = await FindHallSpaceIdOfCurrentSpace();
                if (hallId != Guid.Empty)
                {
                    key = hallId.ToString();
                }
            }

            var space = _spaceService.Get(type, key);
            
            if (space == null)
            {
                Debug.Log("Scene not found " + type);
                space = _spaceService.Get(SpaceType.EnterScene, "");
            }

            var networkManager = UnityEngine.Object.FindObjectOfType<NetworkManager>();
            if (networkManager != null)
            {
                UnityEngine.Object.Destroy(networkManager);
            }
            
            if(space.SpaceType != SpaceType.EnterScene)
                await _registrationInSpacesService.Register(space.Id.ToString());
            CurrentTransitionTarget = space;
            
            if (!string.IsNullOrEmpty(CurrentTransitionTarget.RemoteSceneName))
            {
                await _sceneFactory.ActiveSceneAsync();

                //reloading scene!
                if (reload) await SceneManager.UnloadSceneAsync("InfrastructureScene");
                
                await SceneManager.LoadSceneAsync(CurrentTransitionTarget.SpaceType.ToString(), LoadSceneMode.Additive);
            }
            else
            {
                SceneManager.LoadSceneAsync(space.SceneName);
            }
        }

        private readonly string CurrentTransitionType = "CurrentSceneTransitionType";
        private readonly string CurrentTransitionId = "CurrentSceneTransitionId";

        public LocalSpace CurrentTransitionTarget
        { 
            get
            {
                if (!PlayerPrefs.HasKey(CurrentTransitionType))
                {
                    return null;
                }

                var type = PlayerPrefs.GetString(CurrentTransitionType);
                if (!Enum.TryParse(type, out SpaceType parsedType))
                {
                    Debug.Log("Parse error " + type);
                    return null;
                }
                string id = null;

                if (PlayerPrefs.HasKey(CurrentTransitionId))
                {
                    id = PlayerPrefs.GetString(CurrentTransitionId);
                }
                return _spaceService.Get(parsedType, id);
            }
            set
            {
                PlayerPrefs.SetString(CurrentTransitionType, value.SpaceType.ToString());
                PlayerPrefs.SetString(CurrentTransitionId, value.Id.ToString());
            }
        }

        public async UniTask<string> FindHallNameOfCurrentSpace()
        {
            string name = "";

            if (CurrentTransitionTarget == null)
            {
                return name;
            }

            Hall.Hall hall = await _hallService.FindHallBySpace(CurrentTransitionTarget.Id);
            if (hall != null)
            {
                name = hall.Space.SpaceName;
            }

            return name;
        }

        private async UniTask<Guid> FindHallSpaceIdOfCurrentSpace()
        {
            Guid hallId = Guid.Empty;

            if (CurrentTransitionTarget == null)
            {
                return hallId;
            }

            Hall.Hall hall = await _hallService.FindHallBySpace(CurrentTransitionTarget.Id);
            if (hall != null)
            {
                hallId = hall.Space.Id;
            }

            return hallId;
        }
    }
}