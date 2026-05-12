using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Mechanics.UserWallet.Model;

namespace Assets.Scripts.Trading
{
    public interface ITradeService
    {
        UniTask<ShopDto[]> GetShops();
        UniTask<bool> BuyItem(ShopItemDto shopItemDto);
        UniTask<bool> SellItem(Guid objectId);
        UniTask<bool> BuySpaceAccess(Guid spaceId);
        
        event Action TransactionCompleted;
    }
}