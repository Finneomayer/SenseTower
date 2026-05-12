using Assets.Scripts.Client;
using UnityEngine;
using Zenject;

namespace UI.Panels
{
    public class EntryViewPanel : ViewPanel
    {
        #region Inspector

        [SerializeField] private ViewPanel AuthorizationPanel;
        [SerializeField] private AuthenticationPanel AuthenticationPanel;
        private IClientData _clientData;

        #endregion

        [Inject]
        private void Construct(IClientData clientData)
        {
            _clientData = clientData;
        }

        public override void ShowPanel()
        {
            ShowContentPanel();
            base.ShowPanel();
        }

        private void ShowContentPanel()
        {
            AuthenticationPanel.HidePanel();
            AuthorizationPanel.HidePanel();
        
            if (string.IsNullOrEmpty(_clientData.AccessToken))
            {
                AuthorizationPanel.ShowPanel();
            }
            else
            {
                AuthenticationPanel.ShowPanel();
            }
        }
    }
}