using Assets.Scripts.API;
using Assets.Scripts.Space;
using Cysharp.Threading.Tasks;
using Infrastructure.AssetManagement;
using Infrastructure.Factory;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using Zenject;

public class LoadingSceneChanger : MonoBehaviour
{
    #region Inspector
    [SerializeField] private SceneChangerView _sceneChangerView;
    [SerializeField] private LoadSceneProgressBar _loadSceneProgressBar;
    
    [Space]
    [SerializeField] protected InputActionReference SelectActionR = null;
    [SerializeField] protected InputActionReference SelectActionL = null;
    #endregion

    private static ISceneFactory _sceneFactory;
    private LocalSpace _currentLocalSpace;
    private ISpaceService _spaceService;
    private AsyncOperationHandle<SceneInstance> operationHandler;
    private IApiService _apiService;

    [Inject]
    private void Construct(IApiService apiService,ISpaceService spaceService)
    {
        _apiService = apiService;
        _spaceService = spaceService;
    }

    private void OnDisable()
    {
        if(_sceneFactory != null)
            _sceneFactory.Cleanup();
        
    }

    private void OnSceneLoadComplete()
    {
        SelectActionR.action.Enable();
        SelectActionL.action.Enable();
        
        if(operationHandler.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.LogError(_currentLocalSpace.SpaceName);
            _sceneChangerView.ChangeSpace(_currentLocalSpace.SpaceType, _currentLocalSpace.Id.ToString(), _currentLocalSpace.SpaceName);
        }
    }
    
    public void LoadScene(SpaceType spaceType,string id = default)
    {
        SelectActionR.action.Disable();
        SelectActionL.action.Disable();

        var space = _spaceService.Get(spaceType,id);
        if (space == null) return;
        
        _currentLocalSpace = space;

        if (!string.IsNullOrEmpty(space.RemoteSceneName))
        {
            string catalogUrl = ResourcesLocation.GetRemoteScenePath(space.RemoteFolderName, space.RemoteCatalogName);
            _sceneFactory = new SceneFactory(new AddressableResourcesLocation(catalogUrl, LoadSceneAsync));
        }
        else
        {
            _sceneChangerView.ChangeSpace(space.SpaceType, id, space.SpaceName);
        }
    }

    private async void LoadSceneAsync() 
    {
        operationHandler = _sceneFactory.LoadSceneAsync(_currentLocalSpace.RemoteSceneName);
        operationHandler.Completed += _ => OnSceneLoadComplete();

        _loadSceneProgressBar.SetAsyncOperation(operationHandler);
        await operationHandler;
    }
}
