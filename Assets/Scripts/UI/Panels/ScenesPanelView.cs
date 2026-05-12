using Assets.Scripts.Client;
using Assets.Scripts.Data;
using Assets.Scripts.Space;
using Mechanics.SpaceStaticObjectEditing;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Panels
{
    public class ScenesPanelView : ViewPanel
    {
        #region Inspector

        [SerializeField] private ViewPanel _editingPanel;
        [SerializeField] private ViewPanel _mainPanel;
        [SerializeField] private Button _enableSpaceAdministrationButton;
        [SerializeField] private Button _closeButton;

        #endregion

        private ISpaceEditingService _spaceEditingService;

        private ISpaceManager _spaceManager;
        private IClientData _clientData;

        [Inject]
        private void Construct(ISpaceManager spaceManager, IClientData clientData)
        {
            _spaceManager = spaceManager;
            _clientData = clientData;
        }

        private void OnEnable()
        {
#if UNITY_SERVER
         return;   
#endif
            if (_spaceManager != null && _spaceManager.CurrentTransitionTarget != null &&
                _spaceManager.CurrentTransitionTarget.SpaceOwner != null)
            {
                if (_clientData.UserId.ToString().ToLower()
                        .Equals(_spaceManager.CurrentTransitionTarget.SpaceOwner.UserId.ToString().ToLower()) ||
                    DataExtensions.AvailableUsers(_clientData.UserName) ||
                    DataExtensions.AvailableAdmin(_spaceManager.CurrentTransitionTarget, _clientData.UserId.ToString()))
                {
                    _enableSpaceAdministrationButton.gameObject.SetActive(true);
                }
                else
                {
                    _enableSpaceAdministrationButton.gameObject.SetActive(false);
                }
            }
            else
            {
                if (DataExtensions.AvailableUsers(_clientData.UserName) ||
                    DataExtensions.AvailableAdmin(_spaceManager.CurrentTransitionTarget, _clientData.UserId.ToString()))
                {
                    _enableSpaceAdministrationButton.gameObject.SetActive(true);
                }
                else
                {
                    _enableSpaceAdministrationButton.gameObject.SetActive(false);
                }
            }

            _closeButton.onClick.AddListener(OnCloseEditModeButtonClick);
            _enableSpaceAdministrationButton.onClick.AddListener(ShowEditPlacePanel);
        }

        private void OnDisable()
        {
            _closeButton.onClick.RemoveListener(OnCloseEditModeButtonClick);
            _enableSpaceAdministrationButton.onClick.RemoveListener(ShowEditPlacePanel);
        }

        public override void ShowPanel()
        {
            base.ShowPanel();

            if (_spaceEditingService == null)
            {
                _spaceEditingService = FindObjectOfType<SpaceEditingService>();
            }

            if (_spaceEditingService == null)
            {
                ShowMainPanel();
            }
            else
            {
                if (_spaceEditingService.EditingModeIsEnable())
                {
                    ShowEditPlacePanel();
                }
                else
                {
                    ShowMainPanel();
                }
            }
        }

        private void ShowEditPlacePanel()
        {
            _mainPanel.HidePanel();
            _editingPanel.ShowPanel();
        }

        private void ShowMainPanel()
        {
            _editingPanel.HidePanel();
            _mainPanel.ShowPanel();
        }

        public override void HidePanel()
        {
            base.HidePanel();

            if (_spaceEditingService == null)
            {
                _mainPanel.HidePanel();
            }
            else
            {
                if (_spaceEditingService.EditingModeIsEnable())
                {
                    _editingPanel.HidePanel();
                }
                else
                {
                    _mainPanel.HidePanel();
                }
            }
        }

        private void OnCloseEditModeButtonClick()
        {
            ShowMainPanel();
        }
    }
}