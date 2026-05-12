using System;
using Assets.Scripts.TowerObjects;
using Data;

namespace Assets.Scripts.TowerObjectsClass.Models
{
    public class TowerObjectClass
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PrefabName { get; set; }
        public Enumenators.PrefabObjectType PrefabObjectType;
        public RemoteObjectTypeInfo RemoteObjectTypeInfo;
    }
}