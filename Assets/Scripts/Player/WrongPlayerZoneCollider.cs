using System;
using UnityEngine;

namespace Assets.Scripts.Player
{
    [RequireComponent(typeof(Collider))]
    public class WrongPlayerZoneCollider : MonoBehaviour
    {
        public event Action<WrongPlayerZoneCollider> Enabled;
        public event Action<WrongPlayerZoneCollider> Disabled;

        private void OnEnable()
        {
            Enabled?.Invoke(this);
        }

        private void OnDisable()
        {
            Disabled?.Invoke(this);
        }
    }
}