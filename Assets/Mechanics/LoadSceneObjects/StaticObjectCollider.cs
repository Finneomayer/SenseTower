using Mechanics.LoadSceneObjects.Models;
using UnityEngine;

namespace Mechanics.LoadSceneObjects
{
    [RequireComponent(typeof(Collider))]
    public class StaticObjectCollider : MonoBehaviour
    {
        public StaticObject StaticObject { get; private set; }

        public void Init(StaticObject staticObject)
        {
            StaticObject = staticObject;
        }
    }
}