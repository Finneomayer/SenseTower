using System;
using Assets.Scripts.TowerObjects;

namespace Assets.Scripts.TowerObjectsClass.Models
{
    public class Owner
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public BusinessUnitType? BusinessUnitType;

    }
}