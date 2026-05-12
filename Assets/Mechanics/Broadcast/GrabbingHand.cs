using Oculus.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Interactable;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SphereCollider))]
public sealed class GrabbingHand : MonoBehaviour
{
    [SerializeField] private Transform GrabbableObjectAnchor;
    [SerializeField] private InputActionReference GrabbingInputAction;
    [SerializeField] private InputActionReference TriggeringInputAction;

    public Vector3 CurrentObjectAnchorPosition => GrabbableObjectAnchor.position;
    public Quaternion CurrentObjectAnchorRotation => GrabbableObjectAnchor.rotation;
    public List<InventoryObject> InventoryObjects => _inventoryObjects;
    private StaticObjectHandGrabbable _triggerObjectInHand;
    public bool HandBusy { get; private set; }
    public bool IsGrabbingInProgress => GrabbingInputAction.action.inProgress;
    public bool IsTriggerInProgress => TriggeringInputAction.action.inProgress;

    private List<InventoryObject> _inventoryObjects = new();
    
    private Pose _initialAnchorLocalPose;
    private SphereCollider _handCollider;
    public event Action<GrabbingHand> GrabbingStarted;
    public event Action<GrabbingHand> GrabbingStopped;

    public event Action<GrabbingHand> TriggerStarted;
    public event Action<GrabbingHand> TriggerStopped;

    private void Awake()
    {
        _initialAnchorLocalPose = GrabbableObjectAnchor.GetPose(Space.Self);

        _handCollider = GetComponent<SphereCollider>();
    }

    private void OnEnable()
    {
        GrabbingInputAction.action.started += OnGrabbingInputActionStarted;
        GrabbingInputAction.action.canceled += OnGrabbingInputActionCanceled;
        TriggeringInputAction.action.started += OnTriggerStarted;
        TriggeringInputAction.action.canceled += OnTriggerCanceled;
        StartCoroutine(RefreshInventoryObjectsRoutine());
    }

    private void OnDisable()
    {
        GrabbingInputAction.action.started -= OnGrabbingInputActionStarted;
        GrabbingInputAction.action.canceled -= OnGrabbingInputActionCanceled;
        
        TriggeringInputAction.action.started -= OnTriggerStarted;
        TriggeringInputAction.action.canceled -= OnTriggerCanceled;

        StopAllCoroutines();
    }

    public bool IsCurrentObjectInHandOrNull(StaticObjectHandGrabbable triggerGrabInteractable)
    {
        if (_triggerObjectInHand == null)
            return true;
        
        if (_triggerObjectInHand == triggerGrabInteractable)
            return true;

        return false;
    }

    public void SetObjectInHand(StaticObjectHandGrabbable triggerGrabInteractable)
    {
        _triggerObjectInHand = triggerGrabInteractable;
    }

    private IEnumerator RefreshInventoryObjectsRoutine()
    {
        WaitForSeconds waitForSeconds = new(0.1f);
        while (true)
        {
            yield return waitForSeconds;

            if (_inventoryObjects == null)
            {
                yield break;
            }

            _inventoryObjects.Clear();

            if (IsGrabbingInProgress || IsTriggerInProgress)
            {
                Collider[] colliders = Physics.OverlapSphere(_handCollider.transform.position + _handCollider.center,
                    _handCollider.radius);

                foreach (var item in colliders)
                {
                    if (item.gameObject.TryGetComponent(out InventoryObjectCollider inventoryObjectCollider))
                    {
                        if (!_inventoryObjects.Contains(inventoryObjectCollider.Object))
                        {
                            _inventoryObjects.Add(inventoryObjectCollider.Object);
                        }
                    }
                }
            }
        }
    }

    public void StartGrabbing(Vector3 position, Quaternion rotation)
    {
        GrabbableObjectAnchor.transform.SetPositionAndRotation(position, rotation);
    }

    public void IsUnteract(bool value)
    {
        HandBusy = value;
    }

    private void OnTriggerCanceled(InputAction.CallbackContext obj)
    {
        TriggerStopped?.Invoke(this);
        GrabbableObjectAnchor.SetPose(_initialAnchorLocalPose, Space.Self);
    }

    private void OnTriggerStarted(InputAction.CallbackContext obj)
    {
        TriggerStarted?.Invoke(this);
    }
    
    private void OnGrabbingInputActionStarted(InputAction.CallbackContext obj)
    {
        GrabbingStarted?.Invoke(this);
    }

    private void OnGrabbingInputActionCanceled(InputAction.CallbackContext obj)
    {
        GrabbableObjectAnchor.SetPose(_initialAnchorLocalPose, Space.Self);
        GrabbingStopped?.Invoke(this);
    }
}