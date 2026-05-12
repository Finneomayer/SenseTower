using Assets.Scripts.TowerObjects;
using UnityEngine;

namespace UI
{
    public class InventoryObject : MonoBehaviour
    {
        public TowerObjectDto TowerObjectDto { get; private set; }

        public void Init(TowerObjectDto towerObjectDto)
        {
            TowerObjectDto = towerObjectDto;
        }
    }
}
