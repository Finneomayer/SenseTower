using System;
using System.Collections.Generic;
using Assets.Scripts.Server;
using Cysharp.Threading.Tasks;

namespace Assets.Mechanics.AvatarMovementRecording.Scripts
{
    public interface IAvatarRecorderService
    {
        public void Init(IServerApiData serverApiData);
        public UniTask<bool> OpenRecording(Guid towerObjectId);
        public UniTask<bool> SaveRecording(Guid id, string fileUrl, int duration);
        public UniTask<bool> DeleteRecording(string recordId);
        public UniTask<List<RecordInfo>> GetInfo(Guid towerObjectId);
    }
}
