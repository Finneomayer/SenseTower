using Assets.Scripts.Zones;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportController : MonoBehaviour
{
    [SerializeField] protected InputActionReference TeleportActivate = null;
    [SerializeField] protected GameObject TeleportFloor;
    [SerializeField] protected GameObject TeleportPlace;

    //needs to listen OnHovering state of Distance interactor to disable teleport when UI hovering,
    //because og Unity bug of not working OnHover event on UI
    //https://stackoverflow.com/questions/60788605/xr-interaction-toolkit-hover-event-on-ui-elements-with-xrrayinteractor
    [SerializeField] private XRInteractorLineVisual[] _lineVisuals;

#if !UNITY_SERVER

    private ZonesModel _zonesModel;

    private void Awake()
    {
        _zonesModel = FindObjectOfType<ZonesModel>();
    }

    private void Update()
    {
        bool lineVisualIsShowing = false;
        foreach (var lineVisual in _lineVisuals)
        {
            if (lineVisual.reticle.activeInHierarchy)
            {
                lineVisualIsShowing = true;
                break;
            }
        }

        bool isFloorLocked = _zonesModel != null && _zonesModel.ZoneController != null 
            && _zonesModel.ZoneController.IsLocked && !_zonesModel.ZoneController.IsLocalUserAdmin;

        TeleportFloor.SetActive(!isFloorLocked && TeleportActivate.action.inProgress && !lineVisualIsShowing);
        TeleportPlace.SetActive(!lineVisualIsShowing);
    }
#endif
}
