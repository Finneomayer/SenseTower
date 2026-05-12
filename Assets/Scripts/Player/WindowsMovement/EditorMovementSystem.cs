using System;
using System.Collections;
using System.Runtime.InteropServices;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Management;

namespace Assets.Scripts.Player.WindowsMovement
{
    public class EditorMovementSystem : NetworkBehaviour
    {
        [SerializeField, Range(0, 1)] private float _verticalSensievity = 0.5f;
        [SerializeField, Range(0, 1)] private float _horizontalSensievity = 0.2f;
        [SerializeField] private Transform _cameraRig;
        [SerializeField, Range(1f, 5f)] private float _movementSpeed = 3f;
        [SerializeField, Range(1f, 3f)] private float _jumpHeight = 1.5f;
        private CharacterController _characterController;
        private NetworkObject _networkObject;

        private float _vertical;
        private float _horizontal;
        private Vector2 _mouseInput;
        private float _xRot;
        private TrackedPoseDriver _trackedCamera;
        private Vector3? _prevParentPosition;
        private Vector3? _parentVelocity;
        private bool _jump;
        private float _velocityY;
        private Coroutine _fallingCheckingCoroutine;
        private bool _movementEnabled = true;

        public event Action Jumped;

        void Start()
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR


            _networkObject = GetComponent<NetworkObject>();
            _characterController = GetComponent<CharacterController>();
            _characterController.enabled = true;
            _trackedCamera = _cameraRig.GetComponent<TrackedPoseDriver>();
            //Cursor.lockState = CursorLockMode.Locked;
            if (SceneManager.GetActiveScene().name != "TheEnterScene")
            {
                //_cursorView.gameObject.SetActive(true);
                Cursor.lockState = CursorLockMode.Locked;
                Invoke("UnlockCursor", 0.05f);

                Application.focusChanged += Application_focusChanged;

                if (_fallingCheckingCoroutine == null) _fallingCheckingCoroutine = StartCoroutine(FallingChecking());
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Invoke("UnlockCursor", 0.05f);
            }
#endif
        }

        public override void OnDestroy()
        {
            if (SceneManager.GetActiveScene().name != "TheEnterScene")
            {
                Application.focusChanged -= Application_focusChanged;
            }
            base.OnDestroy();
        }

        private void Application_focusChanged(bool focused)
        {
            if (focused)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Invoke("UnlockCursor", 0.05f);
            }
        }

        private void UnlockCursor() => Cursor.visible = true;

        void Update()
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            if (!_networkObject.IsOwner) return;
            if (XRGeneralSettings.Instance.Manager.isInitializationComplete)
            {
                _trackedCamera.enabled = true;
                _characterController.enabled = true; //senseticket: character controller is need for Yacht scene
            }
            else
            {
                _characterController.enabled = true;
                _trackedCamera.enabled = false;

                _horizontal = Input.GetAxis("Horizontal");
                _vertical = Input.GetAxis("Vertical");
                _jump = Input.GetButton("Jump");
                _mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

                DoRotate(_mouseInput);
                _mouseInput = Vector2.zero;

                if (Application.isEditor)
                {
                    Cursor.visible = true;
                }
            }
            //Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = true;
#endif
        }

        private void LateUpdate()
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR

            if (transform.parent != null)
            {
                if (_prevParentPosition.HasValue)
                {
                    _parentVelocity = (transform.parent.position - _prevParentPosition) / Time.deltaTime;
                }

                _prevParentPosition = transform.parent.position;
            }
            else
            {
                _parentVelocity = null;
                _prevParentPosition = null;
            }

            if (XRGeneralSettings.Instance.Manager.isInitializationComplete) return;
            if (!_networkObject.IsOwner) return;

            if (_movementEnabled)
            {
                DoMovement(_horizontal, _vertical, _jump);
            }

            _horizontal = 0;
            _vertical = 0;

            if (_jump)
            {
                Jumped?.Invoke();
            }
            _jump = false;
#endif
        }

        public void SetEnabledMovement(bool enabled)
        {
            _movementEnabled = enabled;
            _characterController.enabled = enabled;
        }

        public void SetPosition(Vector3 position, Quaternion rotation)
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            if (_characterController != null)
                _characterController.enabled = false;
            transform.position = position;
            transform.rotation = rotation;
            if (_characterController != null && _movementEnabled)
                _characterController.enabled = true;
#endif
        }

        private Vector3 DirectionCalculation(float horizontalInput, float jump, float verticalInput)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                horizontalInput = horizontalInput * 3;
                verticalInput = verticalInput * 3;
            }

            Vector3 direction = transform.right * horizontalInput * _movementSpeed +
                                transform.forward * verticalInput * _movementSpeed;
            direction.y = jump;
            
            if (_parentVelocity.HasValue)
            {
                direction += _parentVelocity.Value;
            }
            return direction;
        }

        private void DoMovement(float horizontalInput, float verticalInput, bool jump)
        {
            if ((horizontalInput < 0.01f &&  horizontalInput > -0.01f)&&
                (verticalInput < 0.01f &&  verticalInput > -0.01f) &&
                !jump &&
                _characterController.isGrounded) return;

            if (jump && _characterController.isGrounded)
            {
                _velocityY += _jumpHeight;
            }

            if (!_characterController.isGrounded)
            {
                _velocityY += -9.8f * Time.deltaTime;
            }

            if (_characterController.isGrounded && !_jump) _velocityY = 0;

            if (_characterController.transform.parent != null)
            {
                _characterController.SimpleMove(DirectionCalculation(horizontalInput, 0f, verticalInput));
            }
            else _characterController.Move(DirectionCalculation(horizontalInput * Time.deltaTime, _velocityY * 0.02f, verticalInput * Time.deltaTime));
        }

        private void DoRotate(Vector2 mouseInput)
        {
            if (mouseInput == Vector2.zero)
            {
                return;
            }
            float x = mouseInput.x * Time.deltaTime * Screen.width * _horizontalSensievity;
            transform.Rotate(transform.up * x);

            float y = mouseInput.y * Time.deltaTime * Screen.height * _verticalSensievity;

            _xRot = _xRot - y;
            _xRot = Mathf.Clamp(_xRot, -90, 90);

            _cameraRig.localRotation = Quaternion.Euler(_xRot, 0, 0);
        }

        private IEnumerator FallingChecking()
        {
            bool firstCheck = false;

            while (true)
            {
                firstCheck = false;

                if (!_characterController.isGrounded &&_characterController.velocity.y < -2f) firstCheck = true;
                
                yield return new WaitForSeconds(2f);

                if (!_characterController.isGrounded &&_characterController.velocity.y < -2f && firstCheck) 
                {
                    SetPosition(Vector3.zero, Quaternion.identity);
                }
            }
        }
    }
}