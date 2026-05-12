using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;
using System;
using Assets.Localization;

namespace SenseAI.Robot.UI
{
    public class InfoPanelController : MonoBehaviour
    {
        [Header("UI elements:")]
        [SerializeField] GameObject _mainPanel;
        [SerializeField] GameObject _terminal;
        [SerializeField] GameObject _firstInteractPanel;
        [SerializeField] GameObject _buttonsPanel;
        [SerializeField] TextMeshProUGUI _outPutMsgText;
        [SerializeField] GameObject _outputMsgObject;
        [SerializeField] GameObject _nameField;
        [SerializeField] Button _goToOfficeButton;
        [SerializeField] Button _goToBaseButton;

        [SerializeField]
        private LocalizationVariant HelloLocalizationVariant;
        [SerializeField]
        private LocalizationVariant FollowMeLocalizationVariant;
        [SerializeField]
        private LocalizationVariant DestinationReachedLocalizationVariant;

        [Header("Events:")]
        public UnityEvent OnHideMainMenu = new UnityEvent();
        public UnityEvent OnShowMainMenu = new UnityEvent();

        public UnityEvent OnShowButtonsMenu = new UnityEvent();
        public UnityEvent OnHideButtonsMenu = new UnityEvent();

        public UnityEvent OnFirstInteractMenuShow = new UnityEvent();
        public UnityEvent OnFirstInteractMenuHide = new UnityEvent();


        [Header("Settings:")]
        [SerializeField]
        private bool _isOutputing;
        [SerializeField] RobotDevice _robot;
        [SerializeField] MVPNavigation _mvpNavigation;

        public event Action GoToOfficeButtonPressed;
        public event Action GoToBaseButtonPressed;

        private void Awake()
        {
            Cashing();
            EventSettings();
        }

        private void OnEnable()
        {
            _goToOfficeButton.onClick.AddListener(OnGoToOfficeButtonPressed);
            _goToBaseButton.onClick.AddListener(OnGoToBaseButtonPressed);
        }

        private void OnDisable()
        {
            _goToOfficeButton.onClick.RemoveListener(OnGoToOfficeButtonPressed);
            _goToBaseButton.onClick.RemoveListener(OnGoToBaseButtonPressed);
        }

        private void OnGoToOfficeButtonPressed()
        {
            GoToOfficeButtonPressed?.Invoke();
        }

        private void OnGoToBaseButtonPressed()
        {
            GoToBaseButtonPressed?.Invoke();
        }

        private void Cashing()
        {
            if (_robot == null)
                _robot = GetComponentInParent<RobotDevice>();

            if (_mvpNavigation == null)
                _mvpNavigation = GetComponentInParent<MVPNavigation>();

            _outputMsgObject = _outPutMsgText.transform.parent.gameObject;
        }

        private void EventSettings()
        {
            if (_robot != null)
                _robot.OnOutPutInfoSetted.AddListener(SetInfoMsg);
            else
                Debug.Log("Info panel не имеет ссылка на дейвайс робота!");


            OnHideMainMenu.AddListener(HideMainMenu);
            OnShowMainMenu.AddListener(ShowMainMenu);

            OnShowButtonsMenu.AddListener(ShowButtonsMenu);
            OnHideButtonsMenu.AddListener(HideButtonsMenu);

            OnFirstInteractMenuShow.AddListener(ShowFirstInteractMenu);
            OnFirstInteractMenuHide.AddListener(HideFirstInteractMenu);

            OnFirstInteractMenuShow.AddListener(StartMsg);
        }

        public void StartMsg()
        {
            _robot.OutputInfo = HelloLocalizationVariant.Localize(_robot.DeviceOwner);
        }

        public void ShowMsg(string msg)
        {
            HideFirstInteractMenu();
            if (!_outputMsgObject.activeInHierarchy)
                _terminal.SetActive(true);

            _robot.OutputInfo = msg;
        }
        public void HideEveryrhing()
        {
            _mainPanel.SetActive(false);
            HideButtonsMenuInvoke();
            HideFirstInteractMenuInvoke();
            _outputMsgObject.SetActive(false);
            StopAllCoroutines();
            _robot.OutputInfo = "";
        }

        public void ShowName()
        {
            if (!_mainPanel.activeInHierarchy)
                _nameField.SetActive(true);

            /*
            if (!_outputMsgObject.activeInHierarchy)
                ShowMsg(_robot.DeviceOwner);
            */
        }
        public void HideName()
        {

            _nameField.SetActive(false);
            /*
            if (!_outputMsgObject.activeInHierarchy)
                ShowMsg(_robot.DeviceOwner);
            */
        }

        #region FirstInteract
        public void ShowFirstInteractMenu()
        {
            _mainPanel.SetActive(true);
            _outputMsgObject.SetActive(true);
            _firstInteractPanel.SetActive(true);
        }
        public void HideFirstInteractMenu()
        {
            _firstInteractPanel.SetActive(false);
        }
        public void ShowFirstInteractMenuInvoke()
        {
            if (!_buttonsPanel.activeInHierarchy)
                OnFirstInteractMenuShow.Invoke();

        }
        public void HideFirstInteractMenuInvoke()
        {
            OnFirstInteractMenuHide.Invoke();
        }

        #endregion

        #region ButtonsMenu
        public void ShowGoToOfficeFinishedMessage()
        {
            ShowMsg(DestinationReachedLocalizationVariant.Localize(_robot.DeviceOwner));
        }
        public void ShowGoToOfficeStartMessage()
        {
            ShowMsg(FollowMeLocalizationVariant.Localize());
        }

        public void ShowButtonsMenu()
        {
            HideFirstInteractMenu();
            _robot.OutputInfo = "Выбирите локацию:";
            _buttonsPanel.SetActive(true);
        }
        public void HideButtonsMenu()
        {
            _buttonsPanel.SetActive(false);
        }
        public void ShowButtonsMenuInvoke()
        {
            OnShowButtonsMenu.Invoke();
        }
        public void HideButtonsMenuInvoke()
        {
            OnHideButtonsMenu.Invoke();
        }
        #endregion


        #region MainMenu
        public void ShowMainMenu()
        {
            _mainPanel.SetActive(true);
        }
        public void ShowMainMenuInvoke()
        {
            if (!_mainPanel.activeInHierarchy)
                OnShowMainMenu.Invoke();
        }
        public void HideMainMenu()
        {
            _mainPanel.SetActive(false);
        }
        public void HideMainMenuInvoke()
        {
            OnHideMainMenu.Invoke();
        }
        #endregion
        public void SetInfoMsg()
        {
            StopAllCoroutines();
            var msg = _robot.OutputInfo;
            _isOutputing = false;
            if (!_isOutputing)
                StartCoroutine(DelayedMsg(msg));
        }

        private IEnumerator DelayedMsg(string msg)
        {
            yield return new WaitWhile(() => _isOutputing);
            _isOutputing = true;
            _outPutMsgText.text = msg;
            yield return new WaitForSeconds(60);
            _isOutputing = false;
        }
    }
}