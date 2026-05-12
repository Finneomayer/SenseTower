using Assets.Scripts;
using Assets.Scripts.API;
using UnityEngine;
using Zenject;

public class UIInitialization : MonoBehaviour
{
    public MainMenuService MainMenuService;
    public UISwitcherService SwitcherService;
    public ApplicationRunner ApplicationRunner;

    private IApiService _apiService;
    
    [Inject]
    public void Construct(IApiService apiService)
    {
        _apiService = apiService;
    }

    private void Start()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        ////moved to EditorMovementSystem.cs
        //Cursor.visible = true;
        //Cursor.lockState = CursorLockMode.None;
#endif
        _apiService.FailServerInitialize += OnFailGetServerInfo;
        _apiService.ServerInitializedSuccess += OnGetServerInfo;
        ApplicationRunner._applicationBootstrapperInstance.ProfileChange += OnProfileChange;
    }

    private void OnDestroy()
    {
        ApplicationRunner._applicationBootstrapperInstance.ProfileChange -= OnProfileChange;
        _apiService.FailServerInitialize -= OnFailGetServerInfo;
        _apiService.ServerInitializedSuccess -= OnGetServerInfo;
    }

    private void OnProfileChange()
    {
        SwitcherService.SetButtonsInteractable(false);
    }

    private void OnFailGetServerInfo()
    {
        MainMenuService.OnFailServerInit();
    }
    
    private void OnGetServerInfo()
    {
        MainMenuService.OnSuccessLoadServerInfo();
    }
}
