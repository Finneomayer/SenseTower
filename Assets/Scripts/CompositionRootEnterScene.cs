using Assets.Scripts.API;
using Assets.Scripts.Client;
using Assets.Scripts.Player;
using Assets.Scripts.Zones;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Assets.Scripts
{
    public class CompositionRootEnterScene : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField] private PlayerLogic _playerLogic;

        [Space]
        [Header("Views")]
        [SerializeField] private UserInterfaceView _userInterfaceView;

        [SerializeField] private SceneChangerView _sceneChangerView;
        [SerializeField] private LoadingSceneChanger _loadingSceneChangerView;

        [SerializeField] private OnPlayerUI _onPlayerUi;

        [Inject] private IApiService _apiService;
        [Inject] private IClientData _clientData;
        private UserInterfaceLogic _userInterfaceLogic;

        private void Awake()
        {
            _userInterfaceLogic = new UserInterfaceLogic();
        }

        private void Start()
        {
            _userInterfaceLogic.Init(_apiService, _userInterfaceView);

            _playerLogic.Init(_userInterfaceLogic, _sceneChangerView, _onPlayerUi);

            _sceneChangerView.Init(_onPlayerUi);

            CheckUserToken().Forget();
        }

        public LoadingSceneChanger LoadingSceneChangerView {
            get => _loadingSceneChangerView;
            private set => _loadingSceneChangerView = value;
        }

        private async UniTask<bool> CheckUserToken()
        {
            var utcs = new UniTaskCompletionSource<bool>();

            bool result = await _apiService.RefreshToken();
            if (!result)
            {
                utcs.TrySetResult(false);
                _clientData.DeleteAllData();
            }

            return await utcs.Task;
        }
    }
}
