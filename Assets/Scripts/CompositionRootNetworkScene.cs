using System;
using Assets.Mechanics.Tips;
using Assets.Scripts.Player;
using Assets.Scripts.Server;
using Assets.Scripts.Space;
using Assets.Scripts.Zones;
using Client;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using NetworkPlayer = Assets.Scripts.Shared.NetworkPlayer;

public struct LightingSettings
{
    public Color AmbientLight;
}
public class CompositionRootNetworkScene : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerLogic _playerLogic;
    [SerializeField] private ClientVerification _clientVerification;
    [SerializeField] private TipsViewer[] _tipsViewers;
    [Header("Server")]
    [SerializeField] private ServerVerification _serverVerification;
    
    [Space]
    [Header("Views")]
    [SerializeField] private SceneChangerView _sceneChangerView;

    [SerializeField] private OnPlayerUI _onPlayerUi;
    [SerializeField] private UIBinder _playerSettingsMenu;

    [Space]
    [Header("Zones")]
    [SerializeField] private ZonesModel _zonesmodel;

    [Space] [Header("Player spawn points near lift")] 
    [SerializeField] private Transform[] _firstFloorPoints = null;
    [SerializeField] private Transform[] _secondFloorPoints = null;

    public LightingSettings InitialLightingSettings { get; private set; }
    public SceneChangerView SceneChanger => _sceneChangerView;
    public Transform[] FirstFloorPoints => _firstFloorPoints;
    public Transform[] SecondFloorPoints => _secondFloorPoints;
    public bool Initialized => _playerLogic != null;

    private ISpaceModeData _spaceModeData;
    private ITipsSceneRepository _tipsSceneRepository;
    private ITipsSceneContext _tipsSceneContext;

    [Inject]
    private void Construct(ITipsSceneContext tipsSceneContext,ITipsSceneRepository tipsSceneRepository, ISpaceModeData spaceModeData)
    {
        _tipsSceneContext = tipsSceneContext;
        _tipsSceneRepository = tipsSceneRepository;
        _spaceModeData = spaceModeData;
    }

    private void Awake()
    {
        InitialLightingSettings = new()
        {
            AmbientLight = RenderSettings.ambientLight
        };

        _sceneChangerView.Init(_onPlayerUi);
    }

    private void OnDisable()
    {
        _tipsSceneContext.CleanUp();

        RenderSettings.ambientLight = InitialLightingSettings.AmbientLight;
    }

    public void InitZones(ulong id, PlayerLogic playerLogic)
    {
        if (_zonesmodel == null) return;

        _zonesmodel.Init(id, playerLogic, _onPlayerUi);
    }

    /// <summary>
    /// On Network Spawn of client OWNER
    /// </summary>
    /// <param name="playerLogic"></param>
    public async UniTask InitPlayer(PlayerLogic playerLogic, NetworkPlayer networkPlayer)
    {
        _playerLogic = playerLogic;

        _playerLogic.Init(_sceneChangerView, _onPlayerUi);

        _playerLogic.SetPlayerUI(_playerSettingsMenu);
        _playerLogic.SetPlayerEmoji(_playerSettingsMenu.Switcher.EmojiSwitcherObject);
        _playerLogic.SetTipsService(_playerSettingsMenu.Switcher,_spaceModeData,_tipsSceneRepository);
        _playerLogic.CorrectPosition();
        for (int i = 0; i < _tipsViewers.Length; i++)
        {
            _tipsViewers[i].Construct(_tipsSceneRepository,_spaceModeData,_playerSettingsMenu.Switcher);
        }
        await UniTask.Yield(PlayerLoopTiming.Update);
        await UniTask.Yield(PlayerLoopTiming.FixedUpdate);

        await _sceneChangerView.InitScene(networkPlayer);

        InitValidation();
    }

    public void InitValidation()
    {
        _clientVerification = FindObjectOfType<ClientVerification>();
        if (_clientVerification == null)
        {
            Debug.LogWarning("ClientVerification object == null");
            return;
        }
        _clientVerification.Initialize(_sceneChangerView,_serverVerification);
    }

    public void InitPresentationNetwork(Presentation presentation)
    {
        presentation.SetLaserActivator(_playerLogic.PresentationLaserActivator);
    }

    public void InitPresentationNetwork(PresentationPad presentation)
    {
        presentation.SetLaserActivator(_playerLogic.PresentationLaserActivator);
    }

    public async UniTask<PlayerLogic> GetLocalPlayerAsync()
    {
        var utcs = new UniTaskCompletionSource<PlayerLogic>();
        await UniTask.WaitUntil(() => _playerLogic != null);
        utcs.TrySetResult(_playerLogic);
        return await utcs.Task;
    }
}
