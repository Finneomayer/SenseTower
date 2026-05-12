using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public class BlackboardTextureSerializable : INetworkSerializable
{
    public BlackBoardInfo NetworkBlackboard = new();
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsWriter)
        {
            SerializeForNetwork(serializer);
        }
        else
        {
            DeserializeForNetwork(serializer);
        }
    }

    private void SerializeForNetwork<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        byte[] textureBytes = SerializeData();
        serializer.SerializeValue(ref textureBytes);
    }

    private void DeserializeForNetwork<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        byte[] textureBytes = null;
        serializer.SerializeValue(ref textureBytes);
        DeserializeData(textureBytes);
    }

    public static BlackboardTextureSerializable Deserialize(byte[] data)
    {
        BlackboardTextureSerializable result = new();
        result.DeserializeData(data);
        return result;
    }

    public byte[] SerializeData()
    {
        using (MemoryStream m = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(m))
            {
                byte[] data = NetworkBlackboard.Serialize();
                writer.Write(data.Length);
                writer.Write(data);
            }
            return m.ToArray();
        }
    }

    public void DeserializeData(byte[] data)
    {
        NetworkBlackboard = new();

        using (MemoryStream m = new MemoryStream(data))
        {
            using (BinaryReader reader = new BinaryReader(m))
            {
                int dataLength = reader.ReadInt32();
                byte[] blackboardData = reader.ReadBytes(dataLength);
                NetworkBlackboard = BlackBoardInfo.Deserialize(blackboardData);
            }
        }
    }
}
