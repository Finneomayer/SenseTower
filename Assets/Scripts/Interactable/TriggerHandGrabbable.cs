using Data;

namespace Assets.Scripts.Interactable
{
    public class TriggerHandGrabbable : HandGrabbable
    {
        protected override void RegisterHandListeners(GrabbingHand grabbingHand)
        {
            grabbingHand.TriggerStarted += OnGrabbingHandGrabbingStarted;
            grabbingHand.TriggerStopped += OnGrabbingHandGrabbingStopped;
        }

        protected override void UnregisterHandListeners(GrabbingHand grabbingHand)
        {
            grabbingHand.TriggerStarted -= OnGrabbingHandGrabbingStarted;
            grabbingHand.TriggerStopped -= OnGrabbingHandGrabbingStopped;
        }

        protected override void InGrabbingProgress(GrabbingHand grabbingHand)
        {
            if (grabbingHand.IsTriggerInProgress) 
                OnGrabbingHandGrabbingStarted(grabbingHand);
        }

        protected override void StartMovingWithMovementType(GrabbingHand grabbingHand,
            Enumenators.MovementType movementType)
        {
            base.StartMovingWithMovementType(grabbingHand, Enumenators.MovementType.TriggerMove);
        }
    }
}