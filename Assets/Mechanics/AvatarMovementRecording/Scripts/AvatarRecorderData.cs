using System;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Mechanics.AvatarMovementRecording.Scripts
{
    [Serializable]
    public class AvatarRecorderData : INetworkSerializable
    {
        public AvatarRecorderData(List<AvatarFrame[]> movement, AudioClip audio, RecordParams param)
        {
            Movement = movement;
            Audio = audio;
            AvatarParameters = param;
        }

        public List<AvatarFrame[]> Movement;
        public AudioClip Audio;
        public RecordParams AvatarParameters;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            float[] audioFloat = null;
            int frameCount = 0;
            int frameLodLength = 0;

            if (serializer.IsWriter)
            {
                //audio
                audioFloat = new float[Audio.samples * Audio.channels];
                Audio.GetData(audioFloat, 0);
                serializer.SerializeValue(ref audioFloat);
                //movement
                frameCount = Movement.Count;
                serializer.SerializeValue(ref frameCount);
                foreach (var frameLod in Movement)
                {
                    frameLodLength = frameLod.Length;
                    serializer.SerializeValue(ref frameLodLength);
                    var frame = frameLod;
                    serializer.SerializeValue(ref frame);
                }
            }
            else //IsReader
            {
                //audio
                serializer.SerializeValue(ref audioFloat);
                Audio.SetData(audioFloat, 0);
                //movement
                serializer.SerializeValue(ref frameCount);
                for (int i = 0; i < frameCount; i++)
                {
                    serializer.SerializeValue(ref frameLodLength);
                    var frame = new AvatarFrame[frameLodLength];
                    serializer.SerializeValue(ref frame);
                    Movement[i] = frame;
                }
            }
            serializer.SerializeValue(ref AvatarParameters);
        }
    }
}
