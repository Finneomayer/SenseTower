using System;
using System.Linq;
using Unity.Netcode;

namespace Assets.Mechanics.Mafia
{
    [Serializable]
    public class GameState : INetworkSerializable
    {
        public string TableId;
        public MafiaGameStage GameStage;
        public MafiaGameStage NextGameStage;
        public PlayerState[] PlayerStates;
        public int Day;
        public int Turn;
        public MafiaGameStatus GameStatus;
        public Guid GameMasterId;
        public Guid[] VotingCandidates;

        public static GameState CreateFromMafiaGameStateDto(MafiaGameStateDto mafiaGameStateDto)
        {
            GameState gameState = new();
            gameState.TableId = mafiaGameStateDto.TableId;
            gameState.GameStage = mafiaGameStateDto.GameStage;
            gameState.NextGameStage = mafiaGameStateDto.NextGameStage;
            gameState.Day = mafiaGameStateDto.Day;
            gameState.Turn = mafiaGameStateDto.Turn;
            gameState.GameStatus = mafiaGameStateDto.GameStatus;
            gameState.GameMasterId = mafiaGameStateDto.GameMasterId;
            gameState.VotingCandidates = mafiaGameStateDto.VotingCandidates.ToArray();

            gameState.PlayerStates = new PlayerState[mafiaGameStateDto.PlayerStates.Count];
            for (int i = 0; i < gameState.PlayerStates.Length; i++)
            {
                gameState.PlayerStates[i] = PlayerState.CreateFromDto(mafiaGameStateDto.PlayerStates[i]);
                if (gameState.PlayerStates[i].Role != MafiaPlayerRole.GameMaster 
                    && gameState.GameStatus != MafiaGameStatus.InProgress)
                {
                    gameState.PlayerStates[i].AvailableActions = new MafiaPlayerAction[]
                    {
                        MafiaPlayerAction.Watch,
                        MafiaPlayerAction.Talk
                    };
                }
            }
            return gameState;
        }

        public bool IsVoting()
        {
            if (PlayerStates == null)
            {
                return false;
            }

            foreach (PlayerState playerState in PlayerStates)
            {
                if (playerState.Role != MafiaPlayerRole.GameMaster && playerState.AvailableActions.Contains(MafiaPlayerAction.Vote))
                {
                    return true;
                }
            }

            return false;
        }

        public bool CanSelectPlayer(PlayerState initiator, PlayerState playerToSelect, bool gameMasterCanSelect = false)
        {
            if (initiator == null || playerToSelect == null)
            {
                return false;
            }

            if (!gameMasterCanSelect && initiator.Role == MafiaPlayerRole.GameMaster)
            {
                return false;
            }

            if (playerToSelect.Role == MafiaPlayerRole.GameMaster)
            {
                return false;
            }

            if (!initiator.IsAlive || !playerToSelect.IsAlive)
            {
                return false;
            }

            if (!initiator.AvailableActions.Contains(MafiaPlayerAction.Vote))
            {
                return false;
            }

            return VotingCandidates.Contains(playerToSelect.PlayerId);
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref TableId);
            serializer.SerializeValue(ref GameStage);
            serializer.SerializeValue(ref NextGameStage);
            serializer.SerializeValue(ref Day);
            serializer.SerializeValue(ref Turn);
            serializer.SerializeValue(ref GameStatus);

            SerializeGuid(serializer, ref GameMasterId);

            SerializePlayerStates(serializer);
            SerializeVotingCandidates(serializer);
        }

        private void SerializeGuid<T>(BufferSerializer<T> serializer, ref Guid guid) where T : IReaderWriter
        {
            string gameMasterIdString = string.Empty;
            if (serializer.IsWriter)
            {
                gameMasterIdString = guid.ToString();
                serializer.SerializeValue(ref gameMasterIdString);
            }
            else
            {
                serializer.SerializeValue(ref gameMasterIdString);
                guid = Guid.Parse(gameMasterIdString);
            }
        }

        private void SerializePlayerStates<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            int playerStatesCount = 0;
            if (serializer.IsWriter)
            {
                playerStatesCount = PlayerStates.Length;
                serializer.SerializeValue(ref playerStatesCount);
                foreach (PlayerState playerState in PlayerStates)
                {
                    playerState.NetworkSerialize(serializer);
                }
            }
            else
            {
                serializer.SerializeValue(ref playerStatesCount);
                PlayerStates = new PlayerState[playerStatesCount];
                for (int i = 0; i < PlayerStates.Length; i++)
                {
                    PlayerStates[i] = new PlayerState();
                    PlayerStates[i].NetworkSerialize(serializer);
                }
            }
        }

        private void SerializeVotingCandidates<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            int votingCandidatesCount = 0;
            if (serializer.IsWriter)
            {
                if (VotingCandidates == null)
                {
                    serializer.SerializeValue(ref votingCandidatesCount);
                    return;
                }

                votingCandidatesCount = VotingCandidates.Length;
                serializer.SerializeValue(ref votingCandidatesCount);

                foreach (Guid playerGuid in VotingCandidates)
                {
                    string playerGuidString = playerGuid.ToString();
                    serializer.SerializeValue(ref playerGuidString);
                }
            }
            else
            {
                serializer.SerializeValue(ref votingCandidatesCount);
                VotingCandidates = new Guid[votingCandidatesCount];
                for (int i = 0; i < votingCandidatesCount; i++)
                {
                    string playerGuidString = string.Empty;
                    serializer.SerializeValue(ref playerGuidString);
                    VotingCandidates[i] = Guid.Parse(playerGuidString);
                }
            }
        }
    }
}
