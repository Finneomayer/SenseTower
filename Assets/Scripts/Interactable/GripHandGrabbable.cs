namespace Assets.Scripts.Interactable
{
    public class GripHandGrabbable : HandGrabbable
    {
        protected override void RegisterHandListeners(GrabbingHand grabbingHand)
        {
            grabbingHand.GrabbingStarted += OnGrabbingHandGrabbingStarted;
            grabbingHand.GrabbingStopped += OnGrabbingHandGrabbingStopped;
        }

        protected override void UnregisterHandListeners(GrabbingHand grabbingHand)
        {
            grabbingHand.GrabbingStarted -= OnGrabbingHandGrabbingStarted;
            grabbingHand.GrabbingStopped -= OnGrabbingHandGrabbingStopped;
        }

        protected override void InGrabbingProgress(GrabbingHand grabbingHand)
        {
            if (grabbingHand.IsGrabbingInProgress) 
                OnGrabbingHandGrabbingStarted(grabbingHand);
        }
    }
}