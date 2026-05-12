using System;
using Assets.Scripts.Client;
using UnityEngine;
using Zenject;

namespace UI.Panels
{
    public class AuthenticationPanel : ViewPanel
    {
        #region Inspector

        [SerializeField] private ViewPanel _userPanel;
        [SerializeField] private ViewPanel _guestPanel;
        #endregion

        private IClientData _clientData;
        
        [Inject]
        private void Construct(IClientData clientData)
        {
            _clientData = clientData;
        }

        public override void ShowPanel()
        {
            _guestPanel.HidePanel();
            _userPanel.HidePanel();
            
            if(_clientData.IsGuest)
                _guestPanel.ShowPanel();
            else
            {
                _userPanel.ShowPanel();
            }
            base.ShowPanel();
        }
    }
}