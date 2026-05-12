using UnityEngine;

namespace Assets.Mechanics.MetaAvatars.Scripts
{
    [System.Serializable]
    public class AvatarSkinSelector
    {
        [SerializeField]
        private int minAssetID = 0;
        [SerializeField]
        private int maxAssetID = 31;
        [SerializeField]
        private int[] _overridedAvailableAssetIDs = new int[0];

        public int AssetID { get; private set; }

        public void SetRandomID()
        {
            if (_overridedAvailableAssetIDs.Length == 0)
            {
                AssetID = Random.Range(minAssetID, maxAssetID + 1);
            }
            else
            {
                AssetID = _overridedAvailableAssetIDs[Random.Range(0, _overridedAvailableAssetIDs.Length)];
            }

        }
    }
}
