using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "CustomLogicTowerObjectData", menuName = "Static Data/CustomLogicTowerObjectData")]
    public class CustomLogicTowerObjectData : ScriptableObject
    {
        [field: SerializeField]
        public string Key;

        [field: SerializeField]
        public GameObject Prefab { get; private set; }

        [field: SerializeField]
        public GameObject InventoryModelPrefab { get; private set; }
    }
}
