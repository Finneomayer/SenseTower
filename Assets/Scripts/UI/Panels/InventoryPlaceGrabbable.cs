using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace UI
{
    [RequireComponent(typeof(Collider))]
    public sealed class InventoryPlaceGrabbable : MonoBehaviour
    {
        private List<GrabbingHand> _grabbingHands = new();
        private GrabbingHand _currentGrabbingHand;

        public bool AreHandsInPlace => _grabbingHands != null && _grabbingHands.Count > 0;
        public GrabbingHand LastGrabbingHand => _currentGrabbingHand;
        public List<GrabbingHand> GrabbingHands => _grabbingHands;

        public Action<GrabbingHand> GrabStarted;
        public Action<GrabbingHand> GrabStopped;
        public event Action HandInPlaceChanged;

        private void OnEnable()
        {
            foreach (var grabbingHand in _grabbingHands)
            {
                RegisterHandListeners(grabbingHand);
            }
            HandInPlaceChanged?.Invoke();
        }

        private void OnDisable()
        {
            foreach (var grabbingHand in _grabbingHands)
            {
                UnregisterHandListeners(grabbingHand);
            }
            _grabbingHands.Clear();
            _currentGrabbingHand = null;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out GrabbingHand grabbingHand))
            {
                return;
            }

            if (_grabbingHands.Contains(grabbingHand))
            {
                return;
            }

            _grabbingHands.Add(grabbingHand);
            RegisterHandListeners(grabbingHand);

            HandInPlaceChanged?.Invoke();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out GrabbingHand grabbingHand))
            {
                return;
            }

            _grabbingHands.Remove(grabbingHand);
            UnregisterHandListeners(grabbingHand);

            HandInPlaceChanged?.Invoke();
        }

        public void RaiseGrab(GrabbingHand grabbingHand)
        {
            OnGrabbingHandGrabbingStarted(grabbingHand);
        }

        private void RegisterHandListeners(GrabbingHand grabbingHand)
        {
            grabbingHand.GrabbingStarted += OnGrabbingHandGrabbingStarted;
            grabbingHand.GrabbingStopped += OnGrabbingHandGrabbingStopped;
            
            grabbingHand.TriggerStarted += OnGrabbingHandGrabbingStarted;
            grabbingHand.TriggerStopped += OnGrabbingHandGrabbingStopped;
        }

        private void UnregisterHandListeners(GrabbingHand grabbingHand)
        {
            grabbingHand.GrabbingStarted -= OnGrabbingHandGrabbingStarted;
            grabbingHand.GrabbingStopped -= OnGrabbingHandGrabbingStopped;
            
            grabbingHand.TriggerStarted -= OnGrabbingHandGrabbingStarted;
            grabbingHand.TriggerStopped -= OnGrabbingHandGrabbingStopped;
        }

        private void OnGrabbingHandGrabbingStarted(GrabbingHand grabbingHand)
        {
            _currentGrabbingHand = grabbingHand;
            GrabStarted?.Invoke(grabbingHand);
        }

        private void OnGrabbingHandGrabbingStopped(GrabbingHand grabbingHand)
        {
            if (grabbingHand == _currentGrabbingHand)
            {
                _currentGrabbingHand = null;
                foreach (var item in _grabbingHands)
                {
                    if (item.IsGrabbingInProgress || item.IsTriggerInProgress && item != grabbingHand)
                    {
                        _currentGrabbingHand = item;
                    }
                }
            }

            GrabStopped?.Invoke(grabbingHand);
        }
    }
}
