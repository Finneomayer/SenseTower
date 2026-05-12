using System.Linq;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "CustomLogicTowerObjectsSet", menuName = "Static Data/CustomLogicTowerObjectsSet")]
    public class CustomLogicTowerObjectsSet : ScriptableObject
    {
        [SerializeField]
        private CustomLogicTowerObjectData[] CustomLogicTowerObjects;

        public bool TryGetPrefabByKey(string key, out GameObject prefab)
        {
            prefab = null;
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }
            
            CustomLogicTowerObjectData data = CustomLogicTowerObjects.FirstOrDefault((x) => x.Key == key);
            if (data != null)
            {
                prefab = data.Prefab;
                return true;
            }
            return false;
        }

        public bool TryGetInventoryPrefabByKey(string key, out GameObject prefab)
        {
            prefab = null;
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            CustomLogicTowerObjectData data = CustomLogicTowerObjects.FirstOrDefault((x) => x.Key == key);
            if (data != null)
            {
                prefab = data.InventoryModelPrefab;
                return true;
            }
            return false;
        }
    }
}
