using System.Collections.Generic;
using Unity.Netcode;

namespace Assets.Mechanics.Mafia
{
    public class MafiaPlayersKickSerializedData : INetworkSerializable
    {
        public List<MafiaPlayerKickTimeData> PlayersKickTimers = new();

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            int count = 0;
            if (serializer.IsWriter)
            {
                count = PlayersKickTimers.Count;
                serializer.SerializeValue(ref count);
                foreach (var item in PlayersKickTimers)
                {
                    item.NetworkSerialize(serializer);
                }
            }
            else
            {
                PlayersKickTimers = new();
                serializer.SerializeValue(ref count);
                for (int i = 0; i < count; i++)
                {
                    MafiaPlayerKickTimeData data = new();
                    data.NetworkSerialize(serializer);
                    PlayersKickTimers.Add(data);
                }
            }
        }
    }

    public class MafiaPlayerKickTimeData : INetworkSerializable
    {
        public string PlayerId;
        public int SecondsToKick;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref PlayerId);
            serializer.SerializeValue(ref SecondsToKick);
        }

        public MafiaPlayerKickTimeData CreateCopy()
        {
            MafiaPlayerKickTimeData kickData = new()
            {
                PlayerId = this.PlayerId,
                SecondsToKick = this.SecondsToKick,
            };
            return kickData;
        }
    }
}
