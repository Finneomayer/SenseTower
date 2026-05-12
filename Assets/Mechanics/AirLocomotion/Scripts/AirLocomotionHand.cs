using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Mechanics.AirLocomotion.Scripts
{
    public class AirLocomotionHand : MonoBehaviour
    {
        [field: SerializeField]
        public InputActionReference LocomotionInputAction { get; private set; }

        private bool _airLocomotionEnabled;
        private List<AirLocomotionZone> _currentLocomotionZones;

        public Vector3 CurrentPosition => transform.position;

        public event Action<AirLocomotionHand> AirLocomotionStarted;
        public event Action<AirLocomotionHand> AirLocomotionStopped;

        private void Awake()
        {
            _currentLocomotionZones = new();
        }

        private void OnEnable()
        {
            LocomotionInputAction.action.started += OnAirLocomotionActionStarted;
            LocomotionInputAction.action.canceled += OnAirLocomotionActionCanceled;
        }

        private void OnDisable()
        {
            LocomotionInputAction.action.started -= OnAirLocomotionActionStarted;
            LocomotionInputAction.action.canceled -= OnAirLocomotionActionCanceled;
        }

        public void ForseStopLocomotion(AirLocomotionZone airLocomotionZone)
        {
            _currentLocomotionZones.Remove(airLocomotionZone);
            if (_currentLocomotionZones.Count == 0)
            {
                _airLocomotionEnabled = false;
                InvokeStopLocomotionEvent();
            }
        }

        private void OnAirLocomotionActionStarted(InputAction.CallbackContext obj)
        {
            if (!_airLocomotionEnabled)
            {
                return;
            }
            AirLocomotionStarted?.Invoke(this);
        }

        private void OnAirLocomotionActionCanceled(InputAction.CallbackContext obj)
        {
            InvokeStopLocomotionEvent();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out AirLocomotionZone airLocomotionZone))
            {
                return;
            }

            if (!_currentLocomotionZones.Contains(airLocomotionZone))
            {
                _currentLocomotionZones.Add(airLocomotionZone);
                _airLocomotionEnabled = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out AirLocomotionZone airLocomotionZone))
            {
                return;
            }

            ForseStopLocomotion(airLocomotionZone);
        }

        private void InvokeStopLocomotionEvent()
        {
            AirLocomotionStopped?.Invoke(this);
        }
    }
}
