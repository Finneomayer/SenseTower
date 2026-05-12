using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using Assets.Scripts.Server;
using static Assets.Scripts.Server.ServerVerification;
using System.Collections;
using Assets.Mechanics.Network.Scripts;

namespace Assets.Mechanics.Mafia
{
    public class MafiaPlayersAutoKickBehaviour : NetworkBehaviour
    {
        [SerializeField]
        private int InactivityTimeToActivateKickTimer = 5;
        [SerializeField]
        private int InactivityTimeToKick = 300;

        private GameState _gameState;
        private MafiaEventMediatorServer _eventMediatorServer;
        private ServerVerification _serverVerification;
        private RoomUsersKicker _roomUsersKicker;

        private MafiaEventMediatorClient _eventMediatorClient;

        private Coroutine _autoKickRoutine;

        public void Init(MafiaEventMediatorServer eventMediator, ServerVerification serverVerification)
        {
            _eventMediatorServer = eventMediator;
            _serverVerification = serverVerification;
            _roomUsersKicker = FindObjectOfType<RoomUsersKicker>();
        }

        public void InitClient(MafiaEventMediatorClient eventMediator)
        {
            _eventMediatorClient = eventMediator;
        }

        public void SetState(GameState gameState)
        {
            _gameState = gameState;

            if (_gameState == null)
            {
                if (_autoKickRoutine != null)
                {
                    StopCoroutine(_autoKickRoutine);
                    _autoKickRoutine = null;
                }
            }
            else
            {
                if (_autoKickRoutine == null)
                {
                    _autoKickRoutine = StartCoroutine(AutoKickRoutine());
                }               
            }
        }

        private IEnumerator AutoKickRoutine()
        {
            Dictionary<string, MafiaPlayerKickTimeData> playersKickTimersMap = new();
            List<MafiaPlayerKickTimeData> playersKickTimersToSend = new();
            MafiaPlayersKickSerializedData playersKickSerializedData = new();

            var waitDelay = new WaitForSeconds(1);
            while (true)
            {
                yield return waitDelay;
                if (NetworkManager == null || _serverVerification == null)
                {
                    break;
                }

                if (_gameState == null || _gameState.PlayerStates == null)
                {
                    break;
                }

                foreach (var player in _gameState.PlayerStates)
                {
                    if (!player.IsAlive)
                    {
                        playersKickTimersMap.Remove(player.PlayerId.ToString());
                        continue;
                    }
                    string playerGuidString = player.PlayerId.ToString();
                    ClientInfo playerClientInfo = _serverVerification.Users.FirstOrDefault(x => x.Id == playerGuidString);

                    if (playerClientInfo == null 
                        || Time.unscaledTime - playerClientInfo.LastActiveTime >= InactivityTimeToActivateKickTimer)
                    {
                        if (playersKickTimersMap.TryGetValue(playerGuidString, out var kickTimer))
                        {
                            kickTimer.SecondsToKick--;
                        }
                        else
                        {
                            kickTimer = new() 
                            { 
                                PlayerId = playerGuidString,
                                SecondsToKick = InactivityTimeToKick,
                            };
                            playersKickTimersMap[playerGuidString] = kickTimer;
                        }
                    }
                    else
                    {
                        playersKickTimersMap.Remove(playerGuidString);
                    }
                }

                string adminGuidString = _gameState.GameMasterId.ToString();

                foreach (var playerKickTimer in playersKickTimersMap)
                {
                    if (playerKickTimer.Value.SecondsToKick >= 0)
                    {
                        continue;
                    }

                    _roomUsersKicker.KickUserServer(playerKickTimer.Value.PlayerId);
                    if (playerKickTimer.Key == adminGuidString)
                    {
                        _eventMediatorServer.RequestCompleteGame();
                    }
                    else
                    {
                        _eventMediatorServer.RequestKickUser(Guid.Parse(playerKickTimer.Value.PlayerId));
                    }
                }

                playersKickTimersToSend.Clear();
                foreach (var playerTimer in playersKickTimersMap)
                {
                    if (playerTimer.Value.SecondsToKick >= 0)
                    {
                        playersKickTimersToSend.Add(playerTimer.Value.CreateCopy());
                    }
                }

                playersKickSerializedData.PlayersKickTimers = playersKickTimersToSend;
                SendPlayersKickDataClientRpc(playersKickSerializedData);
            }

            _autoKickRoutine = null;
        }

        [ClientRpc(Delivery = RpcDelivery.Unreliable)]
        private void SendPlayersKickDataClientRpc(MafiaPlayersKickSerializedData playersKickData)
        {
            _eventMediatorClient?.RequestRefreshAutoKickData(playersKickData);
        }
    }
}
