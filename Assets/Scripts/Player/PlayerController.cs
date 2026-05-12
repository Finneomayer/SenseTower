using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace Assets.Scripts.Player
{
    public class PlayerController: MonoBehaviour
    {
        [field: SerializeField]
        public InputDeviceCharacteristics InputDeviceCharacteristics { get; private set; }

        [field: SerializeField]
        public XRBaseControllerInteractor GrabInteractor { get; private set; }

        [field: SerializeField]
        public XRBaseControllerInteractor RayInteractor { get; private set; }

        [field: SerializeField]
        public Transform MarkerAnchor { get; private set; }

        [field: SerializeField]
        public GrabbingHand GrabbingHand { get; private set; }

        [SerializeField]
        private WrongPlayerZoneHapticsActivator HapticsActivator;

        [Space]
        [SerializeField] private Transform _anchorWhenControllerActive;
        [SerializeField] private Transform _anchorWhenHandTracking;

        public event Action<PlayerController> Enabled;
        public event Action<PlayerController> Disabled;

        private void Awake()
        {
            SetActiveHaptics(false);
        }

        private void OnEnable()
        {
            Enabled?.Invoke(this);
        }

        private void OnDisable()
        {
            Disabled?.Invoke(this);
        }

        public void SetActiveHaptics(bool active)
        {
            HapticsActivator.SetActiveHaptics(active);
        }

        public void SetControllerTracking(bool controllerActive)
        {
            if (controllerActive)
            {
                if (_anchorWhenControllerActive != null) GrabInteractor.attachTransform = _anchorWhenControllerActive;
                transform.localPosition = Vector3.zero;
            }
            else
            {
                if (_anchorWhenHandTracking != null) GrabInteractor.attachTransform = _anchorWhenHandTracking;

                if (InputDeviceCharacteristics.HasFlag(InputDeviceCharacteristics.Right))
                    transform.localPosition = Vector3.one * 1000;
                else if (InputDeviceCharacteristics.HasFlag(InputDeviceCharacteristics.Left))
                    transform.localPosition = Vector3.one * 1000;
            }
        }
    }
}
