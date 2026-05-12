using UI;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Blackboard
{
    public sealed class BlackboardSnapshotsView : ViewPanel
    {
        [SerializeField]
        private BlackboardSnapshotItem SnapshotPrefab;
        [SerializeField]
        private RectTransform SnapshotsContent;

        private BlackboardEventMediator _blackboardEventMediator;
        private Dictionary<string, BlackboardSnapshotItem> _blackboardSnapshotItemsMap = new();

        public void Init(BlackboardEventMediator blackboardEventMediator)
        {
            _blackboardEventMediator = blackboardEventMediator;
        }

        public void AddItem(string filename, byte[] thumbnailData)
        {
            BlackboardSnapshotItem snapshotItem = Instantiate(SnapshotPrefab, SnapshotsContent);
            snapshotItem.Init(_blackboardEventMediator, filename, thumbnailData);
            _blackboardSnapshotItemsMap[filename] = snapshotItem;
        }

        public void RemoveItem(string filename)
        {
            if (_blackboardSnapshotItemsMap.TryGetValue(filename, out BlackboardSnapshotItem item))
            {
                _blackboardSnapshotItemsMap.Remove(filename);
                Destroy(item.gameObject);
            }
        }

        public void DestroyItems()
        {
            foreach (var item in _blackboardSnapshotItemsMap.Values)
            {
                Destroy(item.gameObject);
            }
            _blackboardSnapshotItemsMap.Clear();
        }
    }
}


