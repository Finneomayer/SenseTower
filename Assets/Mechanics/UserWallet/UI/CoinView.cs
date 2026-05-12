using Mechanics.UserWallet.Model;
using Mechanics.UserWallet.Service;
using TMPro;
using UnityEngine;
using Zenject;

namespace Mechanics.UserWallet
{
    public class CoinView : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private TMP_Text _cointValueText;

        #endregion

        private IWalletService _walletService;

        [Inject]
        private void Construct(IWalletService wallet)
        {
            _walletService = wallet;
        }

        private void Awake()
        {
#if !UNITY_SERVER
            FillCoinView();
#endif
        }

        private void OnEnable()
        {
            _walletService.WalletChanged += OnWalletChanged;
        }

        private void OnDisable()
        {
            _walletService.WalletChanged -= OnWalletChanged;
        }

        private void OnWalletChanged()
        {
            SetCoinValue(_walletService.TwrWallet);
        }

        private async void FillCoinView()
        {
            SetCoinValue(await _walletService.GetMyWalletAsync());
        }

        private void SetCoinValue(TwrWallet wallet)
        {
            if (wallet != null)
                _cointValueText.text = ((int)wallet.sum).ToString();
        }
    }
}