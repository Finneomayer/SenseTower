using System;
using Cysharp.Threading.Tasks;

namespace Mechanics.InAppPurchase.Interfaces
{
    public interface IAppPurchaseService
    {
        public void ConstructPurchaseList();
        public UniTask<string[]> GetAvailablePurchase();
        public void PurchaseByName(string tittle, Action<bool, string> result);
    }
}