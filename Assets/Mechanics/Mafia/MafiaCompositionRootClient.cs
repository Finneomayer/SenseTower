using System;
using System.Linq;
using Assets.Mechanics.Mafia.Table;
using Assets.Mechanics.Mafia.UI;
using Assets.Scripts.Client;
using Assets.Scripts.Player;
using Assets.Scripts.Shared;
using Assets.Scripts.Zones;
using Cysharp.Threading.Tasks;
using Data;
using Mechanics.LoadSceneObjects;
using UnityEngine;

namespace Assets.Mechanics.Mafia
{

    public class MafiaCompositionRootClient : MonoBehaviour
    {
        [SerializeField]
        private TableExpander Table;
        [SerializeField]
        private SimpleInfoPanel NoTicketInfoPanel;
        [SerializeField]
        private SimpleInfoPanel GameInProgressInfoPanel;
        [SerializeField]
        private MafiaAdminPanelViewPanel MafiaAdminPanelViewPanel;
        [SerializeField] 
        private MafiaWinnerCup _mafiaWinnerCup;
        [SerializeField]
        private MafiaGameStatePlayerUI MafiaGameStatePlayerUI;
        [SerializeField]
        private MafiaNetworkBehaviour MafiaNetworkBehaviour;
        [SerializeField]
        private PlayerSeatsController PlayerSeatsController;
        [SerializeField]
        private MafiaPlayersAudioSwitcher MafiaPlayersAudioSwitcher;
        [SerializeField]
        private MafiaNightView _mafiaNight;
        [SerializeField]
        private GameState CurrentGameState;
        [Space] 
        [SerializeField] private DiscoveryServiceStaticData _contour; //to disable ticket check on dev

        private MafiaEventMediatorClient _eventMediator = new();
        private MafiaPlayerConditionController _mafiaPlayerConditionController = new();
        private string _localUserId;
        private ZonesModel _zonesModel;
        private PlayerLogic _localPlayer;
        private Scripts.Shared.NetworkPlayer _localNetworkPlayer;
        private IClientData _clientData;
        private IMafiaGameService _mafiaGameService;
        private MafiaPlayerRole[] _currentPreset;
        
        private MafiaLocalizationResultDto _localizationData;
        private MafiaTicketChecker _mafiaTicketChecker;
        private bool _initialized;
        private bool _isGameActiveForLocalUser;

        private void Awake()
        {
            //OnNewClientGameStateReceived(CurrentGameState);
            CurrentGameState = null;
            NoTicketInfoPanel.SetActivePanel(false);
            GameInProgressInfoPanel.SetActivePanel(false);

            _clientData = null;
            _zonesModel = FindObjectOfType<ZonesModel>();

            CommonDIInstaller commonDIInstaller = FindObjectOfType<CommonDIInstaller>();
            if (commonDIInstaller != null)
            {
                _clientData = commonDIInstaller.Resolve<IClientData>();
                _mafiaGameService = commonDIInstaller.Resolve<IMafiaGameService>();
            }

            _mafiaTicketChecker = new();
        }

        private void OnDestroy()
        {
            _mafiaTicketChecker.StopChecking();
        }

        private void OnEnable()
        {
            _eventMediator.StartGameRequested += OnStartGameRequested;
            _eventMediator.CompleteGameRequested += OnCompleteGameRequested;
            _eventMediator.NextGameStageRequested += OnNextGameStageRequested;
            _eventMediator.PlayerStatesChanged += OnPlayerStatesChanged;
            _eventMediator.NewClientGameStateReceived += OnNewClientGameStateReceived;
            _eventMediator.SelectPlayerRequested += OnSelectPlayerRequested;
            _eventMediator.PlayerRolePresetRequested += _eventMediator_PlayerRolePresetRequested;
            _eventMediator.OnUpdatePreset += _eventMediator_OnUpdatePreset;
            _eventMediator.PlayersSoundChanged += OnPlayersSoundChanged;
            _eventMediator.RestoreGameForUserRequested += OnRestoreGameForUserRequested;
            _eventMediator.KickUserRequested += OnKickUserRequested;
            _eventMediator.AutoKickDataRefreshRequested += OnAutoKickDataRefreshRequested;

            _mafiaTicketChecker.Updated += RefreshTicketDependingElements;
        }

