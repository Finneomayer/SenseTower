using System;
using Assets.Scripts.TowerObjects;

namespace Assets.Scripts.TowerObjectsRevision.Models
{
    public class SaveTowerObjectRequestDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid TowerClassObjectId { get; set; }
        public BusinessUnitType OwnerBusinessUnitType { get; set; }
        /*
        {
            "name": "Место посадки",
            "description": "Место посадки",
            "towerClassObjectId": "7b020e9e-6e85-4053-8beb-dbc238685682",
            "ownerBusinessUnitType": 0
        }
        */
    }
}