using Assets.Mechanics.Mafia.Table;
using Assets.Scripts.Audio;
using Assets.Scripts.Data;
using Assets.Scripts.Server;
using Assets.Scripts.Shared;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Assets.Scripts.Server.ServerVerification;

namespace Assets.Mechanics.Mafia
{
    public class MafiaCompositionRootServer : MonoBehaviour
    {
        [SerializeField]
        private TableExpander Table;
        [SerializeField]
        private MafiaNetworkBehaviour MafiaNetworkBehaviour;
        [SerializeField]
        private MafiaPlayersAudioSwitcher MafiaPlayersAudioSwitcher;
        [SerializeField]
        private GameState DebugGameState;

        private IMafiaGameService _mafiaGameService;
        private IServerApiData _serverApiData;
        private ServerVerification _serverVerification;
        private MafiaEventMediatorServer _eventMediator = new();

        private GameState _gameState;
        private Dictionary<string, PlayerSeatInfo> _clientSeats = new Dictionary<string, PlayerSeatInfo>();

        private void Awake()
        {
#if !UNITY_SERVER
            enabled = false;
            return;
#else
            CommonDIInstaller commonDIInstaller = FindObjectOfType<CommonDIInstaller>();
            if (commonDIInstaller != null)
            {
                _mafiaGameService = commonDIInstaller.Resolve<IMafiaGameService>();
                _serverApiData = commonDIInstaller.Resolve<IServerApiData>();
            }

            _serverVerification = FindObjectOfType<ServerVerification>();
#endif
        }

        private void Start()
        {
            if (_mafiaGameService == null)
            {
                return;
            }

            MafiaPlayersAudioSwitcher.Init(_serverVerification, Table.TableData.TableNumber);
            MafiaNetworkBehaviour.Init(_eventMediator, _serverVerification);
            _mafiaGameService.Init(_serverApiData);
        }

        private void OnEnable()
        {
            _eventMediator.StartGameRequested += OnStartGameRequested;
            _eventMediator.NextGameStageRequested += OnNextStageRequested;
            _eventMediator.CompleteGameRequested += OnCompleteGameRequested;
            _eventMediator.PlayerReconnectToGameRequested += OnPlayerReconnectToGameRequested;
            _eventMediator.KickUserRequested += OnKickUserRequested;
        }

        private void OnDisable()
        {
            _eventMediator.StartGameRequested -= OnStartGameRequested;
            _eventMediator.NextGameStageRequested -= OnNextStageRequested;
            _eventMediator.CompleteGameRequested -= OnCompleteGameRequested;
            _eventMediator.PlayerReconnectToGameRequested -= OnPlayerReconnectToGameRequested;
            _eventMediator.KickUserRequested -= OnKickUserRequested;
        }

        private void OnKickUserRequested(Guid userGuid)
        {
            KickUser(userGuid).Forget();
        }

        private void OnStartGameRequested(ulong adminClientId, MafiaPlayerRole[] userRolesPreset)
        {
            StartGame(adminClientId, userRolesPreset).Forget();
        }

        private void OnNextStageRequested(NextMafiaGameStageRequestDto sendingData)
        {
            SetNextStage(sendingData).Forget();
        }

        private void OnCompleteGameRequested()
        {
            CompleteGame().Forget();
        }

        private async UniTask CompleteGame()
        {
            await _mafiaGameService.CompleteGame(Table.TableData.TableId);
            Debug.Log("Mafia CompleteGame");
            ApplyState(null);    
        }

