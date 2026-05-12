using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace Sense.Interectable.Teleportation
{
    public abstract class CustomTeleportationInteractable : BaseTeleportationInteractable
    {
        [SerializeField] protected InputActionReference TeleportActivateR = null;
        [SerializeField] protected InputActionReference TeleportActivateL = null;


        /// <summary>
        /// The <see cref="Transform"/> that represents the teleportation destination.
        /// </summary>
        public abstract Transform TeleportAnchorTransform { get; set; }

        protected virtual void OnValidate() { }
        protected override void Reset() { }

        protected abstract override  bool GenerateTeleportRequest(IXRInteractor interactor, RaycastHit raycastHit, ref TeleportRequest teleportRequest);
    }
}