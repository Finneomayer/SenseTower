using System.Collections.Generic;
using Data;
using Mechanics.LoadSceneObjects.Models;
using Mechanics.SpaceStaticObjectEditing.Interaction;
using UnityEngine;

namespace Mechanics.SpaceStaticObjectEditing
{
    public interface ISpaceRepository
    {
        public void Save(Enumenators.PrefabObjectType prefabType, ISpaceGrabingService obj);

        public List<StaticObject> GetSpaceObjectsList();
    }
}