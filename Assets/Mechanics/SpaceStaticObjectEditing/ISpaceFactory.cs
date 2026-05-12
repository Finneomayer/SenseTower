using Data;
using Mechanics.LoadSceneObjects.Models;
using Mechanics.SpaceStaticObjectEditing.Model;
using UnityEngine;

namespace Mechanics.SpaceStaticObjectEditing
{
    public interface ISpaceFactory
    {
        public StaticObject[] GetExistModelPrefabs();
        public SpaceStaticObjectModel CreateObjectVisualize(string id);
        public GameObject CreateSpaceEditingObject(string uid, Vector3 position,Vector3 offset, GrabbingHand grabbingHand = default);
        public GameObject CreateObjectWithParametrs(string id);
    }
}