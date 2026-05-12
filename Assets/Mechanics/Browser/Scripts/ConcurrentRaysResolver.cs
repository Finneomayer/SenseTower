using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;

namespace Assets.Scripts.Player
{
    public class ConcurrentRaysResolver : MonoBehaviour
    {
        [field: SerializeField]
        public XRRayInteractor[] RightXRRays { get; private set; }

        [field: SerializeField]
        public XRRayInteractor[] LeftXRRays { get; private set; }

        private XRRayInteractorData[] _rightXRRaysData;
        private XRRayInteractorData[] _leftXRRaysData;

        private void Awake()
        {
            _rightXRRaysData = new XRRayInteractorData[RightXRRays.Length];
            for (int i = 0; i < RightXRRays.Length; i++)
            {
                _rightXRRaysData[i] = new(RightXRRays[i]);
            }
            _leftXRRaysData = new XRRayInteractorData[LeftXRRays.Length];
            for (int i = 0; i < LeftXRRays.Length; i++)
            {
                _leftXRRaysData[i] = new(LeftXRRays[i]);
            }
        }

        private void Update()
        {
            RefreshRays(_rightXRRaysData);
            RefreshRays(_leftXRRaysData);
        }

        private void RefreshRays(XRRayInteractorData[] xrRays)
        {
            XRRayInteractorData closestRay = null;
            float minRayDistance = float.MaxValue;
            foreach (var xrRay in xrRays)
            {
                if (!xrRay.Interactor.gameObject.activeInHierarchy)
                {
                    continue;
                }

                if (!xrRay.Interactor.TryGetCurrentRaycast(out RaycastHit? hitInfo, out int raycastHitIndex,
                    out RaycastResult? uiRaycastHit, out int uiRaycastHitIndex, out bool isUiHitClosest))
                {
                    continue;
                }

                float rayDistance;
                if (uiRaycastHit.HasValue && isUiHitClosest)
                {
                    rayDistance = uiRaycastHit.Value.distance;
                }
                else if (hitInfo.HasValue
                    && xrRay.Interactor.interactablesHovered.Count > 0)
                {
                    rayDistance = hitInfo.Value.distance;
                }
                else
                {
                    continue;
                }

                if (rayDistance < minRayDistance)
                {
                    closestRay = xrRay;
                    minRayDistance = rayDistance;
                }
            }

            foreach (var xrRay in xrRays)
            {
                SetEnabledRay(xrRay, xrRay == closestRay);
            }
        }

        private void SetEnabledRay(XRRayInteractorData ray, bool enabled)
        {
            ray.LineVisual.enabled = enabled;
            ray.XrController.enabled = enabled;
            if (!enabled)
            {
                IXRSelectInteractable[] selectInteractables = ray.Interactor.interactablesSelected.ToArray();
                foreach (var interactable in selectInteractables)
                {
                    ray.Interactor.interactionManager.SelectExit(ray.Interactor, interactable);
                }
                IXRHoverInteractable[] hoverInteractables = ray.Interactor.interactablesHovered.ToArray();
                foreach (var interactable in hoverInteractables)
                {
                    ray.Interactor.interactionManager.HoverExit(ray.Interactor, interactable);
                }
            }


        }

        #region Internal class
        private class XRRayInteractorData
        {
            public XRRayInteractor Interactor { get; }
            public XRInteractorLineVisual LineVisual { get; }
            public XRBaseController XrController { get; }

            public XRRayInteractorData(XRRayInteractor interactor)
            {
                Interactor = interactor;
                LineVisual = interactor.GetComponent<XRInteractorLineVisual>();
                XrController = interactor.GetComponent<XRBaseController>();
            }
        }
        #endregion
    }
}