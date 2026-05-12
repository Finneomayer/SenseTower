using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class BlackBoardInfo : IDisposable
{
    public Texture2D TexturePicture;
    public int Width;
    public int Height;

    public BlackBoardInfo() { }

    ~BlackBoardInfo()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        TexturePicture = default;
    }

    public byte[] Serialize()
    {
        using (MemoryStream m = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(m))
            {
                Width = TexturePicture.width;
                Height = TexturePicture.height;
                writer.Write(Width);
                writer.Write(Height);
                //writer.Write(TexturePicture.GetRawTextureData().Length);
                //writer.Write(TexturePicture.GetRawTextureData());
                writer.Write(TexturePicture.EncodeToPNG().Length);
                writer.Write(TexturePicture.EncodeToPNG());
            }
            return m.ToArray();
        }
    }

    public static BlackBoardInfo Deserialize(byte[] data)
    {
        BlackBoardInfo result = new BlackBoardInfo();

        using (MemoryStream m = new MemoryStream(data))
        {
            using (BinaryReader reader = new BinaryReader(m))
            {
                result.Width = reader.ReadInt32();
                result.Height = reader.ReadInt32();
                int dataLength = reader.ReadInt32();
                result.TexturePicture = new Texture2D(result.Width, result.Height);
                result.TexturePicture.LoadImage(reader.ReadBytes(dataLength));
                //result.TexturePicture.LoadRawTextureData(reader.ReadBytes(dataLength));
                result.TexturePicture.Apply();
            }
        }
        return result;
    }
}
