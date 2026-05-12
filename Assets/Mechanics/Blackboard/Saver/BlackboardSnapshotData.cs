using System;

namespace Assets.Blackboard
{
    [Serializable]
    public class BlackboardSnapshotData
    {
        public BlackboardSerializableData Data;
        public byte[] BlackboardScreenThumbnailData;
        public string SpaceIdData;
        public string BlackboardIdData;

        public Guid? SpaceId => string.IsNullOrEmpty(SpaceIdData) ? null : new Guid(SpaceIdData);
        public Guid? BlackboardId => string.IsNullOrEmpty(BlackboardIdData) ? null : new Guid(BlackboardIdData);
    }
}
