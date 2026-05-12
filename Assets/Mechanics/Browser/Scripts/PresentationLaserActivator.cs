using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class PresentationLaserActivator : MonoBehaviour
{
    [SerializeField]
    private PresentationLaser[] PresentationLasers;

    public XRRayInteractor ActiveInteractor { get; private set; }

    private Dictionary<InputAction, Action<InputAction.CallbackContext>> _actionsHandlersMap;

    private void Awake()
    {
        _actionsHandlersMap = new();
        ActiveInteractor = PresentationLasers[0].PresentationInteractor;
    }

    private void OnEnable()
    {
        foreach (var item in PresentationLasers)
        {
            Action<InputAction.CallbackContext> handler = (e) => SetActiveInteractor(item.PresentationInteractor);
            item.LaserActivationInputAction.action.started += handler;
            _actionsHandlersMap.Add(item.LaserActivationInputAction.action, handler);
        }
    }

    private void OnDisable()
    {
        foreach (var item in _actionsHandlersMap)
        {
            item.Key.started -= item.Value;
        }
        _actionsHandlersMap.Clear();
    }

    public bool IsPresentationRay(XRRayInteractor ray)
    {
        foreach (var item in PresentationLasers)
        {
            if (item.PresentationInteractor == ray)
            {
                return true;
            }
        }
        return false;
    }

    private void SetActiveInteractor(XRRayInteractor xrInteractor)
    {
        ActiveInteractor = xrInteractor;
        foreach (var item in PresentationLasers)
        {
            item.PresentationInteractor.gameObject.SetActive(
                item.PresentationInteractor == ActiveInteractor);
        }
    }

    [Serializable]
    public class PresentationLaser
    {
        [field: SerializeField]
        public XRRayInteractor PresentationInteractor { get; private set; }

        [field: SerializeField]
        public XRInteractorLineVisual PresentationLineVisual { get; private set; }

        [field: SerializeField]
        public InputActionReference LaserActivationInputAction { get; private set; }

        [field: SerializeField]
        public XRInteractorLineVisual[] MoreSignificantLineVisuals { get; private set; }
    }
}
