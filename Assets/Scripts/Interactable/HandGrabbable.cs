using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

namespace Assets.Scripts.Interactable
{
    [RequireComponent(typeof(Collider))]
    public abstract class HandGrabbable : MonoBehaviour
    {
        [SerializeField] private bool AllowXRotation = true;
        [SerializeField] private bool AllowYRotation = true;
        [SerializeField] private bool AllowZRotation = true;

        private List<GrabbingHand> _grabbingHands;
        private Coroutine _grabbingRoutine;
        private GrabbingHand _currentGrabbingHand;
        private Vector3 _initialEulerRotation;

        public bool IsGrabbing => _grabbingRoutine != null;
        public GrabbingHand CurrentGrabbingHand => _currentGrabbingHand;

        public event Action GrabStarted;
        public event Action GrabStopped;

        private void Awake()
        {
            _grabbingHands = new();
            _initialEulerRotation = transform.eulerAngles;
        }

        private void OnEnable()
        {
            foreach (var grabbingHand in _grabbingHands)
            {
                RegisterHandListeners(grabbingHand);
            }
        }

        private void OnDisable()
        {
            OnGrabbingHandGrabbingStopped(_currentGrabbingHand);
            foreach (var grabbingHand in _grabbingHands)
            {
                UnregisterHandListeners(grabbingHand);
            }
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
            InGrabbingProgress(grabbingHand);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out GrabbingHand grabbingHand))
            {
                return;
            }

            _grabbingHands.Remove(grabbingHand);
            UnregisterHandListeners(grabbingHand);
        }
        
        [ContextMenu("Grab StartIn Editor")]
        private void HandGrad()
        {
            GrabStarted?.Invoke();
            GrabStopped?.Invoke();
        }

        protected abstract void RegisterHandListeners(GrabbingHand grabbingHand);

        protected abstract void UnregisterHandListeners(GrabbingHand grabbingHand);

        protected abstract void InGrabbingProgress(GrabbingHand grabbingHand);

        protected virtual void StartMovingWithMovementType(GrabbingHand grabbingHand,
            Enumenators.MovementType movementType = Enumenators.MovementType.GrabbingMove)
        {
            _grabbingRoutine = StartCoroutine(GrabbingRoutine(grabbingHand,movementType));
        }

        protected virtual void OnGrabbingHandGrabbingStarted(GrabbingHand grabbingHand)
        {
            if (_grabbingRoutine != null)
            {
                StopCoroutine(_grabbingRoutine);
            }

            StartMovingWithMovementType(grabbingHand);
            _currentGrabbingHand = grabbingHand;

            GrabStarted?.Invoke();
        }

        protected virtual void OnGrabbingHandGrabbingStopped(GrabbingHand grabbingHand)
        {
            if (grabbingHand != _currentGrabbingHand)
            {
                return;
            }

            if (_grabbingRoutine != null)
            {
                StopCoroutine(_grabbingRoutine);
                _grabbingRoutine = null;
            }

            GrabStopped?.Invoke();
        }

        private IEnumerator GrabbingRoutine(GrabbingHand grabbingHand, Enumenators.MovementType movementType)
        {
            grabbingHand.StartGrabbing(transform.position, transform.rotation);
            while (true)
            {
                yield return new WaitForFixedUpdate();
                bool isMovingProgress = movementType == Enumenators.MovementType.GrabbingMove
                    ? grabbingHand.IsGrabbingInProgress
                    : grabbingHand.IsTriggerInProgress;

                transform.position = grabbingHand.CurrentObjectAnchorPosition;

                Vector3 eulerRotation = grabbingHand.CurrentObjectAnchorRotation.eulerAngles;
                if (!AllowXRotation)
                {
                    eulerRotation.x = _initialEulerRotation.x;
                }

                if (!AllowYRotation)
                {
                    eulerRotation.y = _initialEulerRotation.y;
                }

                if (!AllowZRotation)
                {
                    eulerRotation.z = _initialEulerRotation.z;
                }

                transform.rotation = Quaternion.Euler(eulerRotation);
                
                if (!isMovingProgress)
                {
                    OnGrabbingHandGrabbingStopped(grabbingHand);
                }
            }
        }
    }
}