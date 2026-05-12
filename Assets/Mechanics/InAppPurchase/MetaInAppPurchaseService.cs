using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Mechanics.InAppPurchase.Interfaces;
using Oculus.Platform;
using Oculus.Platform.Models;
using UnityEngine;

namespace Assets.Mechanics.InAppPurchase
{
    public class MetaInAppPurchaseService : IAppPurchaseService
    {
        private Dictionary<string, string> _purchaseList = new();

        private Action<bool, string> _purchaseResult;
        private string[] skus = new[] {"HundredTWR"};
        private bool _isPurchaseRequestInProgress;

        public void ConstructPurchaseList()
        {
            if (Core.IsInitialized())
            {
                IAP.GetProductsBySKU(skus).OnComplete(GetViewerPurchasesCallback);
            }

            _isPurchaseRequestInProgress = false;
        }

        public async UniTask<string[]> GetAvailablePurchase()
        {
            UniTaskCompletionSource<string[]> unts = new();

            if (_isPurchaseRequestInProgress)
            {
                await UniTask.WaitUntil(() => !_isPurchaseRequestInProgress);
            }

            _isPurchaseRequestInProgress = true;

            if (_purchaseList.Count == 0)
                ConstructPurchaseList();
            else
            {
                _isPurchaseRequestInProgress = false;
            }

            await UniTask.WaitUntil(() => !_isPurchaseRequestInProgress);

            unts.TrySetResult(_purchaseList.Keys.ToArray());

            return await unts.Task;
        }

        public void PurchaseByName(string tittle, Action<bool, string> result)
        {
            _purchaseResult = result;

            IAP.LaunchCheckoutFlow(_purchaseList[tittle]).OnComplete(LaunchCheckoutFlowCallback);
        }

        private void GetViewerPurchasesCallback(Message<ProductList> msg)
        {
            if (msg.IsError)
            {
                Debug.Log(msg.GetError().Message);
                return;
            }

            for (int i = 0; i < msg.GetProductList().Count; i++)
            {
                var tempPurchaseItem = msg.GetProductList()[i];
                _purchaseList.Add(tempPurchaseItem.Name, tempPurchaseItem.Sku);
            }

            _isPurchaseRequestInProgress = false;
        }

        private void LaunchCheckoutFlowCallback(Message<Purchase> msg)
        {
            if (msg.IsError)
            {
                _purchaseResult.Invoke(false, msg.Data.ID);
                return;
            }

            Purchase p = msg.GetPurchase();
            _purchaseResult.Invoke(true, $"{p.ID}");
        }
    }
}