        private void RefreshGameState(MafiaGameStateDto gameStateDto)
        {
            if (gameStateDto == null)
            {
                Debug.Log("RefreshGameState. Mafia gameStateDto == null");              
                ApplyState(null);
                return;
            }

            Debug.Log($"Refresh gamestate {gameStateDto.GameStage}");

            _gameState = GameState.CreateFromMafiaGameStateDto(gameStateDto);
            if (_gameState == null)
            {
                ApplyState(null);
                return;
            }

            foreach (var playerState in _gameState.PlayerStates)
            {
                PlayerSeatInfo playerSeatInfo = _clientSeats[playerState.PlayerId.ToString()];
                playerState.Number = playerSeatInfo.Number;
                playerState.PlayerName = playerSeatInfo.UserName;
                playerState.ChairIndexInTable = playerSeatInfo.ChairIndexInTable;
            }
            ApplyState(_gameState);
        }

        private async UniTask KickUser(Guid userGuid)
        {
            if (_gameState == null)
            {
                Table.KickParticipant(userGuid);
                return;
            }

            MafiaGameStateDto gameStateDto = await _mafiaGameService.KickPlayer(Table.TableData.TableId, userGuid);
            RefreshGameState(gameStateDto);
        }

        private async UniTask StartGame(ulong adminClientId, MafiaPlayerRole[] userRolesPreset)
        {
            await _mafiaGameService.CompleteGame(Table.TableData.TableId);

            _clientSeats.Clear();

            List<string> playerIds = new();
            List<Place> participantPlaced = Table.GetOccupiedPlaces();
            int currentSeatNumber = 1;
            string adminId = "";

            foreach (Place place in participantPlaced)
            {
                ulong clientId = place.IsOccupiedID.Value;
                ClientInfo clientInfo = _serverVerification.Users.FirstOrDefault(x => x.ServerId == clientId);

                if (clientInfo == null)
                {
                    Debug.LogError($"No client info for Mafia. ClientId: {clientId}");
                }
                PlayerSeatInfo playerSeatInfo = new();
                playerSeatInfo.ChairIndexInTable = Table.GetChairIndexByPlace(place);
                playerSeatInfo.UserName = clientInfo.UserName;

                if (clientId == adminClientId)
                {
                    playerSeatInfo.Number = 0;
                    adminId = clientInfo.Id;
                }
                else
                {
                    playerSeatInfo.Number = currentSeatNumber;
                    currentSeatNumber++;
                    playerIds.Add(clientInfo.Id);
                }

                _clientSeats[clientInfo.Id] = playerSeatInfo;
            }

            if (string.IsNullOrEmpty(adminId))
            {
                Debug.LogError("There is no admin for the table");
            }

            MafiaGameStateDto gameStateDto = await _mafiaGameService.StartNewGame(Table.TableData.TableId, adminId, playerIds.ToArray(), userRolesPreset);
            RefreshGameState(gameStateDto);
        }

        [ContextMenu("Start debug game")]
        private async void StartGameDebug()
        {
            string adminId = "48607d4c-baa3-4f8e-852e-6c0845053048";
            string[] playersIds = new string[3];
            playersIds[0] = "3ef08090-13aa-4333-b6d2-c9f55f6d13ae";
            playersIds[1] = "2e01f4d3-da96-472c-a7be-388cd06a1137";
            playersIds[2] = "a807e1f0-140e-40df-a469-da26fc902d27";
            MafiaGameStateDto gameStateDto = await _mafiaGameService.StartNewGame(Table.TableData.TableId, adminId, playersIds, Array.Empty<MafiaPlayerRole>());
            RefreshGameState(gameStateDto);
        }

        private async UniTask SetNextStage(NextMafiaGameStageRequestDto sendingData)
        {
            MafiaGameStateDto gameStateDto = await _mafiaGameService.GoToNextStage(Table.TableData.TableId, sendingData);
            RefreshGameState(gameStateDto);
        }

        private void OnPlayerReconnectToGameRequested(Guid playerGuid)
        {
            ApplyState(_gameState);
        }

        private void ApplyState(GameState gameState)
        {
            _gameState = gameState;
            DebugGameState = gameState;

            MafiaNetworkBehaviour.SetState(_gameState);
            MafiaPlayersAudioSwitcher.SetState(_gameState);
            Table.SetState(_gameState);
        }
    }
}
