using Assets.Scripts.Interactable;
using UI;
using UnityEngine;

public class RecordingCameraGrabbable : GripHandGrabbable, IInventoryObjectGrabbable
{
    public Transform Transform => transform;
}
