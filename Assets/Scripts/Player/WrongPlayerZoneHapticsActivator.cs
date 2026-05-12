using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Assets.Scripts.Player
{
    [RequireComponent(typeof(Collider))]
    public class WrongPlayerZoneHapticsActivator : MonoBehaviour
    {
        [SerializeField]
        private XRBaseControllerInteractor Interactor;
        [SerializeField]
        private float VibrationIntensity;

        private HashSet<Collider> _wrongZoneCollidersEnteredInTrigger;
        private HashSet<Collider> _wrongZoneCollidersStayingInTrigger;

        private void Awake()
        {
            _wrongZoneCollidersEnteredInTrigger = new HashSet<Collider>();
            _wrongZoneCollidersStayingInTrigger = new HashSet<Collider>();
        }

        private void FixedUpdate()
        {
            if (_wrongZoneCollidersStayingInTrigger.Count > 0)
            {
                Interactor.SendHapticImpulse(VibrationIntensity, 0.1f);
                _wrongZoneCollidersStayingInTrigger.Clear();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out WrongPlayerZoneCollider _))
            {
                return;
            }

            _wrongZoneCollidersEnteredInTrigger.Add(other);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out WrongPlayerZoneCollider _))
            {
                return;
            }

            _wrongZoneCollidersEnteredInTrigger.Remove(other);
            _wrongZoneCollidersStayingInTrigger.Remove(other);
        }

        private void OnTriggerStay(Collider other)
        {
            if (_wrongZoneCollidersEnteredInTrigger.Contains(other)
                && !_wrongZoneCollidersStayingInTrigger.Contains(other))
            {
                _wrongZoneCollidersStayingInTrigger.Add(other);
            }
        }

        public void SetActiveHaptics(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}