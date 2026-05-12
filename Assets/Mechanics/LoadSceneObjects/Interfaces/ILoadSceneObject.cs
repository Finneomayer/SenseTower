using System;
using UnityEngine;

namespace Mechanics.LoadSceneObjects.Interfaces
{
    public interface ILoadSceneObject
    {
        public event Action<bool> OnCreate;
        public void Init(string path);
        public void Create(string key, Vector3 position, Vector3 rotation = default, Vector3 scale = default);
    }
}