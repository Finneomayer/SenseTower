using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Assets.Scripts.Server;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using CompressionLevel = System.IO.Compression.CompressionLevel;

namespace Assets.Mechanics.AvatarMovementRecording.Scripts
{
    [Serializable]
    public struct RecordParams : INetworkSerializable
    {
        public RecordParams(int avatarNumber, int lengthInSeconds, Vector3 shift)
        {
            AvatarNumber = avatarNumber;
            LengthInSeconds = lengthInSeconds;
            Shift = shift;
        }
        public int AvatarNumber;
        public int LengthInSeconds;
        public Vector3 Shift;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref AvatarNumber);
            serializer.SerializeValue(ref LengthInSeconds);
            serializer.SerializeValue(ref Shift);
        }
    }

    public class AvatarRecorderSaver
    {
        private bool _isInitedFromStartMethod = false;

        private IAvatarRecorderService _avatarRecorderService; 

        public void Init(IAvatarRecorderService service, IServerApiData serverApiData)
        {
            _avatarRecorderService = service;
            _avatarRecorderService.Init(serverApiData);
            _isInitedFromStartMethod = true;
        }

        public async UniTask Save(AvatarRecorderData data, Guid objectId, int duration)
        {
            if (!Directory.Exists($"{Application.temporaryCachePath}/Animation{objectId}"))
                Directory.CreateDirectory($"{Application.temporaryCachePath}/Animation{objectId}");
            else
            {
                DirectoryInfo info = new DirectoryInfo($"{Application.temporaryCachePath}/Animation{objectId}");
                foreach (var file in info.GetFiles())
                {
                    file.Delete();
                }
            }

            int i = 0;
            foreach (var frame in data.Movement)
            {
                int j = 0;
                foreach (var lod in frame)
                {
                    await File.WriteAllBytesAsync($"{Application.temporaryCachePath}/Animation{objectId}/{i}-{j}-avatar", data.Movement[i][j].Data);
                    j++;
                }
                i++;
            }

            await File.WriteAllBytesAsync($"{Application.temporaryCachePath}/Animation{objectId}/audio.wav",OpenWavParser.AudioClipToByteArray(data.Audio));

            RecordParams param = data.AvatarParameters;
            string jsonString = JsonUtility.ToJson(param);

            await File.WriteAllTextAsync($"{Application.temporaryCachePath}/Animation{objectId}/params.txt", jsonString);

            if (File.Exists($"{Application.temporaryCachePath}/Animation{objectId}.zip"))
                File.Delete($"{Application.temporaryCachePath}/Animation{objectId}.zip");

            ZipFile.CreateFromDirectory($"{Application.temporaryCachePath}/Animation{objectId}",
                $"{Application.temporaryCachePath}/Animation{objectId}.zip", CompressionLevel.NoCompression, true);

            await _avatarRecorderService.SaveRecording(objectId, $"{Application.temporaryCachePath}/Animation{objectId}.zip", duration);
        }

        public async UniTask<int> CheckRecordAvailability(Guid objectId) 
        {
            var utcs = new UniTaskCompletionSource<int>();

            //is called from OnNetworkSpawn method
            //so, it's earlier than Init method, which is called from Start method

            await UniTask.WaitUntil(() => _isInitedFromStartMethod = true);

            var response = await _avatarRecorderService.GetInfo(objectId); 

            if (response != null && response.Count > 0)
            {
                utcs.TrySetResult(response[0].DurationSeconds);
            }
            else
            {
                utcs.TrySetResult(-1);
            }
            return await utcs.Task;
        }

        public async UniTask<AvatarRecorderData> Open(Guid objectId)
        {
            var utcs = new UniTaskCompletionSource<AvatarRecorderData>();

            bool canOpen = await _avatarRecorderService.OpenRecording(objectId);

            if (!canOpen)
            {
                utcs.TrySetResult(null);
                return await utcs.Task;
            }
            
            if (Directory.Exists($"{Application.temporaryCachePath}/AnimationExtracted{objectId}"))
            {
                DirectoryInfo info = new DirectoryInfo($"{Application.temporaryCachePath}/AnimationExtracted{objectId}/Animation{objectId}");
                foreach (var file in info.GetFiles())
                {
                    file.Delete();
                }
                Directory.Delete($"{Application.temporaryCachePath}/AnimationExtracted{objectId}/Animation{objectId}");
                Directory.Delete($"{Application.temporaryCachePath}/AnimationExtracted{objectId}");
            }

            ZipFile.ExtractToDirectory($"{Application.temporaryCachePath}/Animation{objectId}.zip", 
                $"{Application.temporaryCachePath}/AnimationExtracted{objectId}");

            var movement = new List<AvatarFrame[]>();
            int framesCount = new System.IO.DirectoryInfo($"{Application.temporaryCachePath}/AnimationExtracted{objectId}/Animation{objectId}").GetFiles().Length / 4;

            for (int i = 0; i < framesCount; i++)
            {
                var frame = new AvatarFrame[4];

                for (int j = 0; j < 4; j++)
                {
                    frame[j] = new AvatarFrame
                    {
                        Data = File.ReadAllBytes($"{Application.temporaryCachePath}/AnimationExtracted{objectId}/Animation{objectId}/{i}-{j}-avatar")
                    };
                }

                movement.Add(frame);
            }

            var audio = File.ReadAllBytes($"{Application.temporaryCachePath}/AnimationExtracted{objectId}/Animation{objectId}/audio.wav");

            var output = File.ReadAllText($"{Application.temporaryCachePath}/AnimationExtracted{objectId}/Animation{objectId}/params.txt");

            RecordParams param = JsonUtility.FromJson<RecordParams>(output);

            var result = new AvatarRecorderData(movement, OpenWavParser.ByteArrayToAudioClip(audio), param);

            utcs.TrySetResult(result);
            return await utcs.Task;
        }
    }
}