        private void OnDisable()
        {
            _eventMediator.StartGameRequested -= OnStartGameRequested;
            _eventMediator.CompleteGameRequested -= OnCompleteGameRequested;
            _eventMediator.NextGameStageRequested -= OnNextGameStageRequested;
            _eventMediator.PlayerStatesChanged -= OnPlayerStatesChanged;
            _eventMediator.NewClientGameStateReceived -= OnNewClientGameStateReceived;
            _eventMediator.SelectPlayerRequested -= OnSelectPlayerRequested;
            _eventMediator.PlayerRolePresetRequested -= _eventMediator_PlayerRolePresetRequested;
            _eventMediator.OnUpdatePreset -= _eventMediator_OnUpdatePreset;
            _eventMediator.PlayersSoundChanged -= OnPlayersSoundChanged;
            _eventMediator.RestoreGameForUserRequested -= OnRestoreGameForUserRequested;
            _eventMediator.KickUserRequested -= OnKickUserRequested;
            _eventMediator.AutoKickDataRefreshRequested -= OnAutoKickDataRefreshRequested;

            _mafiaTicketChecker.Updated -= RefreshTicketDependingElements;
        }

        private void Start()
        {
            InitAsync().Forget();
        }

        private async UniTask InitAsync()
        {
            if (_mafiaGameService == null || _clientData == null || _clientData.UserId == null)
            {
                return;
            }

            _localUserId = _clientData.UserId.Value.ToString();

            _mafiaGameService.InitAsClient(_clientData);

            _localizationData = await _mafiaGameService.GetLocalizationContentClient();

            CompositionRootNetworkScene compositionRootNetworkScene = FindObjectOfType<CompositionRootNetworkScene>();
            if (compositionRootNetworkScene != null)
            {
                _localPlayer = await compositionRootNetworkScene.GetLocalPlayerAsync();
                _localNetworkPlayer = _localPlayer.gameObject.GetComponent<Scripts.Shared.NetworkPlayer>();
            }
            _mafiaPlayerConditionController.Init(_localNetworkPlayer, _localUserId);

            MafiaNetworkBehaviour.InitClient(_eventMediator, _localUserId);
            MafiaPlayersAudioSwitcher.InitClient(_eventMediator, _localUserId);
            MafiaAdminPanelViewPanel.Init(_eventMediator, _localizationData);
            MafiaGameStatePlayerUI.Init(_clientData, _mafiaNight);
            _mafiaNight.Init(_localizationData);
            PlayerSeatsController.Init(_eventMediator, _localizationData, _clientData, Table);

            await UniTask.WaitUntil(() => Table.TableData != null);

            _mafiaTicketChecker.Init(_mafiaGameService, Table.TableData.TableId, _contour.Assembly.AssemblyType == Enumenators.Server.Profile.Develop);
            _mafiaTicketChecker.StartChecking();

            _initialized = true;
        }

        private async void OnAutoKickDataRefreshRequested(MafiaPlayersKickSerializedData playersKickData)
        {
            await UniTask.WaitUntil(() => _initialized);
            PlayerSeatsController.SetPlayersKickData(playersKickData);
        }

        private async void OnKickUserRequested(Guid userGuid)
        {
            await UniTask.WaitUntil(() => _initialized);
            MafiaNetworkBehaviour.KickUserClient(userGuid);
        }

        private async void OnStartGameRequested()
        {
            await UniTask.WaitUntil(() => _initialized);
            MafiaNetworkBehaviour.StartGameClient(_currentPreset);
        }

        private async void OnCompleteGameRequested()
        {
            await UniTask.WaitUntil(() => _initialized);
            MafiaNetworkBehaviour.CompleteGameClient();
        }

        private async void OnNextGameStageRequested()
        {
            await UniTask.WaitUntil(() => _initialized);
            MafiaNetworkBehaviour.SetNextGameStageClient();
        }

        private async void OnRestoreGameForUserRequested()
        {
            await UniTask.WaitUntil(() => _initialized);
            RestoreGameForUser().Forget();
        }

