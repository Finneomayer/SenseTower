using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PadFollowingPlayer : MonoBehaviour
{
    [SerializeField] private Transform _pad;
    [SerializeField] private ZoneExpander _zoneExpander;
    [SerializeField] private XRGrabInteractable _padXR;

    private Vector3 shift;

    private void Start()
    {
        _zoneExpander = FindObjectOfType<ZoneExpander>();
        if (_zoneExpander == null)
        {
            return;
        }
        _zoneExpander.ZoneSizeChangingStart += ZoneExpander_ZoneSizeChangingStart;
        _zoneExpander.ZoneSizeChangingEnd += ZoneExpander_ZoneSizeChangingEnd;
    }

    private void OnDestroy()
    {
        if (_zoneExpander == null)
        {
            return;
        }
        _zoneExpander.ZoneSizeChangingStart -= ZoneExpander_ZoneSizeChangingStart;
        _zoneExpander.ZoneSizeChangingEnd -= ZoneExpander_ZoneSizeChangingEnd;
    }

    private void ZoneExpander_ZoneSizeChangingStart(Transform player)
    {
        Camera camera = player.GetComponentInChildren<Camera>();
        shift = camera.transform.position - _pad.position;
    }

    private void ZoneExpander_ZoneSizeChangingEnd(Transform player)
    {
        if (!_padXR.isHovered)
        {
            Camera camera = player.GetComponentInChildren<Camera>();
            _pad.position = camera.transform.position - shift;
        }
    }
}
