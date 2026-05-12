using System;
using UnityEngine;

namespace Assets.Mechanics.AirLocomotion.Scripts
{
    [RequireComponent(typeof(Collider))]
    public class AirLocomotionZoneVisualizerActivator : MonoBehaviour
    {
        public event Action<AirLocomotionZoneVisualizerActivator> Enabled;
        public event Action<AirLocomotionZoneVisualizerActivator> Disabled;
            
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