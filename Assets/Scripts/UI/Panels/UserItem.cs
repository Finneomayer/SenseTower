using Assets.Localization;
using System;
using Assets.Scripts.TowerObjects;
using Data;
using UnityEngine;
using Infrastructure.Factory;

namespace UI
{
    [Serializable]
    public class UserItem
    {
        [field: SerializeField]
        public string TowerObjectClassName { get; set; }

        [field: SerializeField]
        public GameObject ItemModelPrefab { get; set; }

        [field: SerializeField]
        public RemoteObjectTypeInfo RemoteObjectTypeInfo { get; set; }

        [field: SerializeField]
        public LocalizationVariant LabelLocalizationVariant { get; set; }

        public BoughtItems ConvertToBoughtItems()
        {
            var item = new BoughtItems
            {
                TowerObject = new TowerObjectDto(),
                Count = 1
            };

            if (TowerObjectClassName == NetworkFactory.CameraObjectClassName)
            {
                item.TowerObject.PrefabObjectType = Enumenators.PrefabObjectType.Camera;
            }
            if (TowerObjectClassName == NetworkFactory.PadObjectClassName)
            {
                item.TowerObject.PrefabObjectType = Enumenators.PrefabObjectType.Tablet;
            }
            else if (TowerObjectClassName == NetworkFactory.MirrorObjectClassName)
            {
                item.TowerObject.PrefabObjectType = Enumenators.PrefabObjectType.Mirror;
            }

            item.TowerObject.Id = Guid.Empty;

            item.TowerObject.TowerObjectClassName = TowerObjectClassName;
            //item.TowerObject.BehaviorType = TowerObjectBehaviorType.Movable;
            item.TowerObject.RemoteObjectTypeInfo = RemoteObjectTypeInfo;

            item.TowerObject.Name = LabelLocalizationVariant.Localize();
            item.TowerObject.OwnerBusinessUnitType = BusinessUnitType.User;
            return item;
        }
    }
}
