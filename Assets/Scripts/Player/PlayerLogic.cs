using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Localization;
using Assets.Mechanics.Tips;
using Assets.Scripts.API;
using Assets.Scripts.Client;
using Assets.Scripts.Space;
using Broadcasting;
using Infrastructure.AssetManagement;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using Zenject;
using InputDevice = UnityEngine.XR.InputDevice;

namespace Assets.Scripts.Player
{
    public class PlayerLogic : MonoBehaviour
    {
        [SerializeField] private InputActionReference _menuButton = null;
        [SerializeField] private PlayerBracelet _playerBraceletL;
        [SerializeField] private TipsViewer _tipsViewer;
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private Transform _cameraOffset;
        [SerializeField] private PlayerController[] _controllerObjects;
        [SerializeField] private PresentationLaserActivator _presentationLaserActivator;
        [SerializeField] private PlayerEmoji _playerEmoji;

        [Space]
        [Header("Localization")]
        [SerializeField]
        private LocalizationVariant _loginSuccessfulLocalizationVariant;
        [SerializeField]
        private LocalizationVariant _loginFailedLocalizationVariant;

        public PlayerBracelet GetPlayerBraceletL => _playerBraceletL;
        public PresentationLaserActivator PresentationLaserActivator => _presentationLaserActivator;
        public Transform CameraTransform => _cameraTransform;
        public PlayerController[] Controllers => _controllerObjects;

        public event Action StartControllerTracking;
        public event Action StartHandTracking;
        private bool _isControllerTracking;

        private UserInterfaceLogic _userInterfaceLogic;
        private SceneChangerView _sceneChangerView;

        private OnPlayerUI _onPlayerUi;
        private IClientData _clientData;
        private ISpaceManager _spaceManager = new SpaceManager();
        private static Vector3 _cachedPosition;

        private WaitForSeconds _checkingDevicesWaitForSeconds = new(0.1f);
        private UIBinder _playerUIMenu;

        private Vector3 _playerBraceletRPosition = Vector3.zero;
        private Vector3 _playerBraceletRRotation = Vector3.zero;
        [Inject]
        public void Init(ISpaceManager spaceManager, IClientData clientData)
        {
            _clientData = clientData;
            _spaceManager = spaceManager;
        }


        private void Start()
        {
            //PlayerStartRotate(180, 1.5f);
            _menuButton.action.started += MenuExecute;
            if (SceneManager.GetActiveScene().name == "TheEnterScene")
            {
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                ClearPosition();
            }

        }

        private void OnDisable()
        {
            _playerBraceletL.MainButton.onClick.RemoveAllListeners();
            _menuButton.action.started -= MenuExecute;
        }
        /// <summary>
        /// for network scenes
        /// </summary>
        /// <param name="sceneChangerView"></param>
        /// <param name="onPlayerUi"></param>
        public void Init(SceneChangerView sceneChangerView, OnPlayerUI onPlayerUi)
        {
            _sceneChangerView = sceneChangerView;

            _onPlayerUi = onPlayerUi;
            _onPlayerUi.SetParent(_cameraTransform);
            _onPlayerUi.SetPlayerCamera(_cameraTransform.GetComponent<Camera>());
            _onPlayerUi.LogoutPressed += _onPlayerUI_LogoutPressed;
            _onPlayerUi.ToggleInRoomChanged += _onPlayerUI_ToggleInRoomChanged;
            _onPlayerUi.ToggleContinuousMovingChanged += _onPlayerUI_ToggleContinuousMovingChanged;
            _onPlayerUi.ToggleVignetteChanged += _onPlayerUI_ToggleVignetteChanged;

            StartCoroutine(TrackingDevicesChangingActionsRoutine());
        }

        /// <summary>
        /// for enter scene
        /// </summary>
        /// <param name="userInterfaceLogic"></param>
        /// <param name="sceneChangerView"></param>
        /// <param name="onPlayerUi"></param>
        public void Init(UserInterfaceLogic userInterfaceLogic, SceneChangerView sceneChangerView, OnPlayerUI onPlayerUi)
        {
            _userInterfaceLogic = userInterfaceLogic;

            _userInterfaceLogic.LoginSuccessful += _userInterfaceLogic_LoginSuccessful;
            _userInterfaceLogic.LoginFailed += _userInterfaceLogic_LoginFailed;

            Init(sceneChangerView, onPlayerUi);
        }

        public void SetPlayerUI(UIBinder playerUIMenu) 
        {
            playerUIMenu.SetPlayer(this);
            _playerBraceletL.MainButton.onClick.AddListener(playerUIMenu.ToogleUIVisible);
            _playerUIMenu = playerUIMenu;
        }

        public void SetTipsService(UISwitcherService switcher,ISpaceModeData spaceModeData,ITipsSceneRepository tipsSceneRepository)
        {
            _tipsViewer.Construct(tipsSceneRepository,spaceModeData,switcher);
        }

