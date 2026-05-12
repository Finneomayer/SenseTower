using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace Assets.Mechanics.PadKeyboard
{

    public enum Axis
    {
        Z = 0,
        X = 1,
        Y = 2
    }

    public class FingerPhysicButton : MonoBehaviour
    {
        [SerializeField] private Axis _axis = Axis.Z;
        [SerializeField] private float _depth = 0.06f;
        [SerializeField] private GameObject _visualButton;
        [SerializeField] private string _fingerTag;
        [SerializeField] RaycastButton _raycastButton;

        public event Action OnHoverEnter;
        public event Action OnHoverExit;

        private const float TimeDelay = 0.3f;

        private float _axisMin = 0.0f;
        private float _axisMax = 0.0f;
        private bool _previousPress = false;

        private float _previousHandHeight = 0.0f;
        private GameObject _hoverInteractor = null;

        private bool _isPressing;
        private float _lastPressTime;

        private TrackedDeviceGraphicRaycaster _raycaster;

        public event Action OnPress;

        private void Awake()
        {
            _raycaster = GetComponentInChildren<TrackedDeviceGraphicRaycaster>();
            if (_raycaster != null)
            {
                _raycaster.ignoreReversedGraphics = true;
            }

            if (_raycastButton == null)
            {
                _raycastButton = GetComponentInChildren<RaycastButton>();
            }
        }

        private void OnEnable()
        {
            if (_depth == 0)
            {
                switch (_axis)
                {
                    case Axis.Z:
                        _depth = transform.parent.localScale.z;
                        break;
                    case Axis.X:
                        _depth = transform.parent.localScale.x;
                        break;
                    case Axis.Y:
                        _depth = transform.parent.localScale.y;
                        break;
                }
            }

            SetMinMax();

            if (_raycastButton != null)
            {
                _raycastButton.OnClick += OnRaycastButtonClick;
                _raycastButton.OnHoverEnter += OnRaycastButtonHoverEnter;
                _raycastButton.OnHoverExit += OnRaycastButtonHoverExit;
            }
        }

        private void OnDisable()
        {
            if (_raycastButton != null)
            {
                _raycastButton.OnClick -= OnRaycastButtonClick;
                _raycastButton.OnHoverEnter -= OnRaycastButtonHoverEnter;
                _raycastButton.OnHoverExit -= OnRaycastButtonHoverExit;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_hoverInteractor != null)
            {
                return;
            }

            if (!other.CompareTag(_fingerTag))
            {
                return;
            }

            _hoverInteractor = other.gameObject;
            if (!IsForwardInteraction(other.transform))
            {
                return;
            }

            StartPress(other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            if (_hoverInteractor != null && other.gameObject == _hoverInteractor.gameObject &&
                other.CompareTag(_fingerTag))
            {
                _hoverInteractor = null;
                if (_isPressing)
                {
                    EndPress(other.gameObject);
                }
            }
        }

        private void OnTriggerStay()
        {
            if (!_isPressing) return;
            var newHandHeight = GetLocalPosition(_hoverInteractor.transform.position);
            var handDifference = _previousHandHeight - newHandHeight;
            _previousHandHeight = newHandHeight;

            float newPosition = 0;

            switch (_axis)
            {
                case Axis.Z:
                    newPosition = transform.localPosition.z - handDifference;
                    break;
                case Axis.X:
                    newPosition = transform.localPosition.x - handDifference;
                    break;
                case Axis.Y:
                    newPosition = transform.localPosition.y - handDifference;
                    break;
            }

            SetPosition(newPosition);

            if (!_previousPress)
            {
                CheckPress();
            }
        }

        public void Hide()
        {
            _visualButton.SetActive(false);
        }

        public void Show()
        {
            _visualButton.SetActive(true);
        }

        private bool IsForwardInteraction(Transform interactor)
        {
            Vector3 interactorLocalPoint = transform.parent.InverseTransformPoint(interactor.position);
            return interactorLocalPoint.z > transform.localPosition.z;
        }

        private void StartPress(GameObject interactor)
        {
            _previousHandHeight = GetLocalPosition(_hoverInteractor.transform.position);
            _isPressing = true;
            _previousPress = false;
        }

        private void EndPress(GameObject interactor)
        {
            _isPressing = false;

            _previousHandHeight = 0.0f;

            _previousPress = false;
            SetPosition(_axisMax);
        }

        private void SetMinMax()
        {
            switch (_axis)
            {
                case Axis.Z:
                    _axisMin = transform.localPosition.z - _depth;
                    _axisMax = transform.localPosition.z;

                    break;
                case Axis.X:
                    _axisMin = transform.localPosition.x - _depth;
                    _axisMax = transform.localPosition.x;
                    break;
                case Axis.Y:
                    _axisMin = transform.localPosition.y - _depth;
                    _axisMax = transform.localPosition.y;
                    break;
            }
        }

        private float GetLocalPosition(Vector3 position)
        {
            Vector3 localPosition = transform.parent.InverseTransformPoint(position);

            float localPositionFloat = 0;

            switch (_axis)
            {
                case Axis.Z:
                    localPositionFloat = localPosition.z;
                    break;
                case Axis.X:
                    localPositionFloat = localPosition.x;
                    break;
                case Axis.Y:
                    localPositionFloat = localPosition.y;
                    break;
            }

            return localPositionFloat;
        }

        private void SetPosition(float position)
        {
            Vector3 newPosition = transform.localPosition;

            switch (_axis)
            {
                case Axis.Z:
                    newPosition.z = Mathf.Clamp(position, _axisMin, _axisMax);
                    break;
                case Axis.X:
                    newPosition.x = Mathf.Clamp(position, _axisMin, _axisMax);
                    break;
                case Axis.Y:
                    newPosition.y = Mathf.Clamp(position, _axisMin, _axisMax);
                    break;
            }

            transform.localPosition = newPosition;
        }

        private void CheckPress()
        {
            if (!IsButtonInPressedPosition())
            {
                return;
            }

            if (!_previousPress && Time.time - _lastPressTime > TimeDelay)
            {
                _lastPressTime = Time.time;
                OnPress?.Invoke();
                _previousPress = true;
            }
        }

        private bool IsButtonInPressedPosition()
        {
            bool result = false;

            switch (_axis)
            {
                case Axis.Z:
                    result = Math.Abs(transform.localPosition.z - _axisMin) < 0.001f;
                    break;
                case Axis.X:
                    result = Math.Abs(transform.localPosition.x - _axisMin) < 0.001f;
                    break;
                case Axis.Y:
                    result = Math.Abs(transform.localPosition.y - _axisMin) < 0.001f;
                    break;
            }
            return result;
        }

        private void OnRaycastButtonClick()
        {
            if (_raycaster == null || _raycaster.pokeStateData == null)
            {
                return;
            }

            if (_raycaster.pokeStateData.Value.interactionStrength < 0.8f)
            {
                OnPress?.Invoke();
            }
        }

        private void OnRaycastButtonHoverEnter() => OnHoverEnter?.Invoke();
        private void OnRaycastButtonHoverExit() => OnHoverExit?.Invoke();
    }
}
