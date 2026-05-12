using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace UI
{
    public static class XrColliderHovering
    {
        public static bool IsHovering(IXRInteractor[] _interactors, BoxCollider HoveringCollider)
        {
            if (_interactors == null || _interactors.Length == 0)
            {
                return false;
            }

            bool isHovered = false;
            foreach (var interactor in _interactors)
            {
                if (IsHoveringByInteractor(interactor, HoveringCollider))
                {
                    isHovered = true;
                    break;
                }
            }

            return isHovered;
        }

        private static bool IsHoveringByInteractor(IXRInteractor interactor, BoxCollider hoveringCollider)
        {
            if (interactor == null) return false;
            if (hoveringCollider == null || !interactor.transform.gameObject.activeInHierarchy)
            {
                return false;
            }

            Ray ray = new Ray(interactor.transform.position, interactor.transform.forward);
            return hoveringCollider.Raycast(ray, out RaycastHit _, 10);
        }
    }
}