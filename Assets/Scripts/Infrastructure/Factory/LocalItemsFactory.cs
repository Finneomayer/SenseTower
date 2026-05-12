using Assets.Scripts.TowerObjects;
using System;
using Data;
using UnityEngine;

namespace Infrastructure.Factory
{
    [Serializable]
    public class TowerObjectPrefab
    {
        [field: SerializeField]
        public Enumenators.PrefabObjectType PrefabObjectType { get; private set; }
        [field: SerializeField]
        public GameObject Prefab { get; private set; }
    }

    public class LocalItemsFactory : MonoBehaviour
    {
        [SerializeField]
        private TowerObjectPrefab[] Prefabs;

        public bool TryGetPrefab(Enumenators.PrefabObjectType prefabType, out GameObject prefab)
        {
            prefab = null;
            foreach (var prefabObj in Prefabs)
            {
                if (prefabObj.PrefabObjectType == prefabType)
                {
                    prefab = prefabObj.Prefab;
                }
            }

            return prefab != null;
        }
    }
}