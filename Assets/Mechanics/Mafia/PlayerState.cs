using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Mechanics.Mafia
{

    [Serializable]
    public enum Effect
    {
        DeathByMafia = 0,
        DonsVisit = 1,
        ChangeRoleToCommissioner = 2,
        DeathByPsycho = 3,
        DoctorProtection = 4,
        VotingProtection = 5, //uses only for lover effect!
        LawyerProtection = 6,
        DeathByPoison = 7,
        NoEffect = 8,
        DeathByVoting = 9,
        InvestigatedByCommissioner = 10,
        LawyerCantChooseTwice = 11,
        DoctorCantChooseTwice = 12,
        LoverCantChooseTwice = 13,
    }

    public enum MafiaGameStatus
    {
        InProgress = 0,
        MafiaWin = 1,
        CitizensWin = 2,
        ManualEnd = 3,
        GameMasterOffline = 4,
        AllPlayersOffline = 5,

    }

    [Serializable]
    public class PlayerState : INetworkSerializable
    {
        public const int DefaultSelectedNumber = -1;

        public Guid PlayerId;
        public MafiaPlayerRole Role;
        public MafiaPlayerAction[] AvailableActions;
        public bool IsAlive;
        public int Number;
        public string PlayerName = string.Empty;
        public int SelectedNumberOfOtherPlayer = DefaultSelectedNumber;
        public string SelectedNameOfOtherPlayer = string.Empty;
        public int VoteCount;
        public int ChairIndexInTable;
        public Dictionary<Effect, int> Effects;
        public bool IsDeadManLastWords;

        public static PlayerState CreateFromDto(MafiaPlayerStateDto mafiaPlayerStateDto)
        {
            PlayerState playerState = new();
            playerState.PlayerId = mafiaPlayerStateDto.PlayerId;
            playerState.Role = mafiaPlayerStateDto.Role;
            playerState.AvailableActions = mafiaPlayerStateDto.MafiaPlayerActions;
            playerState.IsAlive = mafiaPlayerStateDto.IsAlive;
            playerState.Effects = mafiaPlayerStateDto.Effects;
            playerState.IsDeadManLastWords = mafiaPlayerStateDto.IsDeadManLastWords;

            return playerState;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            string playerIdString = null;
            Effect[] effectType = null;
            int[] effectCount = null;

            if (serializer.IsWriter)
            {
                playerIdString = PlayerId.ToString();
                serializer.SerializeValue(ref playerIdString);
                //
                effectType = Effects.Keys.ToArray();
                effectCount = Effects.Values.ToArray();
                serializer.SerializeValue(ref effectType);
                serializer.SerializeValue(ref effectCount);
            }
            else
            {
                serializer.SerializeValue(ref playerIdString);
                PlayerId = Guid.Parse(playerIdString);
                //
                serializer.SerializeValue(ref effectType);
                serializer.SerializeValue(ref effectCount);
                Effects = new Dictionary<Effect, int>();
                for (int i = 0; i < effectType.Length; i++)
                {
                    Effects.Add(effectType[i], effectCount[i]);
                }
            }

            serializer.SerializeValue(ref Number);
            serializer.SerializeValue(ref PlayerName);
            serializer.SerializeValue(ref Role);
            serializer.SerializeValue(ref AvailableActions);
            serializer.SerializeValue(ref IsAlive);
            serializer.SerializeValue(ref IsDeadManLastWords);

            serializer.SerializeValue(ref SelectedNumberOfOtherPlayer);
            serializer.SerializeValue(ref SelectedNameOfOtherPlayer);
            serializer.SerializeValue(ref VoteCount);
            serializer.SerializeValue(ref ChairIndexInTable);
        }
    }

    public class PlayerSeatInfo
    {
        public int ChairIndexInTable;
        public int Number;
        public string UserName;
    }
}
