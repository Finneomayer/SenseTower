using System;
using System.Collections.Generic;
using Assets.Scripts.Client;
using UnityEngine;
using Assets.Mechanics.Mafia.Table;
using System.Linq;
using Squirrel.SimpleSplat;

namespace Assets.Mechanics.Mafia
{
    public class PlayerSeatsController : MonoBehaviour
    {
        [SerializeField]
        private Transform PlayerSeatsContent;
        [SerializeField]
        private PlayerSeat PlayerSeatPrefab;

        private GameState _gameState;
        private PlayerSeat[] _playerSeats;
        private MafiaPlayersKickSerializedData _playersKickData;

        private MafiaEventMediatorClient _eventMediator;
        private Guid _localUserId;
        private TableExpander _mafiaTable;
        private List<Guid> _wasDeadGuidList = new List<Guid>();
        private MafiaLocalizationResultDto _mafiaLocalizationData;
        private GameState _previousGameState;
        
        public void Init(MafiaEventMediatorClient eventMediator, MafiaLocalizationResultDto mafiaLocalizationData,
            IClientData clientData, TableExpander mafiaTable)
        {
            _eventMediator = eventMediator;
            if (clientData.UserId != null)
            {
                _localUserId = clientData.UserId.Value;
            }
            _mafiaTable = mafiaTable;
            _mafiaLocalizationData = mafiaLocalizationData;
        }

        public void SetState(GameState gameState)
        {
            _gameState = gameState;
            Fill(_gameState);
            RefreshKickView();
        }

        public void SetPlayersKickData(MafiaPlayersKickSerializedData playersKickData)
        {
            _playersKickData = playersKickData;
            RefreshKickView();
        }

        private void RefreshKickView()
        {
            if (_playerSeats == null || _gameState == null || _gameState.PlayerStates == null || _playersKickData == null)
            {
                return;
            }

            foreach (var playerSeat in _playerSeats)
            {
                string playerGuidString = playerSeat.PlayerSeatData.PlayerId.ToString();
                var playerKickData = _playersKickData.PlayersKickTimers.FirstOrDefault(x => x.PlayerId == playerGuidString);
                
                if (playerKickData == null)
                {
                    playerSeat.SetKickTimerActive(false);
                    continue;
                }

                playerSeat.SetKickTimerActive(true);
                playerSeat.SetKickTimerValue(playerKickData.SecondsToKick);
            }
        }

        private void Fill(GameState gameState)
        {
            Clear();

            if (gameState == null || gameState.PlayerStates == null)
            {
                _wasDeadGuidList.Clear();
                return;
            }

            if (!gameState.TryGetPlayerStateByUserId(_localUserId.ToString(), out PlayerState currentPlayerState))
            {
                Debug.LogError("There is no current user in game");
                return;
            }

            if (!currentPlayerState.AvailableActions.Contains(MafiaPlayerAction.Watch))
            {
                return;
            }

            PlayerState selectedPlayerState = null;
            if (currentPlayerState.SelectedNumberOfOtherPlayer != PlayerState.DefaultSelectedNumber
                && !gameState.TryGetPlayerStateBySeatNumber(currentPlayerState.SelectedNumberOfOtherPlayer, out selectedPlayerState))
            {
                Debug.LogError("There is no selected user in game");
            }

            _playerSeats = new PlayerSeat[gameState.PlayerStates.Length];

            bool isSecondVoting = IsVotingSecondary(gameState); //second tour of voting

            for (int i = 0; i < _playerSeats.Length; i++)
            {
                if (!_mafiaTable.TryGetPlaceByChairIndex(gameState.PlayerStates[i].ChairIndexInTable, out Place playerPlace))
                {
                    Debug.LogWarning($"Cannot find place object for player {gameState.PlayerStates[i].Number}");
                    continue;
                }

                PlayerSeat newSeat = Instantiate(PlayerSeatPrefab, playerPlace.transform.position,
                    playerPlace.transform.rotation, PlayerSeatsContent);

                bool canBeSelected = gameState.CanSelectPlayer(currentPlayerState, gameState.PlayerStates[i]);

                bool canBeSelectedWithMasterIncluded = gameState.CanSelectPlayer(currentPlayerState, gameState.PlayerStates[i], true);

                newSeat.Init(gameState, _eventMediator, _mafiaLocalizationData, gameState.PlayerStates[i], 
                    canBeSelected, isSecondVoting && canBeSelectedWithMasterIncluded, currentPlayerState, WasDeadCheck(gameState.PlayerStates[i]));

                if (gameState.PlayerStates[i] == selectedPlayerState)
                {
                    newSeat.SelectSeat();
                }
                _playerSeats[i] = newSeat;
            }

            if (currentPlayerState.Role == MafiaPlayerRole.GameMaster || gameState.GameStatus != MafiaGameStatus.InProgress)
            {
                for (int i = 0; i < _playerSeats.Length; i++)
                {
                    _playerSeats[i].ShowRole();
                }
            }

            if ((currentPlayerState.Role == MafiaPlayerRole.Commissioner ||
                 currentPlayerState.Role == MafiaPlayerRole.Sergent) &&
                gameState.GameStage == MafiaGameStage.CommissionerResult)
            {
                for (int i = 0; i < _playerSeats.Length; i++)
                {
                    _playerSeats[i].ApplyInvestigatedByCommissionerEffect();
                }
            }

            if ((
                    currentPlayerState.Role == MafiaPlayerRole.Mafia 
                 || currentPlayerState.Role == MafiaPlayerRole.Don
                 ) && (
                    gameState.GameStage == MafiaGameStage.MafiaMeetEachOther
                    || gameState.GameStage == MafiaGameStage.DonTurn
                    || gameState.GameStage == MafiaGameStage.MafiaTurn))
            {
                for (int i = 0; i < _playerSeats.Length; i++)
                {
                    _playerSeats[i].ShowMafiaDonEachOther();
                }
            }
        }

        private void Clear()
        {
            if (_playerSeats == null)
            {
                return;
            }

            foreach (var item in _playerSeats)
            {
                Destroy(item.gameObject);
            }
            _playerSeats = null;
        }

        private bool WasDeadCheck(PlayerState playerState)
        {
            if (_wasDeadGuidList.Contains(playerState.PlayerId)) return true;

            if (playerState.IsAlive == false)
            {
                _wasDeadGuidList.Add(playerState.PlayerId);
            }

            return false;
        }

        private bool IsVotingSecondary(GameState gameState)
        {
            if (gameState == null) return false;

            bool result = (_previousGameState != null && gameState.VotingCandidates.Length != 0
                                                      && (gameState.PlayerStates.Length != gameState.VotingCandidates.Length + 1)
                                                      && _previousGameState.GameStage == MafiaGameStage.Voting
                                                      && gameState.GameStage == MafiaGameStage.Voting);
            _previousGameState = gameState;

            return result;
        }
    }
}