        public void SetPlayerEmoji(EmojiSwitcher emojiSwitcher)
        {
            if (_playerEmoji == null) _playerEmoji = GetComponent<PlayerEmoji>();
            _playerEmoji.SetSwitcher(emojiSwitcher);
        }

        private void _onPlayerUI_ToggleVignetteChanged(bool isEnabled)
        {
            GetComponent<CustomContinuousMove>().isVignetteEnabled = isEnabled;
        }

        private void _onPlayerUI_ToggleContinuousMovingChanged(bool isEnabled)
        {        
            GetComponent<CustomContinuousMove>().isEnabled = isEnabled;
        }

        private void MenuExecute(InputAction.CallbackContext inputAction) 
        {
            if(_playerUIMenu != null)
                _playerUIMenu.ToogleUIVisible();
        }

        public void PlayerStartRotate(float oppositeDirection, float delay)
        {
            StartCoroutine(CameraCheck(oppositeDirection, delay));
        }

        public void PlayerRotate()
        {
           // _cameraOffset.eulerAngles += new Vector3(0, 0, 0);
        }

        private IEnumerator CameraCheck(float oppositeDirection, float delay)
        {
            yield return new WaitForSeconds(delay);
        
            if (_cameraTransform.eulerAngles.y < oppositeDirection + 90 && _cameraTransform.eulerAngles.y > oppositeDirection - 90)
            {
                _cameraOffset.eulerAngles = new Vector3(0, oppositeDirection, 0);
            }
            yield return null;
        }

        private void _onPlayerUI_ToggleInRoomChanged(bool obj)
        {
            Debug.Log($"Toggle {obj}");
        }

        private void _onPlayerUI_LogoutPressed()
        {
#if !UNITY_SERVER
            _spaceManager.ChangeSpace(SpaceType.EnterScene);
            _clientData.DeleteAllData();
#endif
        }

        private void _userInterfaceLogic_LoginSuccessful()
        {
            _onPlayerUi.ShowMessage(_loginSuccessfulLocalizationVariant.Localize(), 1.5f);
        }

        private void _userInterfaceLogic_LoginFailed()
        {
            _onPlayerUi.ShowMessage(_loginFailedLocalizationVariant.Localize(), 1.5f);
        }

        public void ShowMessage(string message)
        {
            _onPlayerUi.ShowMessage(message, 2.5f);
        }

        public void BlockExitMenu()
        {        
            _onPlayerUi.BlockButtons();
        }

        public void UnblockExitMenu()
        {
            _onPlayerUi.UnblockButtons();
        }

        public void TeleportTo(Transform transform)
        {
            transform.position = transform.position;
            PlayerRotate();
        }

        public void CorrectPosition()
        {
            transform.position = new Vector3(
                transform.position.x - _cameraTransform.position.x, 
                transform.position.y,
                transform.position.z - _cameraTransform.position.z
            );
        }

        public void SetPositionToZero()
        {
            transform.position = Vector3.zero;
        }

        public void SetY(float y)
        {
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
        }

        private IEnumerator TrackingDevicesChangingActionsRoutine()
        {
            // Enable/disable controllers objects depending on controller devices state
                  
            List<InputDevice> controllerDevices = new();
            while (true)
            {
                controllerDevices.Clear();
                InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller, controllerDevices);

                bool controllerIsTracking = false;

                foreach (var controller in _controllerObjects)
                {
                    controllerIsTracking = controllerIsTracking || (controllerDevices.Exists(device
                        => device.characteristics.HasFlag(controller.InputDeviceCharacteristics)));
                    if (controller.InputDeviceCharacteristics.HasFlag(InputDeviceCharacteristics.Left))
                    {
                        if (controllerIsTracking)
                        {
                            _playerBraceletL.BraceleteUI.Show();
                        }
                        else
                        {
                            _playerBraceletL.BraceleteUI.Hide();
                        }
                    }
                }

                //loop is devided for 2 loops to sum controllerIsTracking values into 1 value

                foreach (var controller in _controllerObjects)
                {
                    //controller.gameObject.SetActive(controllerIsTracking); //sensewarning

                    controller.SetControllerTracking(controllerIsTracking);
                    
                    if (controllerIsTracking && !_isControllerTracking) StartControllerTracking?.Invoke();
                    if (!controllerIsTracking && _isControllerTracking) StartHandTracking?.Invoke();

                    _isControllerTracking = controllerIsTracking;
                }
                yield return _checkingDevicesWaitForSeconds;
            }
        }

        public void SavePosition()
        {
            PlayerLogic._cachedPosition = transform.position;
        }

        public void ClearPosition()
        {
            PlayerLogic._cachedPosition = Vector3.zero;
        }

        public Vector3 GetCachedPosition()
        {
            return PlayerLogic._cachedPosition;
        }

        public PlayerController GetLeftArm()
        {
            return _controllerObjects[0];
        }

        public PlayerController GetRightArm()
        {
            return _controllerObjects[1];
        }
    }
}
