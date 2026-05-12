using System;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Models;
using Assets.Scripts.Server;
using Assets.Scripts.WebUtils;
using Cysharp.Threading.Tasks;
using Models;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Mechanics.AvatarMovementRecording.Scripts
{
    public class RecordInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public string Timestamp { get; set; }
        public int DurationSeconds { get; set; }
        public string TowerObjectId { get; set; }
    }

    public class AvatarRecorderService : IAvatarRecorderService
    {
        private bool _isBusy = false;

        private IServerApiData _serverApiData;
        
        public void Init(IServerApiData serverApiData)
        {
            _serverApiData = serverApiData;
        }

        public async UniTask<bool> OpenRecording(Guid towerObjectId)
        {
            await UniTask.WaitUntil(() => !_isBusy);
            _isBusy = true;

            var objectInfo = await GetInfo(towerObjectId);
            string recordId = objectInfo[0].Id;

            var utcs = new UniTaskCompletionSource<bool>();
            await UniTask.WaitUntil(() => !string.IsNullOrEmpty(ServerApiService.GetAvatarRecorderEndPoint));
            var url = ServerApiService.GetAvatarRecorderEndPoint;

            UnityWebRequest request = UnityWebRequest.Get($"{url}/{recordId}/file");
            request.SetRequestHeader("Authorization", $"Bearer {_serverApiData.AccessToken}");
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                utcs.TrySetResult(true);
                await File.WriteAllBytesAsync($"{Application.temporaryCachePath}/Animation{towerObjectId}.zip", request.downloadHandler.data);
                _isBusy = false;
            }
            else
            {
                utcs.TrySetResult(false);
                _isBusy = false;
            }
            return await utcs.Task;
        }

        public async UniTask<bool> SaveRecording(Guid towerObjectId, string fileUrl, int duration)
        {
            await UniTask.WaitUntil(() => !_isBusy);
            _isBusy = true;

            var records = await GetInfo(towerObjectId);
            if (records != null && records.Count > 0)
            {
                foreach (var rec in records)
                {
                    Debug.Log($"Pre delete Record with record id {rec.Id} ");
                }

                foreach (var rec in records)
                {
                    await DeleteRecording(rec.Id);
                }
            }

            var data = await File.ReadAllBytesAsync(fileUrl);

            var formData = new List<IMultipartFormSection>();

            formData.Add(new MultipartFormDataSection("Name", $"Animation{towerObjectId}.zip"));
            formData.Add(new MultipartFormFileSection("File", data, $"Animation{towerObjectId}.zip", "multipart/form-data"));
            formData.Add(new MultipartFormDataSection("TowerObjectId", towerObjectId.ToString()));
            formData.Add(new MultipartFormDataSection("DurationSeconds", duration.ToString()));

            var utcs = new UniTaskCompletionSource<bool>();

            await UniTask.WaitUntil(() => !string.IsNullOrEmpty(ServerApiService.GetAvatarRecorderEndPoint));

            var url = ServerApiService.GetAvatarRecorderEndPoint;

            UnityWebRequest request = UnityWebRequest.Post(url, formData);
            request.SetRequestHeader("Authorization", $"Bearer {_serverApiData.AccessToken}");
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                utcs.TrySetResult(true);
                _isBusy = false;
            }
            else
            {
                utcs.TrySetResult(false);
                _isBusy = false;
            }
            return await utcs.Task;
        }

        public async UniTask<bool> DeleteRecording(string recordId)
        {
            var utcs = new UniTaskCompletionSource<bool>();
            var url = ServerApiService.GetAvatarRecorderEndPoint;

            Debug.Log($"Deleting record num {recordId}");
            UnityWebRequest request = UnityWebRequest.Delete($"{url}/{recordId}");
            request.SetRequestHeader("Authorization", $"Bearer {_serverApiData.AccessToken}");

            await request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                utcs.TrySetResult(true);
            }
            else
            {
                utcs.TrySetResult(false);
            }
            return await utcs.Task;
        }

        public async UniTask<List<RecordInfo>> GetInfo(Guid towerObjectId)
        {
            var utcs = new UniTaskCompletionSource<List<RecordInfo>>();

            await UniTask.WaitUntil(() => !string.IsNullOrEmpty(ServerApiService.GetAvatarRecorderEndPoint)); 
            var url = ServerApiService.GetAvatarRecorderEndPoint;

            UnityWebRequest request = UnityWebRequest.Get($"{url}?TowerObjectId={towerObjectId.ToString()}");
            request.SetRequestHeader("Authorization", $"Bearer {_serverApiData.AccessToken}");
            
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log(request.downloadHandler.text);

                List<RecordInfo> records  = JsonConvert.DeserializeObject<List<RecordInfo>>(request.downloadHandler.text);
                utcs.TrySetResult(records);
            }
            else
            {
                utcs.TrySetResult(null);
            }
            return await utcs.Task;
        }
    }
}
