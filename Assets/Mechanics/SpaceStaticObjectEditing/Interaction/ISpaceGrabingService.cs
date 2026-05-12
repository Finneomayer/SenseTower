using Data;
using Mechanics.LoadSceneObjects.Models;
using Mechanics.SpaceStaticObjectEditing.Model;
using UnityEngine;

namespace Mechanics.SpaceStaticObjectEditing.Interaction
{
    public interface ISpaceGrabingService
    {
        public StaticObject StaticObject { get; }

        public void Init(Enumenators.PrefabObjectType towerPrefabType, SpaceStaticObjectModel gameObjectVisual);
        public void Save();
        public void SetInactiveObject();
    }
}