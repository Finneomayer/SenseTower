using System;
using Unity.Netcode;

namespace Assets.Mechanics.AvatarMovementRecording.Scripts
{
    [Serializable]
    public class AvatarFrame : INetworkSerializable
    {
        public byte[] Data;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Data);
        }
    }
}
