using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Mechanics.Inventory
{
    public class InventoryObjectGrabbing : MonoBehaviour
    {
        [SerializeField]
        private XRGrabInteractable XrGrabInteractable;

        public bool IsGrabbing { get; private set; }
        public bool FirstContact { get; private set; }

        private List<IXRSelectInteractor> _currentGrabbingInteractors;

        private void Awake()
        {
            _currentGrabbingInteractors = new();
        }

        private void OnDestroy()
        {
            if (XrGrabInteractable != null)
            {
                Destroy(XrGrabInteractable.gameObject);
            }
        }

        private void OnEnable()
        {
            XrGrabInteractable.selectEntered.AddListener(OnGrabStarted);
            XrGrabInteractable.selectExited.AddListener(OnGrabStopped);
        }

        private void OnDisable()
        {
            XrGrabInteractable.selectEntered.RemoveListener(OnGrabStarted);
            XrGrabInteractable.selectExited.RemoveListener(OnGrabStopped);
        }

        private void OnGrabStarted(SelectEnterEventArgs grabEnterArgs)
        {
            if (!_currentGrabbingInteractors.Contains(grabEnterArgs.interactorObject))
            {
                _currentGrabbingInteractors.Add(grabEnterArgs.interactorObject);
                IsGrabbing = true;
                FirstContact = true;
            }
        }

        private void OnGrabStopped(SelectExitEventArgs grabExitArgs)
        {
            _currentGrabbingInteractors.Remove(grabExitArgs.interactorObject);
            if (_currentGrabbingInteractors.Count == 0)
            {
                IsGrabbing = false;
            }
        }

    }
}