using Assets.Localization;
using Mechanics.UserWallet.Model;
using Mechanics.UserWallet.Service;
using TMPro;
using UI;
using UnityEngine;
using Zenject;

namespace Mechanics.UserWallet
{
    public class WalletView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _walletText;
        [SerializeField] private LocalizationVariant BalanceLocalizationVariant;
        [SerializeField] private ViewPanel _walletPanel;
        [SerializeField] private ViewPanel _mainPanel;

        private IWalletService _walletService;

        [Inject]
        public void Init(IWalletService walletService)
        {
            _walletService = walletService;
        }

        private void OnEnable()
        {
            _walletPanel.PanelShown += OnPanelShow;
            _walletPanel.PanelHidden += OnPanelHide;
        }

        private void OnDisable()
        {
            _walletPanel.PanelShown -= OnPanelShow;
            _walletPanel.PanelHidden -= OnPanelHide;
        }
        
        private void OnPanelShow()
        {
            UpdateWalletTextAsync();
            _mainPanel.ShowPanel();
        }

        private void OnPanelHide()
        {
            _mainPanel.HidePanel();
        }

        private async void UpdateWalletTextAsync()
        {
            TwrWallet wallet = await _walletService.GetMyWalletAsync();
            string balanceString = BalanceLocalizationVariant.Localize();
            if (wallet != null) _walletText.text = $"{balanceString} " + ((int)wallet.sum) + " TWR";
            else _walletText.text = $"{balanceString} TWR";
        }

    }
}
