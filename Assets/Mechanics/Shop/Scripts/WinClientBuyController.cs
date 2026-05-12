using System.Globalization;
using Assets.Localization;
using Assets.Scripts.Trading;
using Cysharp.Threading.Tasks;
using Mechanics.Transactions;
using Mechanics.UserWallet;
using Mechanics.UserWallet.Model;
using Mechanics.UserWallet.Service;
using UnityEngine;
using Zenject;

namespace Assets.Mechanics.Shop.Scripts
{
    public class WinClientBuyController : MonoBehaviour
    {
        [SerializeField] private LocalizationVariant _notEnoughMoney;
        [SerializeField] private LocalizationVariant _buyRequest;
        [SerializeField] private LocalizationVariant _buyComplete;

        private ITradeService _tradeService; //BuyItem
        private IWalletService _walletService; //GetWalletValue
        private ModalWindow _notificationWindow;

        public void Init(ITradeService tradeService, IWalletService walletService, ShopItemPlace[] places)
        {
            _tradeService = tradeService;
            _walletService = walletService;

            foreach (var place in places)
            {
                place.RequestToBuy += RequestToBuy;
            }

            var binder = FindObjectOfType<UIBinder>();

            if (binder != null) _notificationWindow = binder.UniversalModalWindow;
        }

        private async void RequestToBuy(ShopItemPlace shopItem)
        {
            if (shopItem == null || shopItem.ShopItemDto == null) return;

            var wallet = await _walletService.GetMyWalletAsync();
            var balance = wallet.sum;

            decimal? price = shopItem.ShopItemDto != null ? shopItem.ShopItemDto.Price : 0;
            string name = shopItem.ShopItemDto != null 
                          && shopItem.ShopItemDto.Item != null ? shopItem.ShopItemDto.Item.Name : "null";

            if (balance < shopItem.ShopItemDto.Price)
            {
                await _notificationWindow.Show(_notEnoughMoney.Localize().
                    Replace("{1}", (price - balance).ToString()).
                    Replace("{2}", (name)), 
                    "Ok");

                return;
            }
            else
            {
                bool buyApply = await _notificationWindow.Show(_buyRequest.Localize().
                        Replace("{1}", balance.ToString(CultureInfo.CurrentCulture)).
                        Replace("{2}", name).
                        Replace("{3}", price.ToString()),
                    "Ok",
                    "Cancel");

                if (buyApply)
                {
                    bool result = await _tradeService.BuyItem(shopItem.ShopItemDto);

                    if (result)
                    {
                        TwrWallet newWallet = null;

                        for (int i = 0; i < 5; i++)
                        {
                            await UniTask.Delay(500);
                            newWallet = await _walletService.GetMyWalletAsync();

                            if (newWallet.sum < wallet.sum) break;
                        }

                        await _notificationWindow.Show(
                            _buyComplete.Localize().Replace("{1}", name).Replace("{2}",
                                newWallet != null ? newWallet.sum.ToString(CultureInfo.CurrentCulture) : "null"),
                            "Ok");
                    }
                }
            }
        }        
    }
}
