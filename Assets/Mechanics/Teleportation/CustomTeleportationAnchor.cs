using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace Sense.Interectable.Teleportation
{
    public class CustomTeleportationAnchor : CustomTeleportationInteractable
    {

        private bool _isHovering = false;

        [SerializeField]
        [Tooltip("The Transform that represents the teleportation destination.")]
        Transform m_TeleportAnchorTransform;

        /// <summary>
        /// The <see cref="Transform"/> that represents the teleportation destination.
        /// </summary>
        public override Transform TeleportAnchorTransform
        {
            get => m_TeleportAnchorTransform;
            set => m_TeleportAnchorTransform = value;
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected override void OnValidate()
        {
            if (m_TeleportAnchorTransform == null)
                m_TeleportAnchorTransform = transform;
        }

        /// <inheritdoc />
        protected override void Reset()
        {
            base.Reset();
            m_TeleportAnchorTransform = transform;
        }

        /// <summary>
        /// Unity calls this when drawing gizmos.
        /// </summary>
        protected void OnDrawGizmos()
        {
            if (m_TeleportAnchorTransform == null)
                return;

            Gizmos.color = Color.blue;
            GizmoHelpers.DrawWireCubeOriented(m_TeleportAnchorTransform.position, m_TeleportAnchorTransform.rotation, 1f);

            GizmoHelpers.DrawAxisArrows(m_TeleportAnchorTransform, 1f);
        }

        /// <inheritdoc />
        protected override bool GenerateTeleportRequest(IXRInteractor interactor, RaycastHit raycastHit, ref TeleportRequest teleportRequest)
        {
            if (m_TeleportAnchorTransform == null || TeleportActivateR.action.inProgress || TeleportActivateL.action.inProgress)
                return false;

            teleportRequest.destinationPosition = m_TeleportAnchorTransform.position;
            teleportRequest.destinationRotation = m_TeleportAnchorTransform.rotation;
            return true;
        }

        protected override void OnHoverEntered(HoverEnterEventArgs args)
        {
            _isHovering = true;
            base.OnHoverEntered(args);
        }

        protected override void OnHoverExited(HoverExitEventArgs args)
        {
            _isHovering = false;
            base.OnHoverExited(args);
        }

    }
}