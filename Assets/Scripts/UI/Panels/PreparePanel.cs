using System;
using Assets.Scripts;

namespace UI
{
    public class PreparePanel : ViewPanel
    {
        #region Inspector

        public ApplicationRunner ApplicationRunner;
        public ViewPanel loadingPanel;
        public ViewPanel failPanel;
        public ButtonUI ButtonUI;
        #endregion

        private void Start()
        {
            ButtonUI.InteractElement.onClick.AddListener(ApplicationRunner._applicationBootstrapperInstance.InitApplication);
        }

        private void OnDestroy()
        {
            ButtonUI.InteractElement.onClick.RemoveAllListeners();
        }

        public void ShowFailPanel()
        {
            loadingPanel.HidePanel();
            failPanel.ShowPanel();
        }

        public void ShowLoadingPanel()
        {
            failPanel.HidePanel();
            loadingPanel.ShowPanel();
        }

        public void AllDisable()
        {
            failPanel.HidePanel();
            loadingPanel.HidePanel();
            HidePanel();
        }
    }
}