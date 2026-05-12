using Assets.Localization;
using Assets.Scripts.Space;
using Data;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Mechanics.Tips
{
    public class TipsViewer : ViewPanel
    {
        #region Inspector
        [SerializeField] private Button _closeButton;
        [SerializeField] private TMP_Text _tipsText;
        [SerializeField] private LocalizationVariant _defaultTipsMessage;

        #endregion

        protected UISwitcherService _uiSwitcherService;

        protected ISpaceModeData _spaceModeData;
        protected ITipsSceneRepository _tipsSceneRepository;

        public void Construct(ITipsSceneRepository tipsSceneRepository, ISpaceModeData spaceModeData,
            UISwitcherService switcherService)
        {
            _uiSwitcherService = switcherService;
            _uiSwitcherService.PanelTypeClick += OnChangePanelTypeClick;
            _tipsSceneRepository = tipsSceneRepository;
            _tipsSceneRepository.RegisterViewer(this);

            _spaceModeData = spaceModeData;
            if (_spaceModeData.SpaceModeType == Enumenators.SpaceModeType.Help)
                ShowPanel();
        }

        private void Start()
        {
            Init();
        }

        private void OnDisable()
        {
            if(_closeButton != null)
                _closeButton.onClick.RemoveListener(OnCloseButtonClick);
            
            if (_uiSwitcherService != null)
                _uiSwitcherService.PanelTypeClick -= OnChangePanelTypeClick;
        }

        public void SetViewText(string viewText)
        {
            string result = string.IsNullOrEmpty(viewText) ? _defaultTipsMessage.Localize() : viewText;

            _tipsText.text = result;
        }

        public override void ShowPanel()
        {
            base.ShowPanel();

            SetViewText(_tipsSceneRepository.GetTipsText());
        }

        protected virtual void Init()
        {
#if !UNITY_ANDROID
            gameObject.SetActive(false);
#endif
            if (_spaceModeData == null)
                return;
            
            if(_closeButton != null)
                _closeButton.onClick.AddListener(OnCloseButtonClick);
            
            if (_spaceModeData.SpaceModeType == Enumenators.SpaceModeType.Help)
                ShowPanel();
        }

        protected virtual void ToogleTips()
        {
            if (IsVisible())
            {
                HidePanel();
                _spaceModeData.SpaceModeType = Enumenators.SpaceModeType.Normal;
            }
            else
            {
                _spaceModeData.SpaceModeType = Enumenators.SpaceModeType.Help;
                ShowPanel();
            }
        }

        private void OnChangePanelTypeClick(Enumenators.PanelType panelType)
        {
            if (panelType != Enumenators.PanelType.TipsPanel)
                return;

            ToogleTips();
        }

        private void OnCloseButtonClick()
        {
            if(_spaceModeData != null)
                _spaceModeData.SpaceModeType = Enumenators.SpaceModeType.Normal;
            
            HidePanel();
        }
    }
}