using Oculus.Avatar2;
using UnityEngine;

namespace Assets.Scripts.Interactable
{
    public class StaticObjectHandGrabbable : TriggerHandGrabbable
    {
        protected override void OnGrabbingHandGrabbingStarted(GrabbingHand grabbingHand)
        {
            if (IsGrabbing || grabbingHand.HandBusy)
                return;

            grabbingHand.SetObjectInHand(this);
            grabbingHand.IsUnteract(true);
            base.OnGrabbingHandGrabbingStarted(grabbingHand);
        }

        protected override void OnGrabbingHandGrabbingStopped(GrabbingHand grabbingHand)
        {
            if (grabbingHand != null)
            {
                grabbingHand.SetObjectInHand(null);
                grabbingHand.IsUnteract(false);
            }
            
            base.OnGrabbingHandGrabbingStopped(grabbingHand);
        }
    }
}