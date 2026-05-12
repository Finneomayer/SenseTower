using System;
using UnityEngine;

namespace Assets.Mechanics.AirLocomotion.Scripts
{
    public class AirLocomotionZoneVisualizerCollider : MonoBehaviour
    {
        private const float IncreaseValue = 0.5f;

        public event Action<AirLocomotionZoneVisualizerActivator> Entered;
        public event Action<AirLocomotionZoneVisualizerActivator> Exited;

        private AirLocomotionZoneVisualizerActivator _activator;

        private void OnDisable()
        {
            if (_activator != null)
            {
                Exited?.Invoke(_activator);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out AirLocomotionZoneVisualizerActivator activator))
            {
                return;
            }

            _activator = activator;

            Entered?.Invoke(activator);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out AirLocomotionZoneVisualizerActivator activator))
            {
                return;
            }

            Exited?.Invoke(activator);
        }

        public void Init(AirLocomotionZone airLocomotionZone)
        {
            if (!TryGetComponent(out BoxCollider collider))
            {
                collider = gameObject.AddComponent<BoxCollider>();
            }

            collider.size = new Vector3(1, 1, 1);
            collider.center = airLocomotionZone.Collider.center;
            collider.isTrigger = true;

            BoxCollider airLocomotionZoneCollider = airLocomotionZone.Collider;

            float sizeX = (airLocomotionZoneCollider.size.x * airLocomotionZoneCollider.transform.lossyScale.x +
                           IncreaseValue)
                          / transform.lossyScale.x;
            float sizeY = (airLocomotionZoneCollider.size.y * airLocomotionZoneCollider.transform.lossyScale.y +
                           IncreaseValue)
                          / transform.lossyScale.y;
            float sizeZ = (airLocomotionZoneCollider.size.z * airLocomotionZoneCollider.transform.lossyScale.z +
                           IncreaseValue)
                          / transform.lossyScale.z;

            transform.position = airLocomotionZoneCollider.transform.position;
            transform.localScale = new Vector3(sizeX, sizeY, sizeZ);
        }
    }
}