        private async void OnNewClientGameStateReceived(GameState newGameState)
        {
            await UniTask.WaitUntil(() => _initialized);

            _mafiaTicketChecker.StopChecking();

            CurrentGameState = newGameState;
            GameState gameStateForLocalUser = newGameState;

            if (CurrentGameState != null)
            {
                Guid localUserGuid = Guid.Parse(_localUserId);
                var currentPlayerState = CurrentGameState.PlayerStates.FirstOrDefault(x => x.PlayerId == localUserGuid);
                if (currentPlayerState == null)
                {
                    gameStateForLocalUser = null;
                }
                else
                {
                    if (Table.TryGetPlaceByChairIndex(currentPlayerState.ChairIndexInTable, out Place playerPlace))
                    {
                        if (!playerPlace.IsOccupiedByLocalUser)
                        {
                            playerPlace.LeaveAndOccupyPlace();
                        }
                    }
                    else
                    {
                        Debug.LogError($"Cannot find place by chairIndex {currentPlayerState.ChairIndexInTable}");
                    }
                }
            }

            _isGameActiveForLocalUser = gameStateForLocalUser != null;

            await SetSeats(gameStateForLocalUser);
            MafiaAdminPanelViewPanel.SetGameState(gameStateForLocalUser);
            MafiaGameStatePlayerUI.SetGameState(gameStateForLocalUser);
            _mafiaWinnerCup.SetGameState(gameStateForLocalUser);
            MafiaPlayersAudioSwitcher.SetGameState(gameStateForLocalUser);
            _mafiaPlayerConditionController.SetGameState(gameStateForLocalUser);

            if (_zonesModel != null)
            {
                if (gameStateForLocalUser != null)
                {
                    //Blocks everything but only for gamers. For user not gaming at that moment place activation logic is at TableExpander.cs
                    if (gameStateForLocalUser.TryGetPlayerStateByUserId(_localUserId, out _))
                    {
                        _zonesModel.SetTeleportingAllowed(false, allTeleports: true);
                    }
                }
                else
                {
                    _zonesModel.SetTeleportingAllowed(true, allTeleports: true);
                    //MafiaCompositionRootClient switch ALL teleports at the scene,
                    // it's too mush, cause we don't need being enabled places of other table variants
                    Table.BlockUnnecessaryTeleportsAfterSwitchOnAll();
                    //Update block for NON players at the table
                    Table.UpdateBlockForNonPlayers();
                }
            }

            RefreshTicketDependingElements();
            if (CurrentGameState == null)
            {
                _mafiaTicketChecker.StartChecking();
                GameInProgressInfoPanel.SetActivePanel(false);
            }
            else
            {
                if (Table.TableData.TryGetGameInProgressInfoPanelPositionAndRotation(out Vector3 position, 
                    out Quaternion rotation))
                {
                    GameInProgressInfoPanel.transform.SetPositionAndRotation(position, rotation);
                    GameInProgressInfoPanel.SetActivePanel(true);
                }
                else
                {
                    GameInProgressInfoPanel.SetActivePanel(false);
                }
            }
        }

        /// <summary>
        /// Is using when player is selected or deselected when voting 
        /// </summary>
        private async void OnPlayerStatesChanged()
        {
            await UniTask.WaitUntil(() => _initialized);
            SetSeats(_isGameActiveForLocalUser ? CurrentGameState : null).Forget();
        }

        private async void OnSelectPlayerRequested(int seatNumber)
        {
            await UniTask.WaitUntil(() => _initialized);
            MafiaNetworkBehaviour.SelectPlayerClient(seatNumber);
        }

        private async UniTask RestoreGameForUser()
        {
            await UniTask.WaitUntil(() => _initialized);
            await MafiaPlayersAudioSwitcher.ConnectToGameAudioChannelClient();
            MafiaNetworkBehaviour.RestoreGameForUserClient();
        }

        /// <summary>
        /// Is using when state updates or player is selected or deselected when voting 
        /// </summary>
        private async UniTask SetSeats(GameState gameState)
        {
            await UniTask.WaitUntil(() => _initialized);
            PlayerSeatsController.SetState(gameState);
        }

        private async void _eventMediator_PlayerRolePresetRequested(int count)
        {
            await UniTask.WaitUntil(() => _initialized);
            GetPlayerRoles(count);
        }

        private async void OnPlayersSoundChanged()
        {
            await UniTask.WaitUntil(() => _initialized);
            MafiaGameStatePlayerUI.RefreshByUsersSound(MafiaPlayersAudioSwitcher.LocalPlayerListeners,
                MafiaPlayersAudioSwitcher.LocalPlayerTalkers);
        }

        private async void GetPlayerRoles(int count)
        {
            await UniTask.WaitUntil(() => _initialized);
            MafiaPlayerRole[] preset = await _mafiaGameService.GetPlayerRolesPresetClient(count);

            if (preset != null && preset.Length > 0)
                MafiaAdminPanelViewPanel.ShowPreset(preset);
        }

        private void RefreshTicketDependingElements()
        {
            if (CurrentGameState != null)
            {
                Table.SetOccupyingAvailable(false);
                NoTicketInfoPanel.SetActivePanel(false);
                return;
            }

            if (_mafiaTicketChecker.IsTicketAvailable)
            {
                Table.SetOccupyingAvailable(true);
                NoTicketInfoPanel.SetActivePanel(false);
            }
            else
            {
                Table.KickLocalClient(setPositionToZero: false);
                Table.SetOccupyingAvailable(false);
                NoTicketInfoPanel.SetActivePanel(true);
            }
        }

        private void _eventMediator_OnUpdatePreset(MafiaPlayerRole[] preset)
        {
            if (preset != null && preset.Length > 0)
            {
                _currentPreset = new MafiaPlayerRole[preset.Length];

                for (int i = 0; i < preset.Length; i++)
                {
                    _currentPreset[i] = preset[i];
                }
            }
        }
    }
}