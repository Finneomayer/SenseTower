using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Assets.Localization;
using UnityEngine.EventSystems;

namespace Assets.Mechanics.Doors
{
    public class DoorAccessView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private ActiveDoor ActiveDoor;
        [SerializeField]
        private XRGrabInteractable XRGrabInteractable;
        [SerializeField]
        private GameObject BlockedDoor;
        [SerializeField]
        private GameObject OpenedDoor;
        [SerializeField]
        private GameObject PaidDoor;
        [SerializeField]
        private RectTransform AccessTextPanel;
        [SerializeField]
        private TMP_Text AccessText;
        
        [SerializeField] 
        private LocalizationVariant OpenedTextLocalizationVariant;
        [SerializeField]
        private LocalizationVariant ClosedTextLocalizationVariant;
        [SerializeField]
        private LocalizationVariant OwnerIsInSpaceTextLocalizationVariant;
        [SerializeField]
        private LocalizationVariant OwnerIsNotInSpaceTextLocalizationVariant;

        private Coroutine _refreshingAccessVisualizationRoutine;

        private Plane _textPanelPlane;
        private IXRHoverInteractor _currentHoverInteractor;
        private List<IXRHoverInteractor> _currentHoverInteractors = new();

        public bool IsHovering => _currentHoverInteractor != null;

        private void Awake()
        {
            _textPanelPlane = new Plane(AccessTextPanel.transform.forward, AccessTextPanel.transform.position);
        }

        private void OnEnable()
        {
            ActiveDoor.SpaceAvailabilityChanged += OnDoorAvailabilityChanged;
            XRGrabInteractable.hoverEntered.AddListener(OnHoverEntered);
            XRGrabInteractable.hoverExited.AddListener(OnHoverExited);
        }

        private void OnDisable()
        {
            ActiveDoor.SpaceAvailabilityChanged -= OnDoorAvailabilityChanged;
            XRGrabInteractable.hoverEntered.RemoveListener(OnHoverEntered);
            XRGrabInteractable.hoverExited.RemoveListener(OnHoverExited);
        }

        private void OnDoorAvailabilityChanged()
        {
            RefreshDoorVisualization();
        }

        public void OnHoverEntered(HoverEnterEventArgs args)
        {
            _currentHoverInteractors.Add(args.interactorObject);
            RefreshHover();
        }

        public void OnHoverExited(HoverExitEventArgs args)
        {
            _currentHoverInteractors.Remove(args.interactorObject);
            RefreshHover();
        }

        private void SetActiveAccessText(bool active)
        {
            if (AccessTextPanel == null)
            {
                return;
            }

            if (!active || ActiveDoor == null)
            {
                AccessTextPanel.gameObject.SetActive(false);
                return;
            }

            if (ActiveDoor.AccessData != null && !string.IsNullOrEmpty(ActiveDoor.AccessData.DoorMessage))
            {
                AccessText.text = ActiveDoor.AccessData.DoorMessage;
            }
            else
            {
                if (ActiveDoor.IsSpaceAvailable())
                {
                    AccessText.text = OpenedTextLocalizationVariant.Localize();
                }
                else
                {
                    AccessText.text = ClosedTextLocalizationVariant.Localize();
                }
            }

            AccessTextPanel.gameObject.SetActive(true);
        }

        private IEnumerator RefreshingTextPanelPositionRoutine(IXRHoverInteractor interactor)
        {
            if (!interactor.transform.TryGetComponent(out XRRayInteractor rayInteractor))
            {
                _refreshingAccessVisualizationRoutine = null;
                yield break;
            }

            while (true)
            {
                RefreshTextPanelPosition(rayInteractor);
                yield return new WaitForFixedUpdate();
            }

        }

        private void RefreshTextPanelPosition(XRRayInteractor rayInteractor)
        {
            if (!rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit raycastHit))
            {
                return;
            }

            Vector3 rayDirectionToOrigin = (rayInteractor.transform.position - raycastHit.point).normalized;
            Ray ray = new(raycastHit.point, rayDirectionToOrigin);

            if (!_textPanelPlane.Raycast(ray, out float intersectionDistance))
            {
                return;
            }

            AccessTextPanel.transform.position = raycastHit.point + intersectionDistance * rayDirectionToOrigin;
        }

        private void RefreshDoorVisualization()
        {
            SetActiveDoorsAccessVisualization(IsHovering);
            SetActiveAccessText(IsHovering && ActiveDoor.IsPrivatePlace);
        }

        private void RefreshHover()
        {
            _currentHoverInteractor = _currentHoverInteractors.Count > 0 ? _currentHoverInteractors[0] : null;

            if (_refreshingAccessVisualizationRoutine != null)
            {
                StopCoroutine(_refreshingAccessVisualizationRoutine);
                _refreshingAccessVisualizationRoutine = null;
            }

            RefreshDoorVisualization();

            if (_currentHoverInteractor != null && ActiveDoor.IsPrivatePlace)
            {
                _refreshingAccessVisualizationRoutine =
                    StartCoroutine(RefreshingTextPanelPositionRoutine(_currentHoverInteractor));
            }
        }

        private void SetActiveDoorsAccessVisualization(bool active)
        {
            if (active)
            {
                ShowDoorAccessVisualization();
            }
            else
            {
                HideDoorAccessVisualization();
            }
        }

        private void ShowDoorAccessVisualization()
        {
            if (ActiveDoor.IsSpaceAvailable())
            {
                OpenedDoor.SetActive(true);
                BlockedDoor.SetActive(false);
                PaidDoor.SetActive(false);
            }
            else
            {
                OpenedDoor.SetActive(false);
                BlockedDoor.SetActive(true);
                PaidDoor.SetActive(false);
            }

            if(ActiveDoor.AccessData is {CanBeHere: false})
            {
                if(ActiveDoor.IsPaidDoor)
                {
                    OpenedDoor.SetActive(false);
                    BlockedDoor.SetActive(false);
                    PaidDoor.SetActive(true);
                }
            }
        }

        private void HideDoorAccessVisualization()
        {
            if (OpenedDoor != null)
            {
                OpenedDoor.SetActive(false);
            }
            if (BlockedDoor != null)
            {
                BlockedDoor.SetActive(false);
            }
            if (PaidDoor != null)
            {
                PaidDoor.SetActive(false);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            SetActiveDoorsAccessVisualization(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SetActiveDoorsAccessVisualization(false);
        }
    }
}