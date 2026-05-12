using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HookahController : MonoBehaviour
{
    [SerializeField]
    private XRBaseInteractable HookahXrGrabInteractable;
    [SerializeField]
    private InterectObjectGrabbable[] MouthpieceGrabbable;

#if !UNITY_SERVER
    private void Update()
    {
        foreach (var grab in MouthpieceGrabbable)
        {
            if (grab != null)
            grab.canGrab = HookahXrGrabInteractable.interactorsSelecting.Count == 0;
        }       
    }
#endif

}
